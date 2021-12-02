using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MDP
{
    private Dictionary<Vector3Int, State> grid = new Dictionary<Vector3Int, State>();

    private float gamma;
    private float epsilon;
    private float alpha;

    private QFunction qFunction = new QFunction();
    private Utility utility = new Utility();
    private Policy policy = new Policy();

    private State initialState;

    private bool isUsingVFunction;

    public bool IsUsingVFunction
    {
        get { return isUsingVFunction; }
    }

    public float Gamma
    {
        get { return gamma; }
    }

    public float Epsilon
    {
        get { return epsilon;  }
    }

    public float Alpha
    {
        get { return alpha;  }
    }

    public float ValueIterationDelta
    {
        get { return epsilon * (1 - gamma) / gamma; }
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

    public MDP(float gamma = 0.9f, float epsilon = 0.2f, float alpha = 0.1f)
    {
        this.gamma = gamma;
        this.epsilon = epsilon;
        this.alpha = alpha;
    }

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
