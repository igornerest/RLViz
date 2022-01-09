using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MDP
{
    private Dictionary<Vector3Int, State> grid = new Dictionary<Vector3Int, State>();

    private QFunction qFunction = new QFunction();
    private Utility utility = new Utility();
    private Policy policy = new Policy();

    private State initialState;

    private bool isUsingVFunction;

    public bool IsUsingVFunction
    {
        get { return isUsingVFunction; }
    }

    // Discount-rate parameter, between 0 and 1
    public float Gamma { get; set; }

    // Probability of taking a random action in an E-greedy policy
    public float Epsilon { get; set; }

    // Value Function learning rate
    public float Alpha { get; set; }

    public float ValueIterationDelta
    {
        get { return Epsilon * (1 - Gamma) / Gamma; }
    }

    public QFunction QFunction
    {
        get { return qFunction; }
    }

    public Utility Utility
    {
        get { return utility; }
    }

    public Policy Policy
    {
        get { return policy; }
    }

    public State InitialState
    {
        get { return initialState; }
    }

    public State AgentState { get; set; }

    public void Reset()
    {
        utility.Clear();
        policy.Clear();
        qFunction.Clear();
    }

    public void AddState(State state, bool isInitial = false)
    {
        grid[state.Position] = state;

        if (isInitial)
        {
            this.initialState = state;
        }
    }

    public List<State> GetAllStates()
    {
        return grid.Values.ToList();
    }

    public List<State> GetAllNonTerminalStates()
    {
        return grid.Values.Where(state => !state.IsTerminal).ToList();
    }

    public void UpdateUtility(Utility utility)
    {
        this.utility = utility;
    }

    public void UpdatePolicy(Policy policy)
    {
        this.policy = policy;
    }

    public void EvaluateProbabilities(Dictionary<Deviation, float> likelyDeviations)
    {
        foreach (State state in GetAllNonTerminalStates())
        {
            Debug.Log("--------> Evaluation Probabilities for state " +  state);

            foreach (Action actualAction in ActionExtensions.GetValidActions())
            {
                Debug.Log("---> Actual action: " + actualAction);

                var likelyNextStates = new List<Tuple<State, float>>();
                
                foreach (var likelyDeviation in likelyDeviations)
                {
                    Deviation deviation = likelyDeviation.Key;
                    float probability = likelyDeviation.Value;

                    Action deviatedAction = ActionExtensions.Deviate(actualAction, deviation);
                    Vector3Int nextPosition = state.GetPositionFromAction(deviatedAction);
                    State nextState = grid.ContainsKey(nextPosition) ? grid[nextPosition] : state;

                    likelyNextStates.Add(new Tuple<State, float>(nextState, probability));
                    Debug.Log("Next State: " + nextState + ". Probability: " + probability);
                }

                state.UpdateNextLikelyStates(actualAction, likelyNextStates);
            }
        }
    }

    public void UseQFunction()
    {
        isUsingVFunction = false;
    }

    public void UseVFunction()
    {
        isUsingVFunction = true;
    }
}
