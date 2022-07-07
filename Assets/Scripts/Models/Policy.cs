using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Policy : IEnumerable
{
    private Dictionary<State, Action> pMap = new Dictionary<State, Action>();

    private Action defaultAction = Action.UP;

    public Action this[State key]
    {
        get { return pMap.ContainsKey(key) ? pMap[key] : defaultAction; }

        set { pMap[key] = value; }
    }
    
    public Policy() { }

    public void Reset()
    {
        foreach (var state in pMap.Keys.ToList())
        {
            pMap[state] = defaultAction;
        }
    }

    public void Remove(State state)
    {
        this.pMap.Remove(state);
    }

    public int Count()
    {
        return pMap.Count;
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
