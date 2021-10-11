using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLManager : MonoBehaviour
{
    [SerializeField]
    private Transform floorPrefab;

    private List<GridState> grid = new List<GridState>();
    private HashSet<Position> gridPositions = new HashSet<Position>();

    private Dictionary<Deviation, float> deviationProbs =
        new Dictionary<Deviation, float> {    
            { Deviation.FORWARD,    0.8f },
            { Deviation.LEFT,       0.1f },
            { Deviation.RIGHT,      0.1f }
        };

    void Start()
    {
        setMDP();
    }

    void Update()
    {
        
    }

    private void setMDP()
    {
        int rows = 3;
        int columns = 4;

        for (int i = 1; i <= rows; i++)
        {
            for (int j = 1; j <= columns; j++)
            {
                var currPosition = (j, i);

                if (currPosition != (2, 2))
                {
                    GridState gridState = new GridState();
                    gridState.SetFloorAtPosition(new Position(j, i), floorPrefab);

                    if (currPosition == (4, 3))
                    {
                        gridState.SetSuccessGoal();
                    }
                    else if (currPosition == (4, 2))
                    {
                        gridState.SetFailureGoal();
                    }

                    grid.Add(gridState);
                    gridPositions.Add(new Position(j, i));
                }
            }
        }

        foreach (GridState gridState in grid)
        {
            gridState.EvaluateProbabilities(deviationProbs, gridPositions);
        }
    }
}
