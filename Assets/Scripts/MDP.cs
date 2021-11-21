using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MDP
{
    private Dictionary<Vector3Int, State> grid = new Dictionary<Vector3Int, State>();

    private Utility utility = new Utility();
    private Policy policy = new Policy();
    private State initialState;

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

    public void Reset()
    {
        utility.Clear();
        policy.Clear();
    }

    public void AddState(State state, bool isInitial = false)
    {
        grid[state.Position] = state;

        if (isInitial)
        {
            this.initialState = state;
        }
    }

    public List<State> getAllStates()
    {
        return grid.Values.ToList();
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
        foreach (State state in grid.Values.ToList())
        {
            Debug.Log("--------> Evaluation Probabilities for state " +  state);

            if (state.IsTerminal)
                continue;

            foreach (Action actualAction in Enum.GetValues(typeof(Action)))
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
}
