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

    [SerializeField] private CustomSlider gammaSlider;      // 0.9 by default
    [SerializeField] private CustomSlider epsilonSlider;    // 0.2 by default
    [SerializeField] private CustomSlider alphaSlider;      // 0.1 by default
    [SerializeField] private CustomSlider timeSlider;       // 1000ms by default
    [SerializeField] private CustomSlider iterationSlider;  // 20 by default

    [SerializeField] private TMPro.TMP_Dropdown algorithmDropdown;

    [SerializeField] private Button playButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button modeButton;

    private Dictionary<string, Func<MDP, RLAlgorithmState, IEnumerator>> supportedAlgorithms = 
        new Dictionary<string, Func<MDP, RLAlgorithmState, IEnumerator>>()
        {
            { RLAlgorithms.ALGORITHM_VALUE_ITERATION, RLAlgorithms.ValueIteration },
            { RLAlgorithms.ALGORITHM_POLICY_ITERATION, RLAlgorithms.PolicyIteration },
            { RLAlgorithms.ALGORITHM_Q_LEARNING, RLAlgorithms.QLearning },
            { RLAlgorithms.ALGORITHM_SARSA, RLAlgorithms.Sarsa }
        };

    private IEnumerator currAlgorithmCoroutine;
    private RLAlgorithmState currAlgorithmState;

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
        mdp.Gamma = gammaSlider.getValue();
        mdp.Epsilon = epsilonSlider.getValue();
        mdp.Alpha = alphaSlider.getValue();

        Time.fixedDeltaTime = timeSlider.getValue()/1000;

        HandleAlgorithmStateUpdate();
        HandleBlockCreation();
    }

    public void HandleAlgorithmStateUpdate()
    {
        if (IsCurrAlgorithmCoroutineActive())
        {
            if (!currAlgorithmState.IsActive())
            {
                StopCurrAlgorithmCoroutine();
                EnabledUIInteraction();
            }
        }
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

    private Func<MDP, RLAlgorithmState, IEnumerator> GetSelectedAlgorithm()
    {
        string selectedAlgorithm = algorithmDropdown.options[algorithmDropdown.value].text;
        Debug.Log(selectedAlgorithm);
        return supportedAlgorithms[selectedAlgorithm];
    }

    private bool IsCurrAlgorithmCoroutineActive()
    {
        return currAlgorithmCoroutine != null;
    }

    private void StopCurrAlgorithmCoroutine()
    {
        StopCoroutine(currAlgorithmCoroutine);
        currAlgorithmCoroutine = null;
    }

    private void EnabledUIInteraction()
    {
        playButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Play";
        iterationSlider.UpdateSliderInteraction(true);
        resetButton.interactable = true;
        modeButton.interactable = true;
        algorithmDropdown.interactable = true;
    }

    private void DisabledUIInteraction()
    {
        playButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Stop";
        iterationSlider.UpdateSliderInteraction(false);
        resetButton.interactable = false;
        modeButton.interactable = false;
        algorithmDropdown.interactable = false;
    }

    public void OnClickPlayButton()
    {
        var playButtonText = playButton.GetComponentInChildren<TMPro.TMP_Text>();

        if (playButtonText.text == "Play")
        {
            DisabledUIInteraction();

            // We reuse the instance to keep iteration count
            // A new instance is created whenever the algorithm changes
            currAlgorithmState.MaxIt = (int)iterationSlider.getValue();

            currAlgorithmCoroutine = GetSelectedAlgorithm()(mdp, currAlgorithmState);
            StartCoroutine(currAlgorithmCoroutine);
        }
        else if (playButtonText.text == "Stop")
        {
            EnabledUIInteraction();

            StopCurrAlgorithmCoroutine();
        }
    }

    public void OnClickResetButton()
    {
        if (!IsCurrAlgorithmCoroutineActive())
        {
            mdp.Reset();
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
        if (!IsCurrAlgorithmCoroutineActive())
        {
            // We use a new instance of algorithm state since the algorithm changes
            currAlgorithmState = new RLAlgorithmState((int)iterationSlider.getValue());

            mdp.Reset();
        }
    }

    public void OnIterationSliderValueChanged()
    {
        if (!IsCurrAlgorithmCoroutineActive())
        {
            currAlgorithmState.MaxIt = (int)iterationSlider.getValue();
        }
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
