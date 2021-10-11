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
        int rows = 3;
        int columns = 4;

        for (int i = 1; i <= rows; i++)
        {
            for (int j = 1; j <= columns; j++)
            {
                GridState gridState = new GridState();
                gridState.SetFloorAtPosition(new Position(j, i), floorPrefab);
                grid.Add(gridState);
                gridPositions.Add(new Position(j, i));
            }
        }

        foreach (GridState gridState in grid)
        {
            gridState.EvaluateProbabilities(deviationProbs, gridPositions);
        }
    }

    void Update()
    {
        
    }
}
