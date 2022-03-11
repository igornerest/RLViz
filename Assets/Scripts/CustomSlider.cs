using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] private string componentName;
    [SerializeField] private float defaultValue;

    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text sliderText;

    private void Start()
    {
        slider.value = defaultValue;
    }

    private void Update()
    {
        float value = slider.value;
        string formattedValue = slider.wholeNumbers ? value.ToString() : value.ToString("0.00");
        string sliderTextPrefix = "";
        if (componentName != "")
        {
            string localizedComponentName = LocalizationLanguageManager.GetLocalizedName("UI Text", componentName);
            sliderTextPrefix = string.Format("{0}: ", localizedComponentName);
        }
        sliderText.text = sliderTextPrefix + formattedValue;
    }

    public void UpdateSliderInteraction(bool isInteractable)
    {
        slider.interactable = isInteractable;
    }

    public float getValue()
    {
        return (float) Math.Round(slider.value, 2);
    }
}
