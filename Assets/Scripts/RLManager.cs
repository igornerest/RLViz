using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RLManager : MonoBehaviour
{
    [SerializeField] private Transform floorPrefab;

    [SerializeField] private TMPro.TMP_Dropdown algorithmDropdown;
    [SerializeField] private Button playButton;

    private Dictionary<string, Func<MDP, IEnumerator>> supportedAlgorithms = 
        new Dictionary<string, Func<MDP, IEnumerator>>()
        {
            { RLAlgorithms.ALGORITHM_VALUE_ITERATION, RLAlgorithms.ValueIteration },
            { RLAlgorithms.ALGORITHM_POLICY_ITERATION, RLAlgorithms.PolicyIteration },
            { RLAlgorithms.ALGORITHM_Q_LEARNING, RLAlgorithms.QLearning },
            { RLAlgorithms.ALGORITHM_SARSA, RLAlgorithms.Sarsa }
        };

    private IEnumerator currAlgorithm;

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
        algorithmDropdown.AddOptions(supportedAlgorithms.Select(kvp => kvp.Key).ToList());
        OnAlgorithmDropdownValueChanged();

        SetMDP();
        
        foreach (State state in mdp.GetAllStates())
        {
            GridBlock gridBlock = Instantiate(floorPrefab, state.Position, Quaternion.identity).GetComponent<GridBlock>();
            gridBlock.UpdateBlock(state, mdp);
        }
    }

    public void OnClickPlayButton()
    {
        if (currAlgorithm == null)
        {
            return;
        }

        var playButtonText = playButton.GetComponentInChildren<TMPro.TMP_Text>();
        if (playButtonText.text == "Play")
        {
            StartCoroutine(currAlgorithm);
            playButtonText.text = "Stop";
        }
        else if (playButtonText.text == "Stop")
        {
            StopCoroutine(currAlgorithm);
            playButtonText.text = "Play";
        }
    }

    public void OnAlgorithmDropdownValueChanged()
    {
        if (currAlgorithm != null)
        {
            StopCoroutine(currAlgorithm);
        }

        string selectedAlgorithm = algorithmDropdown.options[algorithmDropdown.value].text;
        Debug.Log("Selected algorithm: " + selectedAlgorithm);
        currAlgorithm = supportedAlgorithms[selectedAlgorithm](mdp);

        mdp.Reset();

        playButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Play";
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
