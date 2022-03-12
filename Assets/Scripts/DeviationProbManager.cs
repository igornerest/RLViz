using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeviationProbManager : MonoBehaviour
{
    [SerializeField] private Slider forwardSlider;
    [SerializeField] private Slider rightSlider;
    [SerializeField] private Slider backSlider;
    [SerializeField] private Slider leftSlider;

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
        float updatedValue = Mathf.Min(value, remainingProb);
        slider.value = updatedValue;

        return updatedValue;
    }
}
