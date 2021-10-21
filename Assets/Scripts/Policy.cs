using System;
using System.Collections.Generic;
using UnityEngine;

public class Policy : MonoBehaviour
{
    private Dictionary<State, Action> pMap = new Dictionary<State, Action>();

    public Action this[State key]
    {
        get { return pMap[key]; }

        set { pMap[key] = value; }
    }

    public Policy(List<State> states)
    {
        foreach (State state in states)
        {
            pMap[state] = Action.UP; // TODO: Set according to state attributes
        }
    }

    public Tuple<float, Action> GetMaxExpectedValue(State state, Utility utility)
    {
        Action currAction = pMap[state];

        float currExpectedValue = 0f;
        foreach (var (nextState, probability) in state.NextLikelyStates[currAction])
        {
            currExpectedValue += probability * utility[nextState];
        }

        return new Tuple<float, Action>(currExpectedValue, currAction);
    }
}
