using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridState : MonoBehaviour
{
    public TMP_Text policyText;
    public TMP_Text utilityText;
    private Transform floorTransform;

    private Dictionary<Action, List<Tuple<float, Position>>> probs =
        new Dictionary<Action, List<Tuple<float, Position>>>();

    public Position Position { get; private set; }

    public float Reward { get; private set; }  = -0.04f;

    public float Utility { get; private set; } = 0f;

    public bool IsTerminal { get; private set; } =  false;

    public void Start()
    {
        //policyText = transform.Find("Canvas").transform.Find("PolicyText").GetComponent<TMP_Text>();
        //utilityText = transform.Find("Canvas").transform.Find("UtilityText").GetComponent<TMP_Text>();
    }

    public void EvaluateProbabilities(Dictionary<Deviation, float> deviationProbs, Dictionary<Position, GridState> grid)
    {
        Debug.Log(this.Position);

        if (this.IsTerminal)
        {
            Debug.Log("Terminal State. Exiting probability evaluation");
            return;
        }

        foreach (Action action in Enum.GetValues(typeof(Action))) {
            probs[action] = new List<Tuple<float, Position>>();

            foreach(var deviationProb in deviationProbs)
            {
                Deviation deviation = deviationProb.Key;
                float prob = deviationProb.Value;

                Action deviatedAction = ActionExtensions.Deviate(action, deviation);

                Position positionAfterAction = this.Position.MoveWithAction(deviatedAction);
                if (!grid.ContainsKey(positionAfterAction))
                {
                    positionAfterAction = this.Position;
                }

                probs[action].Add(new Tuple<float, Position>(prob, positionAfterAction));

                Debug.Log(
                    String.Format("Action: {0}, Deviation: {1}, ActualState: {2}, Prob: {3}",
                        action, deviation, positionAfterAction, prob)
                );
            }
        }
    }

    public List<float> GetExpectedValuesFromUtility(Dictionary<Position, float> utilities)
    {
        var expectedVals = new List<float>();

        foreach (List<Tuple<float, Position>> probPerDeviation in probs.Values)
        {
            float expectedVal = 0f;
            foreach (Tuple<float, Position> prob in probPerDeviation)
            {
                float probability = prob.Item1;
                Position pos = prob.Item2;

                expectedVal += probability * utilities[pos];
            }
            expectedVals.Add(expectedVal);
        }
       
        return expectedVals;
    }

    public void SetFloorAtPosition(Position position, Transform floorPrefab)
    {
        this.Position = position;

        if (floorTransform == null)
        {
            floorTransform = Instantiate(floorPrefab, position.getWorldPosition(), Quaternion.identity);
        }
        else
        {
            floorTransform.position = position.getWorldPosition();
        }
    }

    public void SetSuccessGoal()
    {
        this.IsTerminal = true;
        this.Reward = 1f;
        floorTransform.GetComponent<Renderer>().material.color = Color.green;
    }

    public void SetFailureGoal()
    {
        this.IsTerminal = true;
        this.Reward = -1f;
        floorTransform.GetComponent<Renderer>().material.color = Color.red;
    }

    public void UpdateUtility(float utility)
    {
        this.Utility = utility;
        utilityText.text = utility.ToString();
    }
}
