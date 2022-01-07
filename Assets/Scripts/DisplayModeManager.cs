using System;
using System.Collections.Generic;
using System.Linq;
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

    private Dictionary<Toggle, DisplayMode> toggleDictionary;

    private void Start()
    {
        toggleDictionary = new Dictionary<Toggle, DisplayMode>()
        {
            { rewardToggle, DisplayMode.Reward },
            { policyToggle, DisplayMode.Policy },
            { valueToggle, DisplayMode.Value },
        };
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
