using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Button backwardsButton;
    [SerializeField] private Button resetStopButton;

    private TMPro.TMP_Text resetStopButtonText;

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
        resetStopButtonText = resetStopButton.GetComponentInChildren<TMPro.TMP_Text>();

        algorithmDropdown.ClearOptions();
        algorithmDropdown.AddOptions( supportedAlgorithms
            .Select(value => LocalizationLanguageManager.GetLocalizedName("UI Text", value))
            .ToList()
       );
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
            if (MDPManager.Instance.AlgorithmState.HasFinishedIterations)
            {
                StopAndResetCurrAlgorithmCoroutine();
                SetUIToIdleState();
            }
        }
    }

    private Func<MDP, RLAlgorithmState, IEnumerator> GetSelectedAlgorithm()
    {
        string selectedAlgorithm = supportedAlgorithms[algorithmDropdown.value];
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

    private void SetUIToIdleState()
    {
        playButton.interactable = true;
        backwardsButton.interactable = true;
        resetStopButton.tag = "Reset";
        resetStopButtonText.text = LocalizationLanguageManager.GetLocalizedName("UI Text", "key_Reset");

        iterationSlider.UpdateSliderInteraction(true);

        algorithmDropdown.interactable = true;
    }

    private void SetUIToActiveState()
    {
        playButton.interactable = false;
        backwardsButton.interactable = false;
        resetStopButton.tag = "Stop";
        resetStopButtonText.text = LocalizationLanguageManager.GetLocalizedName("UI Text", "key_Stop");

        algorithmDropdown.interactable = false;

        iterationSlider.UpdateSliderInteraction(false);
    }

    public void UpdateAlgorithmPanelLocalization()
    {
        string resetStopButtonKey = resetStopButton.CompareTag("Reset") ? "key_Reset" : "key_Stop";
        resetStopButtonText.text = LocalizationLanguageManager.GetLocalizedName("UI Text", resetStopButtonKey);

        int previousSelectedValue = algorithmDropdown.value;
        algorithmDropdown.ClearOptions();
        algorithmDropdown.AddOptions(supportedAlgorithms
            .Select(value => LocalizationLanguageManager.GetLocalizedName("UI Text", value))
            .ToList()
       );
        algorithmDropdown.value = previousSelectedValue;
    }

    public void OnClickPlayButton()
    {
        SetUIToActiveState();

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

    public void OnClickBackwardsButton()
    {
        SetUIToActiveState();

        MDPManager.Instance.AlgorithmState.MaxIt = (int)iterationSlider.getValue();
        MDPManager.Instance.AlgorithmState.IsRunning = true;

        if (MDPManager.Instance.Mdp.IsUsingVFunction)
        {
            currAlgorithmCoroutine = RLAlgorithms.RevertVFunctionStates(
                MDPManager.Instance.Mdp,
                MDPManager.Instance.AlgorithmState
            );
        }
        else
        {
            currAlgorithmCoroutine = RLAlgorithms.RevertQFunctionStates(
                MDPManager.Instance.Mdp,
                MDPManager.Instance.AlgorithmState
            );
        }

        StartCoroutine(currAlgorithmCoroutine);
    }

    public void OnClickStopResetButton()
    {
        if (resetStopButton.CompareTag("Reset"))
        {
            StopAndResetCurrAlgorithmCoroutine();

            MDPManager.Instance.AlgorithmState.Reset((int)iterationSlider.getValue());
            MDPManager.Instance.Mdp.Reset();
        }
        else if (resetStopButton.CompareTag("Stop"))
        {
            SetUIToIdleState();
            StopCoroutine(currAlgorithmCoroutine);

            MDPManager.Instance.AlgorithmState.IsRunning = false;
        }
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
