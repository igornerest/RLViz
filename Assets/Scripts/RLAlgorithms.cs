using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public static class RLAlgorithms
{
    public const String ALGORITHM_VALUE_ITERATION = "Value Iteration";
    public const String ALGORITHM_POLICY_ITERATION = "Policy Iteration";
    public const String ALGORITHM_Q_LEARNING = "Q Learning";

    public static void ValueIteration(MDP mdp)
    {
        mdp.UseVFunction();

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
        mdp.UseVFunction();

        while (true)
        {
            policyEvaluation(mdp);
            bool changed = policyImprovement(mdp);
            if (!changed) break;
        }
    }

    public static void QLearning(MDP mdp)
    {
        mdp.UseQFunction();

        for (int i = 0; i < 1000; i++)
        {
            State currState = mdp.InitialState;
   
            while (!currState.IsTerminal)
            {
                var currAction = mdp.QFunction.EGreedy(currState, mdp.Epsilon);
                var nextState = currState.NextState(currAction);
                var (nextStateMaxQ, _) = mdp.QFunction.MaxQ(nextState);
                
                mdp.QFunction[currState, currAction] = mdp.QFunction[currState, currAction] + mdp.Alpha * (nextState.Reward + mdp.Gamma * nextStateMaxQ - mdp.QFunction[currState, currAction]);

                currState = nextState;
            }
        }

        Policy policy = mdp.QFunction.GetPolicy();
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
