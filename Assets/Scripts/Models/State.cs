using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class State
{
    public Vector3Int Position { private set; get; }

    public bool IsTerminal { set; get; }

    public float Reward { set; get; }

    private static Random random = new Random();

    private Dictionary<Action, List<Tuple<State, float>>> nextLikelyStates =
        new Dictionary<Action, List<Tuple<State, float>>>();

    public Dictionary<Action, List<Tuple<State, float>>> NextLikelyStates { 
        get { return nextLikelyStates;  } 
    }

    public State(int xPos, int yPos, float reward = 0, bool isTerminal = false)
    {
        this.Position = new Vector3Int(xPos, 0, yPos);
        this.Reward = reward;
        this.IsTerminal = isTerminal;
    }

    public void UpdateNextLikelyStates(Action action, List<Tuple<State, float>> nextLikelyStates)
    {
        this.nextLikelyStates[action] = nextLikelyStates;
    }

    public State NextState(Action action)
    {
        double randomChoice = random.NextDouble();
        double accProbability = 0;

        foreach (var (nextState, probability) in NextLikelyStates[action])
        {
            accProbability += probability;

            if (randomChoice <= accProbability)
            {
                return nextState;
            }
        }

        throw new Exception("The sum of the probabilities of all outcomes is not equal to 1");
    }

    public Vector3Int GetPositionFromAction(Action action)
    {
        switch (action)
        {
            case Action.UP:     return this.Position + Vector3Int.forward;
            case Action.RIGHT:  return this.Position + Vector3Int.right;
            case Action.DOWN:   return this.Position + Vector3Int.back;
            case Action.LEFT:   return this.Position + Vector3Int.left;
            default:            return this.Position;
        }
    }

    public bool EqualsPosition(Vector3Int compPosition)
    {
        return this.Position.Equals(compPosition);
    }

    public override bool Equals(object obj)
    {
        State compState = obj as State;

        if (compState == null)
        {
            return false;
        }
        
        return compState.EqualsPosition(this.Position);
    }

    public override int GetHashCode()
    {
        return this.Position.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("State at position ({0},{1})", this.Position.x, this.Position.z);
    }

}
