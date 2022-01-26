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

    private bool isUsingVFunction;
    private bool isUsingQFunction;

    // TODO: let it customizable
    private Dictionary<Deviation, float> deviationProbs =
        new Dictionary<Deviation, float> {
            { Deviation.FORWARD,    0.8f },
            { Deviation.LEFT,       0.1f },
            { Deviation.RIGHT,      0.1f }
        };

    public bool IsUsingVFunction { get => isUsingVFunction; }
    public bool IsUsingQFunction { get => isUsingQFunction; }

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

    public State InitialState { get; set; }

    public void Reset()
    {
        utility.Clear();
        policy.Clear();
        qFunction.Clear();
    }

    public void AddStates(List<State> states)
    {
        foreach (State state in states)
        {
            grid[state.Position] = state;
        }

        EvaluateProbabilities();
    }

    public void AddState(State state)
    {
        grid[state.Position] = state;

        EvaluateProbabilities();
    }

    public void PromoteStateToInitial(State state)
    {
        if (state.IsTerminal)
        {
            throw new Exception("Terminal state cannot be initial");
        }

        if (!grid.ContainsKey(state.Position))
        {
            throw new Exception("The state is not part of the current grid");
        }

        InitialState = state;
    }

    public void UnsetInitialState()
    {
        InitialState = null;
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

    public void EvaluateProbabilities()
    {
        foreach (State state in GetAllNonTerminalStates())
        {
            Debug.Log("--------> Evaluation Probabilities for state " +  state);

            foreach (Action actualAction in ActionExtensions.GetValidActions())
            {
                Debug.Log("---> Actual action: " + actualAction);

                var likelyNextStates = new List<Tuple<State, float>>();
                
                foreach (var likelyDeviation in deviationProbs)
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
        isUsingQFunction = true;
        isUsingVFunction = false;
    }

    public void UseVFunction()
    {
        isUsingVFunction = true;
        isUsingQFunction = false;
    }
}
