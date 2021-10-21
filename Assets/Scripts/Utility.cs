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
            uMap[state] = state.IsTerminal? state.Reward : 0f;
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

    public Tuple<float, Action> GetMaxExpectedValue(State state)
    {
        float maxExpectedValue = float.NegativeInfinity;
        Action bestAction = Action.UP;

        foreach (Action currAction in state.NextLikelyStates.Keys)
        {
            float currExpectedValue = 0f;
            foreach (var (nextState, probability) in state.NextLikelyStates[currAction])
            {
                currExpectedValue += probability * uMap[nextState];
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
}
