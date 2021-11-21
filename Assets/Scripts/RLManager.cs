using System.Collections.Generic;
using UnityEngine;

public class RLManager : MonoBehaviour
{
    [SerializeField] private Transform floorPrefab;
    [SerializeField] private TMPro.TMP_Dropdown algorithmDropdown;

    private List<string> supportedAlgorithms = new List<string>
    {
        RLAlgorithms.ALGORITHM_POLICY_ITERATION,
        RLAlgorithms.ALGORITHM_VALUE_ITERATION,
    };
    
    private MDP mdp = new MDP();

    private Dictionary<Deviation, float> deviationProbs =
        new Dictionary<Deviation, float> {    
            { Deviation.FORWARD,    0.8f },
            { Deviation.LEFT,       0.1f },
            { Deviation.RIGHT,      0.1f }
        };


    private void Start()
    {
        algorithmDropdown.ClearOptions();
        algorithmDropdown.AddOptions(supportedAlgorithms);

        algorithmDropdown.onValueChanged.AddListener(delegate {
            SetAlgorithmOnDropdownValueChange(algorithmDropdown);
        });

        SetMDP();
        
        foreach (State state in mdp.getAllStates())
        {
            GridBlock gridBlock = Instantiate(floorPrefab, state.Position, Quaternion.identity).GetComponent<GridBlock>();
            gridBlock.UpdateBlock(state, mdp);
        }

        RLAlgorithms.valueIteration(mdp, gamma: 0.9f, episilon: 0.001f);
    }

    private void SetAlgorithmOnDropdownValueChange(TMPro.TMP_Dropdown change)
    {
        mdp.Reset();

        string selectedAlgorithm = supportedAlgorithms[change.value];
        switch (selectedAlgorithm)
        {
            case RLAlgorithms.ALGORITHM_VALUE_ITERATION:
                RLAlgorithms.valueIteration(mdp, gamma: 0.9f, episilon: 0.001f);
                return;

            case RLAlgorithms.ALGORITHM_POLICY_ITERATION:
                RLAlgorithms.policyIteration(mdp, gamma: 0.9f);
                return;

            case RLAlgorithms.ALGORITHM_TIME_DIFFERENCE:
                RLAlgorithms.TimeDifference(mdp, gamma: 0.9f, alpha: 0.1f);
                return;

            case RLAlgorithms.ALGORITHM_Q_LEARNING:
                RLAlgorithms.QLearning(mdp, gamma: 0.9f, alpha: 0.1f);
                return;
        }
    }

    private void SetMDP()
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
