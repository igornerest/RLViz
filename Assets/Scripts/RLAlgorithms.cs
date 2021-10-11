using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RLAlgorithms
{
    public static void valueIteration(Dictionary<Position, GridState> grid, float gamma, float episilon)
    {
        float delta = episilon * (1 - gamma) / gamma + 1;

        Dictionary<Position, float> uDict = new Dictionary<Position, float>();
        foreach (Position pos in grid.Keys)
        {
            uDict[pos] = 0f;
        }

        while (delta > episilon * (1 - gamma) / gamma)
        {
            Dictionary<Position, float> newUDict = new Dictionary<Position, float>(uDict);
            delta = 0;
            foreach (GridState gridState in grid.Values)
            {
                if (gridState.IsTerminal)
                {
                    uDict[gridState.Position] = gridState.Reward;
                }
                else
                {
                    float maxUtility = gridState.GetExpectedValuesFromUtility(newUDict).Max();
                    uDict[gridState.Position] = gridState.Reward + gamma * maxUtility;
                }

                delta = Math.Max(delta, Math.Abs(uDict[gridState.Position] - newUDict[gridState.Position]));
            }
            Debug.Log("New Iteration");
        }

        UpdateGridStateUtilities(grid, uDict);

        foreach (KeyValuePair<Position, float> utility in uDict)
        {
            Debug.Log(String.Format("Position: {0}, Utility {1}", utility.Key, utility.Value));
        }
        
    }

    private static void UpdateGridStateUtilities(Dictionary<Position, GridState> grid, Dictionary<Position, float> uDict) { 
        foreach (var u in uDict)
        {
           grid[u.Key].UpdateUtility(u.Value);
        }
    }
}
