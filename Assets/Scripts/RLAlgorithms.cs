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
