using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MDP
{
    Dictionary<Vector3Int, State> grid = new Dictionary<Vector3Int, State>();

    public Utility uMap { private set; get; }

    public Policy pMap { private set; get; }

    public State InitialState { private set; get;  }

    public void AddState(State state, bool isInitial = false)
    {
        grid[state.Position] = state;

        if (isInitial)
        {
            InitialState = state;
        }
    }

    public List<State> getAllStates()
    {
        return grid.Values.ToList();
    }

    public void UpdateUtility(Utility uMap)
    {
        this.uMap = uMap;

        foreach (State state in grid.Values)
        {
            state.Utility = uMap[state];
        }
    }

    public void UpdatePolicy(Policy pMap)
    {
        this.pMap = pMap;

        foreach (State state in grid.Values)
        {
            state.Policy = pMap[state];
        }
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
