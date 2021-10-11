using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RLManager : MonoBehaviour
{
    [SerializeField] private Transform floorPrefab;

    private Dictionary<Position, GridState> grid = new Dictionary<Position, GridState>();

    private Dictionary<Deviation, float> deviationProbs =
        new Dictionary<Deviation, float> {    
            { Deviation.FORWARD,    0.8f },
            { Deviation.LEFT,       0.1f },
            { Deviation.RIGHT,      0.1f }
        };

    void Start()
    {
        setMDP();

        RLAlgorithms.valueIteration(grid, gamma: 0.9f, episilon: 0.001f);
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
                Position position = new Position(j, i);

                if (!position.Is(2, 2))
                {
                    GridState gridState = new GridState();
                    gridState.SetFloorAtPosition(position, floorPrefab);
      
                    if (position.Is(4, 3))
                    {
                        gridState.SetSuccessGoal();
                    }
                    else if (position.Is(4, 2))
                    {
                        gridState.SetFailureGoal();
                    }

                    grid[position] = gridState;
                }
            }
        }

        foreach (GridState gridElem in grid.Values)
        {
            gridElem.EvaluateProbabilities(deviationProbs, grid);
        }
    }
}
