using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeviationProbManager : MonoBehaviour
{
    [SerializeField] private Slider forwardSlider;
    [SerializeField] private Slider rightSlider;
    [SerializeField] private Slider backSlider;
    [SerializeField] private Slider leftSlider;

    [SerializeField] private TMP_Text deviationProbText;

    private float stepSize = 0.05f;

    private void Start()
    {
        UpdateTextLocalization();
    }

    // Used by LanguagePanel
    public void UpdateTextLocalization()
    {
        deviationProbText.text = LocalizationLanguageManager.Localize("key_Probabilities");
    }

    public void EnableSliders()
    {
        forwardSlider.interactable = true;
        rightSlider.interactable = true;
        backSlider.interactable = true;
        leftSlider.interactable = true;
    }

    public void DisableSliders()
    {
        forwardSlider.interactable = false;
        rightSlider.interactable = false;
        backSlider.interactable = false;
        leftSlider.interactable = false;
    }

    public void OnForwardSliderValueChanged(float value)
    {
        float updatedValue = LimitProbSliderValue(forwardSlider, Deviation.FORWARD, value);
        UpdateMDPProb(Deviation.FORWARD, updatedValue);
    }

    public void OnRightSliderValueChanged(float value)
    {
        float updatedValue = LimitProbSliderValue(rightSlider, Deviation.RIGHT, value);
        UpdateMDPProb(Deviation.RIGHT, updatedValue);
    }

    public void OnBackSliderValueChanged(float value)
    {
        float updatedValue = LimitProbSliderValue(backSlider, Deviation.BACK, value);
        UpdateMDPProb(Deviation.BACK, updatedValue);
    }

    public void OnLeftSliderValueChanged(float value)
    {
        float updatedValue = LimitProbSliderValue(leftSlider, Deviation.LEFT, value);
        UpdateMDPProb(Deviation.LEFT, updatedValue);
    }

    private void UpdateMDPProb(Deviation deviation, float value)
    {
        MDPManager.Instance.Mdp.UpdateDeviationProb(deviation, value);
    }

    private float LimitProbSliderValue(Slider slider, Deviation currDeviation, float value)
    {
        float probSum = MDPManager.Instance.Mdp.DeviationProbs
            .Where(x => x.Key != currDeviation)
            .Sum(x => x.Value);

        float remainingProb = 1 - probSum;
        float updatedProbability = Mathf.Min(value * stepSize, remainingProb);
        slider.value = updatedProbability / stepSize;

        return updatedProbability;
    }
}
