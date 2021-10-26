using System;
using System.Linq;
using UnityEngine;

public static class RLAlgorithms
{
    public static void valueIteration(MDP mdp, float gamma, float episilon)
    {
        float delta = episilon * (1 - gamma) / gamma + 1;

        Utility utility = new Utility(mdp.getAllStates());

        while (delta > episilon * (1 - gamma) / gamma)
        {
            Utility newUtility = new Utility(utility);

            delta = 0;
            
            foreach (State state in mdp.getAllStates())
            {
                if (state.IsTerminal)
                {
                    utility[state] = state.Reward;
                }
                else
                {
                    var (maxUtility, _) = newUtility.GetMaxExpectedValue(state);
                    utility[state] = state.Reward + gamma * maxUtility;
                }

                delta = Math.Max(delta, Math.Abs(utility[state] - newUtility[state]));
            }
        }

        mdp.UpdateUtility(utility);
    }

    public static void policyIteration(MDP mdp, float gamma)
    {
        
        Utility utility = new Utility(mdp.getAllStates());
        Policy policy = new Policy(mdp.getAllStates());

        bool changed = true;
        while (changed)
        {
            utility = policyEvaluation(mdp, policy, utility, gamma);
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
    public static void TimeDifference(MDP mdp, float alpha, float gamma)
    {
        Utility utility = new Utility(mdp.getAllStates());

        foreach (var state in mdp.getAllStates())
        {
            utility[state] = state.Reward;
        }

        for (int i = 0; i < 1000; i++)
        {
            State lastState = mdp.InitialState;
            State currState = lastState.NextState(mdp.pMap[lastState]);

            while (!currState.IsTerminal)
            {
                utility[lastState] = utility[lastState] + alpha * (lastState.Reward + gamma * utility[currState] - utility[lastState]);
                lastState = currState;
                currState = currState.NextState(mdp.pMap[lastState]);
            }

            // We still need to update the state prior to the terminal state
            utility[lastState] = utility[lastState] + alpha * (lastState.Reward + gamma * utility[currState] - utility[lastState]);
        }

        mdp.UpdateUtility(utility);
    }

    
    public static void QLearning(MDP mdp, float alpha, float gamma)
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
                
                qFunction[prevState, prevAction] = qFunction[prevState, prevAction] + alpha * (prevState.Reward + gamma * maxQ - qFunction[prevState, prevAction]);

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
    
    private static Utility policyEvaluation(MDP mdp, Policy policy, Utility utility, float gamma)
    {
        foreach (State state in mdp.getAllStates())
        {
            if (state.IsTerminal)
                continue;

            var (expectedValue ,_) = policy.GetMaxExpectedValue(state, utility);
            utility[state] = state.Reward + gamma * expectedValue;
        }

        return utility;
    }
}
