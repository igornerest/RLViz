using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class QFunction : IEnumerable
{

    private Dictionary<State, Dictionary<Action, float>> qMap =
        new Dictionary<State, Dictionary<Action, float>>();

    private static Random random = new Random();

    private bool shouldUpdateBoundaries = false;
    private float minQValue = 0f;
    private float maxQValue = 0f;

    public float this[State state, Action action]
    {
        get { return qMap.ContainsKey(state) && qMap[state].ContainsKey(action) ? qMap[state][action] : 0f; }

        set {
            shouldUpdateBoundaries = true;

            if (!qMap.ContainsKey(state)) 
                qMap[state] = new Dictionary<Action, float>();

            qMap[state][action] = value; 
        }
    }

    public QFunction() { }

    public void Clear()
    {
        qMap.Clear();
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

        foreach (var action in ActionExtensions.GetValidActions())
        {
            float currQ = this[state, action];
            if (currQ > maxQ)
            {
                maxQ = currQ;
                bestAction = action;
            }
        }

        return (maxQ, bestAction);
    }

    public Action EGreedy(State state, float epsilon)
    {
        if (random.NextDouble() < epsilon)
        {
            var allActions = ActionExtensions.GetValidActions();
            int randomIndex = random.Next(0, allActions.Count);
            return allActions[randomIndex];
        } 
        else
        {
            var (_, bestAction) = MaxQ(state);
            return bestAction;
        }
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

    public float Normalize(float qValue, float defaultValue = 0.5f)
    {
        if (qMap.Count == 0)
        {
            return defaultValue;
        }

        if (shouldUpdateBoundaries)
        {
            List<float> currValues = new List<float>();
            foreach (var actionValuesMap in qMap)
            {
                currValues.Add(actionValuesMap.Value.Values.Max());
            }

            minQValue = currValues.Min();
            maxQValue = currValues.Max();
            shouldUpdateBoundaries = false;
        }

        return (qValue - minQValue) / (maxQValue - minQValue);
    }

    public IEnumerator GetEnumerator()
    {
        foreach (var u in qMap)
        {
            yield return u;
        }
    }

    public QFunction Clone()
    {
        QFunction clonedQFunction = new QFunction();
        clonedQFunction.qMap = new Dictionary<State, Dictionary<Action, float>>(this.qMap);
        return clonedQFunction;
    }

}
