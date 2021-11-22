using System;
using System.Linq;
using UnityEngine;

public static class RLAlgorithms
{
    public const String ALGORITHM_VALUE_ITERATION = "Value Iteration";
    public const String ALGORITHM_POLICY_ITERATION = "Policy Iteration";
    public const String ALGORITHM_TIME_DIFFERENCE = "Time Difference";
    public const String ALGORITHM_Q_LEARNING = "Q Learning";

    public static void valueIteration(MDP mdp)
    {
        Utility utility = new Utility();
        float delta;

        do
        {
            delta = 0;
            Utility newUtility = utility.Clone();

            foreach (State state in mdp.getAllStates())
            {
                if (state.IsTerminal)
                {
                    utility[state] = state.Reward;
                }
                else
                {
                    var (maxUtility, _) = newUtility.GetMaxExpectedValue(state);
                    utility[state] = state.Reward + mdp.Gamma * maxUtility;
                }

                delta = Math.Max(delta, Math.Abs(utility[state] - newUtility[state]));
            }
        } while (delta > mdp.ValueIterationDelta);

        mdp.UpdateUtility(utility);
    }

    public static void policyIteration(MDP mdp)
    {
        
        Utility utility = new Utility();
        Policy policy = new Policy();

        bool changed = true;
        while (changed)
        {
            utility = policyEvaluation(mdp, policy, utility);
            changed = false;

            foreach (State state in mdp.getAllStates())
            {
                if (state.IsTerminal)
                    continue;

                var (uUtility, uAction) = utility.GetMaxExpectedValue(state);
                var (pUtility, _)       = policy.GetMaxExpectedValue(state, utility);

                if (uUtility > pUtility)
                {
                    policy[state] = uAction;
                    changed = true;
                }
            }
        }

        mdp.UpdateUtility(utility);
        mdp.UpdatePolicy(policy);
    }

    // For now, we will use a pre-calculated policy
    // What if we get a random one at the beginning? 
    public static void TimeDifference(MDP mdp)
    {
        Utility utility = new Utility();

        foreach (var state in mdp.getAllStates())
        {
            utility[state] = state.Reward;
        }

        for (int i = 0; i < 1000; i++)
        {
            State lastState = mdp.InitialState;
            State currState = lastState.NextState(mdp.Policy[lastState]);

            while (!currState.IsTerminal)
            {
                utility[lastState] = utility[lastState] + mdp.Alpha * (lastState.Reward + mdp.Gamma * utility[currState] - utility[lastState]);
                lastState = currState;
                currState = currState.NextState(mdp.Policy[lastState]);
            }

            // We still need to update the state prior to the terminal state
            utility[lastState] = utility[lastState] + mdp.Alpha * (lastState.Reward + mdp.Gamma * utility[currState] - utility[lastState]);
        }

        mdp.UpdateUtility(utility);
    }

    
    public static void QLearning(MDP mdp)
    {
        QFunction qFunction = new QFunction(mdp.getAllStates());

        for (int i = 0; i < 1000; i++)
        {
            State prevState = mdp.InitialState;
            var (_, prevAction) = qFunction.MaxQ(prevState);
            State currState = prevState.NextState(prevAction);
   
            while (!prevState.IsTerminal)
            {
                if (currState.IsTerminal)
                {
                    qFunction[currState, prevAction] = currState.Reward;
                }

                var (maxQ, currAction) = qFunction.MaxQ(currState);
                
                qFunction[prevState, prevAction] = qFunction[prevState, prevAction] + mdp.Alpha * (prevState.Reward + mdp.Gamma * maxQ - qFunction[prevState, prevAction]);

                prevState = currState;

                if (!currState.IsTerminal)
                {
                    currState = currState.NextState(currAction);
                    prevAction = currAction;
                }
            }
        }

        Policy policy = qFunction.GetPolicy();
        mdp.UpdatePolicy(policy);
    }
    
    private static Utility policyEvaluation(MDP mdp, Policy policy, Utility utility)
    {
        foreach (State state in mdp.getAllStates())
        {
            if (state.IsTerminal)
                continue;

            var (expectedValue ,_) = policy.GetMaxExpectedValue(state, utility);
            utility[state] = state.Reward + mdp.Gamma * expectedValue;
        }

        return utility;
    }
}
