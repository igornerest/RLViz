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
            if (!MDPManager.Instance.AlgorithmState.HasFinishedIterations())
            {
                StopAndResetCurrAlgorithmCoroutine();
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

    private void StopAndResetCurrAlgorithmCoroutine()
    {
        if (currAlgorithmCoroutine != null)
        {
            StopCoroutine(currAlgorithmCoroutine);
            currAlgorithmCoroutine = null;

            MDPManager.Instance.AlgorithmState.IsRunning = false;
        }
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

            // We dont reset the state to keep iteration count
            MDPManager.Instance.AlgorithmState.MaxIt = (int)iterationSlider.getValue();
            MDPManager.Instance.AlgorithmState.IsRunning = true;

            if (currAlgorithmCoroutine == null)
            {
                currAlgorithmCoroutine = currAlgorithmFunc(
                    MDPManager.Instance.Mdp,
                    MDPManager.Instance.AlgorithmState
                );
            }

            StartCoroutine(currAlgorithmCoroutine);
        }
        else if (playButtonText.text == "STOP")
        {
            EnabledUIInteraction();

            StopCoroutine(currAlgorithmCoroutine);

            MDPManager.Instance.AlgorithmState.IsRunning = false;
        }
    }

    public void OnClickResetButton()
    {
        StopAndResetCurrAlgorithmCoroutine();
        MDPManager.Instance.Mdp.Reset();
    }

    public void OnAlgorithmDropdownValueChanged()
    {
        StopAndResetCurrAlgorithmCoroutine();

        MDPManager.Instance.AlgorithmState.Reset((int)iterationSlider.getValue());
        MDPManager.Instance.Mdp.Reset();

        currAlgorithmFunc = GetSelectedAlgorithm();
    }

    public void OnIterationSliderValueChanged()
    {
        if (!IsCurrAlgorithmCoroutineActive())
        {
            MDPManager.Instance.AlgorithmState.MaxIt = (int)iterationSlider.getValue();
        }
    }
}
