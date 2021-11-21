using System;
using System.Collections.Generic;
using UnityEngine;

public class Policy : MonoBehaviour
{
    private Dictionary<State, Action> pMap = new Dictionary<State, Action>();

    public Action this[State key]
    {
        // TODO: Return default policy according to state attributes
        get { return pMap.ContainsKey(key) ? pMap[key] : Action.UP; }

        set { pMap[key] = value; }
    }

    public Policy() { }

    public void Clear()
    {
        pMap.Clear();
    }

    public Tuple<float, Action> GetMaxExpectedValue(State state, Utility utility)
    {
        Action currAction = this[state];

        float currExpectedValue = 0f;
        foreach (var (nextState, probability) in state.NextLikelyStates[currAction])
        {
            currExpectedValue += probability * utility[nextState];
        }

        return new Tuple<float, Action>(currExpectedValue, currAction);
    }
}
