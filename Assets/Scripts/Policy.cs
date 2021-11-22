using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Policy : IEnumerable
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
        if (state.IsTerminal)
        {
            return new Tuple<float, Action>(0f, Action.NONE);
        }

        Action currAction = this[state];

        float currExpectedValue = 0f;
        foreach (var (nextState, probability) in state.NextLikelyStates[currAction])
        {
            currExpectedValue += probability * utility[nextState];
        }

        return new Tuple<float, Action>(currExpectedValue, currAction);
    }

    public IEnumerator GetEnumerator()
    {
        foreach (var u in pMap)
        {
            yield return u;
        }
    }

    public Policy Clone()
    {
        Policy clonedPolicy = new Policy();
        clonedPolicy.pMap = new Dictionary<State, Action>(this.pMap);
        return clonedPolicy;
    }
}
