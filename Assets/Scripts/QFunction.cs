using System;
using System.Collections.Generic;
using UnityEngine;

public class QFunction
{

    private Dictionary<State, Dictionary<Action, float>> qMap =
        new Dictionary<State, Dictionary<Action, float>>();

    public float this[State state, Action action]
    {
        get { return qMap[state][action]; }

        set {
            if (!qMap.ContainsKey(state)) 
                qMap[state] = new Dictionary<Action, float>();

            qMap[state][action] = value; 
        }
    }

    public QFunction(List<State> states)
    {
        foreach (State state in states)
        {
            foreach (Action action in state.NextLikelyStates.Keys)
            {
                this[state, action] = 0f;
            }
        }
    }

    public (float, Action) MaxQ(State state)
    {
        float maxQ = float.NegativeInfinity;
        Action bestAction = Action.UP;   // TO DO: Define no action;

        foreach (var action in qMap[state].Keys)
        {
            float currQ = qMap[state][action];
            if (currQ > maxQ)
            {
                maxQ = currQ;
                bestAction = action;
            }
        }

        return (maxQ, bestAction);
    }

    public Policy GetPolicy()
    {
        Policy policy = new Policy();

        foreach (var state in qMap.Keys)
        {
            var (_, bestAction) = MaxQ(state);
            policy[state] = bestAction;
        }

        return policy;
    }

}
