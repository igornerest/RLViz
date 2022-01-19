using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RLManager : MonoBehaviour
{
    [SerializeField] private CustomSlider gammaSlider;      // 0.9 by default
    [SerializeField] private CustomSlider epsilonSlider;    // 0.2 by default
    [SerializeField] private CustomSlider alphaSlider;      // 0.1 by default
    [SerializeField] private CustomSlider timeSlider;       // 1000ms by default
    [SerializeField] private CustomSlider iterationSlider;  // 20 by default

    [SerializeField] private TMPro.TMP_Dropdown algorithmDropdown;

    [SerializeField] private Button playButton;
    [SerializeField] private Button resetButton;

    private List<string> supportedAlgorithms = new List<string>()
    {
        RLAlgorithms.ALGORITHM_VALUE_ITERATION,
        RLAlgorithms.ALGORITHM_POLICY_ITERATION,
        RLAlgorithms.ALGORITHM_Q_LEARNING,
        RLAlgorithms.ALGORITHM_SARSA,
    };

    private IEnumerator currAlgorithmCoroutine;
    private Func<MDP, RLAlgorithmState, IEnumerator> currAlgorithmFunc;
    private RLAlgorithmState currAlgorithmState;

    private void Start()
    {
        algorithmDropdown.ClearOptions();
        algorithmDropdown.AddOptions(supportedAlgorithms);
        OnAlgorithmDropdownValueChanged();
    }

    private void Update()
    {
        MDPManager.Instance.Mdp.Gamma = gammaSlider.getValue();
        MDPManager.Instance.Mdp.Epsilon = epsilonSlider.getValue();
        MDPManager.Instance.Mdp.Alpha = alphaSlider.getValue();

        Time.fixedDeltaTime = timeSlider.getValue()/1000;

        HandleAlgorithmStateUpdate();
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

    private Func<MDP, RLAlgorithmState, IEnumerator> GetSelectedAlgorithm()
    {
        string selectedAlgorithm = algorithmDropdown.options[algorithmDropdown.value].text;
        Debug.Log(selectedAlgorithm);

        switch (selectedAlgorithm)
        {
            case RLAlgorithms.ALGORITHM_VALUE_ITERATION:
                MDPManager.Instance.Mdp.UseVFunction();
                return RLAlgorithms.ValueIteration;

            case RLAlgorithms.ALGORITHM_POLICY_ITERATION:
                MDPManager.Instance.Mdp.UseVFunction();
                return RLAlgorithms.PolicyIteration;

            case RLAlgorithms.ALGORITHM_Q_LEARNING:
                MDPManager.Instance.Mdp.UseQFunction();
                return RLAlgorithms.QLearning;

            case RLAlgorithms.ALGORITHM_SARSA:
                MDPManager.Instance.Mdp.UseQFunction();
                return RLAlgorithms.Sarsa;
        }

        throw new Exception("No valid algorithm was selected");
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
        playButton.GetComponentInChildren<TMPro.TMP_Text>().text = "PLAY";
        iterationSlider.UpdateSliderInteraction(true);
        resetButton.interactable = true;
        algorithmDropdown.interactable = true;
    }

    private void DisabledUIInteraction()
    {
        playButton.GetComponentInChildren<TMPro.TMP_Text>().text = "STOP";
        iterationSlider.UpdateSliderInteraction(false);
        resetButton.interactable = false;
        algorithmDropdown.interactable = false;
    }

    public void OnClickPlayButton()
    {
        var playButtonText = playButton.GetComponentInChildren<TMPro.TMP_Text>();

        if (playButtonText.text == "PLAY")
        {
            DisabledUIInteraction();

            // We reuse the instance to keep iteration count
            // A new instance is created whenever the algorithm changes
            currAlgorithmState.MaxIt = (int)iterationSlider.getValue();

            currAlgorithmCoroutine = currAlgorithmFunc(MDPManager.Instance.Mdp, currAlgorithmState);
            StartCoroutine(currAlgorithmCoroutine);
        }
        else if (playButtonText.text == "STOP")
        {
            EnabledUIInteraction();

            StopCurrAlgorithmCoroutine();
        }
    }

    public void OnClickResetButton()
    {
        if (!IsCurrAlgorithmCoroutineActive())
        {
            MDPManager.Instance.Mdp.Reset();
        }
    }

    public void OnAlgorithmDropdownValueChanged()
    {
        if (!IsCurrAlgorithmCoroutineActive())
        {
            // We use a new instance of algorithm state since the algorithm changes
            currAlgorithmState = new RLAlgorithmState((int)iterationSlider.getValue());
            currAlgorithmFunc = GetSelectedAlgorithm();
            MDPManager.Instance.Mdp.Reset();
        }
    }

    public void OnIterationSliderValueChanged()
    {
        if (!IsCurrAlgorithmCoroutineActive())
        {
            currAlgorithmState.MaxIt = (int)iterationSlider.getValue();
        }
    }
}
