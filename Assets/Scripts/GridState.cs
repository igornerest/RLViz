using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridState : MonoBehaviour
{
    private Transform floorTransform;

    private Position position;
    
    private float reward = -0.04f;

    private Dictionary<Action, List<Tuple<float, Position>>> probs = 
        new Dictionary<Action, List<Tuple<float, Position>>>();

    public void EvaluateProbabilities(Dictionary<Deviation, float> deviationProbs, HashSet<Position> gridPositions)
    {
        Debug.Log(position);

        foreach (Action action in Enum.GetValues(typeof(Action))) {
            probs[action] = new List<Tuple<float, Position>>();

            foreach(var deviationProb in deviationProbs)
            {
                Deviation deviation = deviationProb.Key;
                float prob = deviationProb.Value;

                Action deviatedAction = ActionExtensions.Deviate(action, deviation);

                Position positionAfterAction = this.position.MoveWithAction(deviatedAction);
                if (!gridPositions.Contains(positionAfterAction))
                {
                    positionAfterAction = position;
                }

                probs[action].Add(new Tuple<float, Position>(prob, positionAfterAction));

                Debug.Log(
                    String.Format("Action: {0}, Deviation: {1}, ActualState: {2}, Prob: {3}",
                        action, deviation, positionAfterAction, prob)
                );
            }
        }
    }

    public void SetFloorAtPosition(Position position, Transform floorPrefab)
    {
        this.position = position;

        if (floorTransform == null)
        {
            floorTransform = Instantiate(floorPrefab, position.getWorldPosition(), Quaternion.identity);
        }
        else
        {
            floorTransform.position = position.getWorldPosition();
        }
    }
}
