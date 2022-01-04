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
        UpdateText(defaultValue);

        slider.onValueChanged.AddListener((val) => {
            UpdateText(val);
        });
    }

    private void UpdateText(float val)
    {
        sliderText.text = string.Format("{0}: {1}", componentName, slider.value.ToString("0.00"));
    }

    public float getValue()
    {
        return (float) Math.Round(slider.value, 2);
    }

}
