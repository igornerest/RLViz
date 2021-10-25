using System.Collections.Generic;
using UnityEngine;

public class RLManager : MonoBehaviour
{
    [SerializeField] private Transform floorPrefab;

    private MDP mdp = new MDP();

    private Dictionary<Deviation, float> deviationProbs =
        new Dictionary<Deviation, float> {    
            { Deviation.FORWARD,    0.8f },
            { Deviation.LEFT,       0.1f },
            { Deviation.RIGHT,      0.1f }
        };

    private void Start()
    {
        setMDP();

        foreach (State state in mdp.getAllStates())
        {
            GridBlock gridBlock = Instantiate(floorPrefab, state.Position, Quaternion.identity).GetComponent<GridBlock>();
            gridBlock.UpdateBlock(state);
        }

        //RLAlgorithms.valueIteration(mdp, gamma: 0.9f, episilon: 0.001f);
        RLAlgorithms.policyIteration(mdp, gamma: 0.9f);
        RLAlgorithms.TimeDifference(mdp, gamma: 0.9f, alpha: 0.1f);
    }


    private void setMDP()
    {
        int rows = 3;
        int columns = 4;

        for (int i = 1; i <= rows; i++)
        {
            for (int j = 1; j <= columns; j++)
            {
                if ((j, i) == (2, 2))
                    continue;

                bool isInitial = ((j, i) == (1, 1));
                bool isTerminal = ((j, i) == (4, 3) || (j, i) == (4, 2));
                float terminalReward = (j, i) == (4, 3) ? 1f : -1f;
                float reward = isTerminal ? terminalReward : -0.04f;

                State state = new State(j, i, reward, isTerminal);
                mdp.AddState(state, isInitial);
            }
        }

        mdp.EvaluateProbabilities(deviationProbs);
    }
}
