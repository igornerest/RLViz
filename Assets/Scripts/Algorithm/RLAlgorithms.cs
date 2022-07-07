using System;
using System.Collections;
using UnityEngine;

public static class RLAlgorithms
{
    public const String ALGORITHM_VALUE_ITERATION = "key_Value_Iteration";
    public const String ALGORITHM_POLICY_ITERATION = "key_Policy_Iteration";
    public const String ALGORITHM_Q_LEARNING = "key_QLearning";
    public const String ALGORITHM_SARSA = "key_SARSA";

    public static IEnumerator ValueIteration(MDP mdp, RLAlgorithmState algorithmState)
    {
        algorithmState.HasFinishedIterations = false;

        for (int it = 0; it < algorithmState.MaxIt; it++) {
            algorithmState.AddIterationState(mdp.Utility.Clone(), mdp.Policy.Clone());

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
            
            yield return new WaitForFixedUpdate();
        }

        algorithmState.HasFinishedIterations = true;
    }

    public static IEnumerator PolicyIteration(MDP mdp, RLAlgorithmState algorithmState)
    {
        algorithmState.HasFinishedIterations = false;

        for (int it = 0; it < algorithmState.MaxIt; it++)
        {
            algorithmState.AddIterationState(mdp.Utility.Clone(), mdp.Policy.Clone());

            policyEvaluation(mdp);
            policyImprovement(mdp);

            yield return new WaitForFixedUpdate();
        }

        algorithmState.HasFinishedIterations = true;
    }

    public static IEnumerator RevertVFunctionStates(MDP mdp, RLAlgorithmState algorithmState)
    {
        algorithmState.HasFinishedIterations = false;

        for (int it = 0; it < algorithmState.MaxIt && algorithmState.HasVFunctionStates(); it++)
        {
            Utility previousUtility = algorithmState.UtilityStack.Pop();
            Policy previousPolicy = algorithmState.PolicyStack.Pop();

            foreach (State state in mdp.GetAllStates())
            {
                mdp.Utility[state] = previousUtility[state];
                mdp.Policy[state] = previousPolicy[state];
            }

            yield return new WaitForFixedUpdate();
        }

        algorithmState.HasFinishedIterations = true;
    }

    public static IEnumerator QLearning(MDP mdp, RLAlgorithmState algorithmState)
    {
        algorithmState.HasFinishedIterations = false;

        int it = 0;
        while (it < algorithmState.MaxIt)
        {
            State currState = algorithmState.AgentState == null || algorithmState.AgentState.IsTerminal
                ? mdp.InitialState
                : algorithmState.AgentState;

            while (mdp.IsGridState(currState) && it < algorithmState.MaxIt)
            {
                if (currState.IsTerminal)
                {
                    mdp.QFunction[currState, Action.NONE] = currState.Reward;
                    break;
                }

                algorithmState.AddIterationState(mdp.QFunction.Clone(), currState);
                algorithmState.AgentState = currState;

                var currAction = mdp.QFunction.EGreedy(currState, mdp.Epsilon);
                var nextState = currState.NextState(currAction);
                var (nextStateMaxQ, _) = mdp.QFunction.MaxQ(nextState);
                
                mdp.QFunction[currState, currAction] = mdp.QFunction[currState, currAction] + mdp.Alpha * (currState.Reward + mdp.Gamma * nextStateMaxQ - mdp.QFunction[currState, currAction]);

                currState = nextState;
                it++;

                yield return new WaitForFixedUpdate();
            }

            algorithmState.AddIterationState(mdp.QFunction.Clone(), currState);
            algorithmState.AgentState = currState;

            yield return new WaitForFixedUpdate();
        }

        algorithmState.HasFinishedIterations = true;
    }

    public static IEnumerator Sarsa(MDP mdp, RLAlgorithmState algorithmState)
    {
        algorithmState.HasFinishedIterations = false;

        int it = 0;
        while (it < algorithmState.MaxIt)
        {
            State currState = algorithmState.AgentState == null || algorithmState.AgentState.IsTerminal
                ? mdp.InitialState
                : algorithmState.AgentState;
            Action currAction = mdp.QFunction.EGreedy(currState, mdp.Epsilon);
            
            while (mdp.IsGridState(currState) && it < algorithmState.MaxIt)
            {
                if (currState.IsTerminal)
                {
                    mdp.QFunction[currState, Action.NONE] = currState.Reward;
                    break;
                }

                algorithmState.AddIterationState(mdp.QFunction.Clone(), currState);
                algorithmState.AgentState = currState;

                var nextState = currState.NextState(currAction);
                var nextAction = mdp.QFunction.EGreedy(nextState, mdp.Epsilon);

                mdp.QFunction[currState, currAction] = mdp.QFunction[currState, currAction] + mdp.Alpha * (currState.Reward + mdp.Gamma * mdp.QFunction[nextState, nextAction] - mdp.QFunction[currState, currAction]);

                currState = nextState;
                currAction = nextAction;
                it++;

                yield return new WaitForFixedUpdate();
            }

            algorithmState.AddIterationState(mdp.QFunction.Clone(), currState);
            algorithmState.AgentState = currState;

            yield return new WaitForFixedUpdate();
        }

        algorithmState.HasFinishedIterations = true;
    }

    public static IEnumerator RevertQFunctionStates(MDP mdp, RLAlgorithmState algorithmState)
    {
        algorithmState.HasFinishedIterations = false;

        for (int it = 0; it < algorithmState.MaxIt && algorithmState.HasQFunctionStates(); it++)
        {
            QFunction previousQFunction = algorithmState.QFunctionStack.Pop();
            State previousState = algorithmState.AgentStateStack.Pop();

            foreach (State state in mdp.GetAllStates())
            {
                foreach (Action action in ActionExtensions.GetValidActions()) {
                    mdp.QFunction[state, action] = previousQFunction[state, action];
                }
            }
            algorithmState.AgentState = previousState;

            yield return new WaitForFixedUpdate();
        }

        algorithmState.HasFinishedIterations = true;
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
