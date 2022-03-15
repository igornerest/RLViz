using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DisplayMode
{
    Reward,
    Policy,
    Value
}

public class DisplayModeManager : MonoBehaviour
{
    [SerializeField] private Toggle rewardToggle;
    [SerializeField] private Toggle policyToggle;
    [SerializeField] private Toggle valueToggle;

    [SerializeField] private TMP_Text displayModeText;

    private Dictionary<Toggle, DisplayMode> toggleDictionary;

    private void Start()
    {
        toggleDictionary = new Dictionary<Toggle, DisplayMode>()
        {
            { rewardToggle, DisplayMode.Reward },
            { policyToggle, DisplayMode.Policy },
            { valueToggle, DisplayMode.Value },
        };

        UpdateTextLocalization();
    }

    // Used by LanguagePanel
    public void UpdateTextLocalization()
    {
        displayModeText.text = LocalizationLanguageManager.Localize("key_Display_mode");

        rewardToggle.GetComponentInChildren<Text>().text = LocalizationLanguageManager.Localize("key_rewards");
        policyToggle.GetComponentInChildren<Text>().text = LocalizationLanguageManager.Localize("key_policies");
        valueToggle.GetComponentInChildren<Text>().text = LocalizationLanguageManager.Localize("key_values");
    }

    public DisplayMode GetDisplayMode()
    {
        foreach (var (toggle, mode) in toggleDictionary.Select((x) => (x.Key, x.Value)))
        {
            if (toggle.isOn)
            {
                return mode;
            }
        }

        throw new Exception("No display mode selected");
    }
}
