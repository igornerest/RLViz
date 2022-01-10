using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InteractionMode
{
    Simulate,
    Delete,
    Create,
    Edit,
}

public class InteractionModeManager : MonoBehaviour
{

    [SerializeField] private Toggle simulateToggle;
    [SerializeField] private Toggle createToggle;
    [SerializeField] private Toggle deleteToggle;
    [SerializeField] private Toggle editToggle;

    [SerializeField] private TMP_InputField rewardInputField;
    [SerializeField] private TMP_Dropdown policyDropdown;
    [SerializeField] private Toggle terminalStateToggle;
    [SerializeField] private Toggle nonTerminalStateToggle;
    [SerializeField] private Toggle initialStateToggle;

    private Dictionary<Toggle, InteractionMode> interactionToggleDictionary;

    private void Start()
    {
        interactionToggleDictionary = new Dictionary<Toggle, InteractionMode>()
        {
            { simulateToggle, InteractionMode.Simulate },
            { deleteToggle, InteractionMode.Delete },
            { createToggle, InteractionMode.Create },
            { editToggle, InteractionMode.Delete }
        };
    }

    private void Update()
    {
        UpdateStateUIInteraction();
    }

    private void UpdateStateUIInteraction()
    {
        var interactionMode = GetInteractionMode();

        if (interactionMode == InteractionMode.Simulate || interactionMode == InteractionMode.Delete)
        {
            rewardInputField.interactable = false;
            policyDropdown.interactable = false;
            terminalStateToggle.interactable = false;
            nonTerminalStateToggle.interactable = false;
            initialStateToggle.interactable = false;
        }
        else
        {
            rewardInputField.interactable = true;
            policyDropdown.interactable = true;
            terminalStateToggle.interactable = true;
            nonTerminalStateToggle.interactable = true;
            initialStateToggle.interactable = true;
        }
    }

    public InteractionMode GetInteractionMode()
    {
        foreach (var (toggle, mode) in interactionToggleDictionary.Select((x) => (x.Key, x.Value)))
        {
            if (toggle.isOn)
            {
                return mode;
            }
        }

        throw new Exception("No interaction mode selected");
    }

    public State GetState()
    {
        // TODO
        return null;
    }

    public void UpdateState(State state)
    {
        // TODO
    }
}
