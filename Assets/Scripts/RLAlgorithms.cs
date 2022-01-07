using System;
using System.Collections;
using UnityEngine;

public static class RLAlgorithms
{
    public const String ALGORITHM_VALUE_ITERATION = "Value Iteration";
    public const String ALGORITHM_POLICY_ITERATION = "Policy Iteration";
    public const String ALGORITHM_Q_LEARNING = "Q Learning";
    public const String ALGORITHM_SARSA = "Sarsa";

    public static IEnumerator ValueIteration(MDP mdp, RLAlgorithmState algorithmState)
    {
        Debug.Log("Running Value Iteration algorithm");

        mdp.UseVFunction();

        for (algorithmState.CurrIt = 0; algorithmState.CurrIt < algorithmState.MaxIt; algorithmState.CurrIt++) { 

            Utility newUtility = mdp.Utility.Clone();  
            foreach (State state in mdp.GetAllStates())
            {
                var (maxUtility, _) = newUtility.GetMaxExpectedValue(state);
                mdp.Utility[state] = state.Reward + mdp.Gamma * maxUtility;
            }

            foreach (State state in mdp.GetAllNonTerminalStates())
            {
                var (_, bestAction) = mdp.Utility.GetMaxExpectedValue(state);
                mdp.Policy[state] = bestAction;
            }

            algorithmState.ItCount++;
            yield return new WaitForFixedUpdate();
        }
    }

    public static IEnumerator PolicyIteration(MDP mdp, RLAlgorithmState algorithmState)
    {
        Debug.Log("Running Policy Iteration algorithm");

        mdp.UseVFunction();

        for (algorithmState.CurrIt = 0; algorithmState.CurrIt < algorithmState.MaxIt; algorithmState.CurrIt++)
        {
            policyEvaluation(mdp);
            policyImprovement(mdp);

            algorithmState.CurrIt++;
            yield return new WaitForFixedUpdate();
        }
    }

    public static IEnumerator QLearning(MDP mdp, RLAlgorithmState algorithmState)
    {
        Debug.Log("Running Q-Learning algorithm");

        mdp.UseQFunction();

        for (algorithmState.CurrIt = 0; algorithmState.CurrIt < algorithmState.MaxIt; algorithmState.CurrIt++)
        {
            State currState = mdp.InitialState;
   
            while (!currState.IsTerminal)
            {
                var currAction = mdp.QFunction.EGreedy(currState, mdp.Epsilon);
                var nextState = currState.NextState(currAction);
                var (nextStateMaxQ, _) = mdp.QFunction.MaxQ(nextState);
                
                mdp.QFunction[currState, currAction] = mdp.QFunction[currState, currAction] + mdp.Alpha * (nextState.Reward + mdp.Gamma * nextStateMaxQ - mdp.QFunction[currState, currAction]);

                currState = nextState;

                yield return new WaitForFixedUpdate();
            }

            algorithmState.CurrIt++;
        }

        Policy policy = mdp.QFunction.GetPolicy();
        mdp.UpdatePolicy(policy);
    }

    public static IEnumerator Sarsa(MDP mdp, RLAlgorithmState algorithmState)
    {
        Debug.Log("Running Sarsa algorithm");

        mdp.UseQFunction();

        for (algorithmState.CurrIt = 0; algorithmState.CurrIt < algorithmState.MaxIt; algorithmState.CurrIt++)
        {
            State currState = mdp.InitialState;
            Action currAction = mdp.QFunction.EGreedy(currState, mdp.Epsilon);
            
            while (!currState.IsTerminal)
            {
                var nextState = currState.NextState(currAction);
                var nextAction = mdp.QFunction.EGreedy(nextState, mdp.Epsilon);

                mdp.QFunction[currState, currAction] = mdp.QFunction[currState, currAction] + mdp.Alpha * (nextState.Reward + mdp.Gamma * mdp.QFunction[nextState, nextAction] - mdp.QFunction[currState, currAction]);

                currState = nextState;
                currAction = nextAction;

                yield return new WaitForFixedUpdate();
            }

            algorithmState.CurrIt++;
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
