using System;
using System.Linq;
using UnityEngine;

public static class RLAlgorithms
{
    public const String ALGORITHM_VALUE_ITERATION = "Value Iteration";
    public const String ALGORITHM_POLICY_ITERATION = "Policy Iteration";
    public const String ALGORITHM_TIME_DIFFERENCE = "Time Difference";
    public const String ALGORITHM_Q_LEARNING = "Q Learning";

    public static void ValueIteration(MDP mdp)
    {
        float delta;

        do
        {
            delta = 0;

            Utility newUtility = mdp.Utility.Clone();  
            foreach (State state in mdp.GetAllStates())
            {
                var (maxUtility, _) = newUtility.GetMaxExpectedValue(state);
                mdp.Utility[state] = state.Reward + mdp.Gamma * maxUtility;

                delta = Math.Max(delta, Math.Abs(mdp.Utility[state] - newUtility[state]));
            }

            foreach (State state in mdp.GetAllNonTerminalStates())
            {
                var (_, bestAction) = mdp.Utility.GetMaxExpectedValue(state);
                mdp.Policy[state] = bestAction;
            }
        } while (delta > mdp.ValueIterationDelta);
    }

    public static void PolicyIteration(MDP mdp)
    {
        while (true)
        {
            policyEvaluation(mdp);
            bool changed = policyImprovement(mdp);
            if (!changed) break;
        }
    }

    // For now, we will use a pre-calculated policy
    // What if we get a random one at the beginning? 
    public static void TimeDifference(MDP mdp)
    {
        Utility utility = new Utility();

        foreach (var state in mdp.GetAllStates())
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
        QFunction qFunction = new QFunction(mdp.GetAllStates());

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
    
    private static void policyEvaluation(MDP mdp)
    {
        float delta;
        
        do
        {
            delta = 0;

            Utility newUtility = mdp.Utility.Clone();
            foreach (State state in mdp.GetAllStates())
            {
                var (expectedValue, _) = mdp.Policy.GetMaxExpectedValue(state, mdp.Utility);
                mdp.Utility[state] = state.Reward + mdp.Gamma * expectedValue;

                delta = Math.Max(delta, Math.Abs(mdp.Utility[state] - newUtility[state]));
            }
        } while (delta > mdp.ValueIterationDelta);
    }

    private static bool policyImprovement(MDP mdp)
    {
        bool changed = false;

        foreach (State state in mdp.GetAllNonTerminalStates())
        {
            var (uUtility, uAction) = mdp.Utility.GetMaxExpectedValue(state);
            var (pUtility, _) = mdp.Policy.GetMaxExpectedValue(state, mdp.Utility);

            if (uUtility > pUtility)
            {
                mdp.Policy[state] = uAction;
                changed = true;
            }
        }

        return changed;
    }
}
