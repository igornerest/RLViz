using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : IEnumerable
{
    private Dictionary<State, float> uMap = new Dictionary<State, float>();

    public float this[State key]
    {
        get { return uMap[key]; }

        set { uMap[key] = value; }
    }

    public Utility(List<State> states)
    {
        foreach (State state in states)
        {
            uMap[state] = 0f;
        }
    }

    public Utility(Utility utility)
    {
        this.uMap = new Dictionary<State, float>(utility.uMap);
    }

    public IEnumerator GetEnumerator()
    {
        foreach (var u in uMap)
        {
            yield return u;
        }
    }

    public List<float> GetExpectedValues(State state)
    {
        var expectedVals = new List<float>();

        Debug.Log("antes " + state);
        foreach (List<Tuple<State, float>> nextLikelyStates in state.NextLikelyStates.Values)
        {
            Debug.Log("dentro");
            float expectedVal = 0f;
            foreach (var (nextState, probability) in nextLikelyStates)
            {
                expectedVal += probability * uMap[nextState];
            }
            expectedVals.Add(expectedVal);
        }

        return expectedVals;
    }
}
