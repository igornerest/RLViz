using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RLManager : MonoBehaviour
{
    [SerializeField] private Transform gridBlockPrefab;

    [SerializeField] private Canvas canvas;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private TMPro.TMP_Dropdown algorithmDropdown;
    [SerializeField] private Button playButton;
    [SerializeField] private Button modeButton;

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

    private bool isBuildMode = false;

    private void Start()
    {
        algorithmDropdown.ClearOptions();
        algorithmDropdown.AddOptions(supportedAlgorithms.Select(kvp => kvp.Key).ToList());
        OnAlgorithmDropdownValueChanged();

        SetMDP();
        
        foreach (State state in mdp.GetAllStates())
        {
            GridBlock gridBlock = Instantiate(gridBlockPrefab, state.Position, Quaternion.identity).GetComponent<GridBlock>();
            gridBlock.UpdateBlock(state, mdp);
        }
    }

    private void Update()
    {
        HandleBlockCreation();
    }

    private bool IsMousePointerOverUIPanel()
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        canvas.GetComponent<GraphicRaycaster>().Raycast(pointerEventData, results);

        return results.Count > 0;
    }

    private void HandleBlockCreation()
    {
        if (isBuildMode && Input.GetMouseButtonDown(0))
        {
            if (IsMousePointerOverUIPanel())
                return;
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "BaseFloor")
                {
                    int xPos = (int) hit.point.x;
                    int yPos = (int) hit.point.z;

                    State state = new State(xPos, yPos, 0f, false);
                    mdp.AddState(state, false);

                    GridBlock gridBlock = Instantiate(gridBlockPrefab, state.Position, Quaternion.identity).GetComponent<GridBlock>();
                    gridBlock.UpdateBlock(state, mdp);
                }
                else if (hit.transform.tag == "GridBlock")
                {
                    Destroy(hit.transform.gameObject);
                }
            }

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

    public void OnClickModeButton()
    {
        var modeButtonText = modeButton.GetComponentInChildren<TMPro.TMP_Text>();
        if (modeButtonText.text == "Build")
        {
            isBuildMode = true;
            modeButtonText.text = "Simulate";
        }
        else if (modeButtonText.text == "Simulate")
        {
            isBuildMode = false;
            modeButtonText.text = "Build";
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

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if ((j, i) == (1, 1))
                    continue;

                bool isInitial = ((j, i) == (0, 0));
                bool isTerminal = ((j, i) == (3, 2) || (j, i) == (3, 1));
                float terminalReward = (j, i) == (3, 2) ? 1f : -1f;
                float reward = isTerminal ? terminalReward : -0.04f;

                State state = new State(j, i, reward, isTerminal);
                mdp.AddState(state, isInitial);
            }
        }

        mdp.EvaluateProbabilities(deviationProbs);
    }
}
