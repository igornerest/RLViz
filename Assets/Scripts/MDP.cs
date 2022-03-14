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

    public Dictionary<Deviation, float> DeviationProbs { get; } =
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

    public string GetStatusLog()
    {
        return string.Format("Entries -> grid: {0}, q-function: {1}, u-function: {2}, policy: {3}",
            grid.Count, qFunction.Count(), utility.Count(), policy.Count());
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
        utility.Reset();
        policy.Reset();
        qFunction.Reset();
    }

    public void AddStates(List<State> states)
    {
        foreach (State state in states)
        {
            AddState(state, evaluateProbabilities: false);
        }

        EvaluateProbabilities();
    }

    public void AddState(State state, bool evaluateProbabilities = true)
    {
        grid[state.Position] = state;

        // This is necessary to reinforce default value if
        // no entry was previosuly computed
        foreach (var action in ActionExtensions.GetValidActions())
        {
            qFunction[state, action] = qFunction[state, action];
        }
        utility[state] = utility[state];
        policy[state] = policy[state];

        if (evaluateProbabilities)
        {
            EvaluateProbabilities();
        }
    }

    public bool IsGridState(State state)
    {
        return grid.ContainsKey(state.Position);
    }

    public void RemoveState(State state)
    {
        grid.Remove(state.Position);
        qFunction.Remove(state);
        utility.Remove(state);
        policy.Remove(state);

        EvaluateProbabilities();
    }

    public void UpdateDeviationProb(Deviation deviation, float probability)
    {
        DeviationProbs[deviation] = probability;

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
                
                foreach (var likelyDeviation in DeviationProbs)
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
