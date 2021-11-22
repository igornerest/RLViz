using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : IEnumerable
{
    private Dictionary<State, float> uMap = new Dictionary<State, float>();

    public float this[State key]
    {
        get { return uMap.ContainsKey(key) ? uMap[key] : 0f; }

        set { uMap[key] = value; }
    }

    public Utility() { }

    public void Clear()
    {
        uMap.Clear();
    }

    public Tuple<float, Action> GetMaxExpectedValue(State state)
    {
        if (state.IsTerminal)
        {
            return new Tuple<float, Action>(0f, Action.NONE);
        }

        float maxExpectedValue = float.NegativeInfinity;
        Action bestAction = Action.UP;

        foreach (Action currAction in state.NextLikelyStates.Keys)
        {
            float currExpectedValue = 0f;
            foreach (var (nextState, probability) in state.NextLikelyStates[currAction])
            {
                currExpectedValue += probability * this[nextState];
            }

            if (currExpectedValue > maxExpectedValue)
            {
                maxExpectedValue = currExpectedValue;
                bestAction = currAction;
            }
        }

        return new Tuple<float, Action>(maxExpectedValue, bestAction); 
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();

        foreach (KeyValuePair<State, float> u in uMap)
        {
            sb.Append(u.Key + " " + u.Value + "\n");
        }

        return sb.ToString();
    }

    public IEnumerator GetEnumerator()
    {
        foreach (var u in uMap)
        {
            yield return u;
        }
    }

    public Utility Clone()
    {
        Utility clonedUtility = new Utility();
        clonedUtility.uMap = new Dictionary<State, float>(this.uMap);
        return clonedUtility;
    }
}
