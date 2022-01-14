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
    [SerializeField] private ToggleGroup interactionToggleGroup;
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

    private InteractionMode currInteractionMode;

    private void Start()
    {
        interactionToggleDictionary = new Dictionary<Toggle, InteractionMode>()
        {
            { simulateToggle, InteractionMode.Simulate },
            { deleteToggle, InteractionMode.Delete },
            { createToggle, InteractionMode.Create },
            { editToggle, InteractionMode.Edit }
        };

        ResetPanel();
    }

    private void Update()
    {
        UpdateStateUIInteraction();
    }

    private void EnableInteraction()
    {
        rewardInputField.interactable = true;
        policyDropdown.interactable = true;
        terminalStateToggle.interactable = true;
        nonTerminalStateToggle.interactable = true;
        initialStateToggle.interactable = true;
    }

    private void DisableInteraction()
    {
        rewardInputField.interactable = false;
        policyDropdown.interactable = false;
        terminalStateToggle.interactable = false;
        nonTerminalStateToggle.interactable = false;
        initialStateToggle.interactable = false;
    }

    private void SetupDropdownOptions()
    {
        policyDropdown.ClearOptions();
        policyDropdown.AddOptions(ActionExtensions.GetValidActionStrings());
    }

    private void EmptyDropdownOptions()
    {
        policyDropdown.ClearOptions();
        policyDropdown.AddOptions(new List<string>() { "No policy" });
    }

    private void ResetPanel()
    {
        DisableInteraction();

        EmptyDropdownOptions();

        rewardInputField.text = "No Reward";
        policyDropdown.value = -1;

        interactionToggleGroup.allowSwitchOff = true;
        interactionToggleGroup.SetAllTogglesOff();
    }

    private void UpdateStateUIInteraction()
    {
        var interactionMode = GetInteractionMode();

        if (currInteractionMode != interactionMode)
        {
            currInteractionMode = interactionMode;

            if (currInteractionMode == InteractionMode.Create)
            {
                SetDefaultPanel();
                EnableInteraction();
            }
            else
            { 
                // Simulate and Delete modes will be disabled all the time
                // Edit mode will be disabled in the beginning, until a gridblock is selected
                ResetPanel();
                DisableInteraction();
            }
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

    public void SetDefaultPanel()
    {
        SetupDropdownOptions();

        // TODO: fetch those data from a defined data structure 
        rewardInputField.text = "-0.04";
        policyDropdown.value = policyDropdown.options.FindIndex(option => option.text == "Up");

        interactionToggleGroup.allowSwitchOff = false;
        nonTerminalStateToggle.isOn = true;

        EnableInteraction();
    }

    public void UpdatePanelWithStateInfo(State state, MDP mdp)
    {
        SetupDropdownOptions();

        rewardInputField.text = state.Reward.ToString("0.00"); ;

        Action actualPolicy = mdp.Policy[state];
        string policyStr = ActionExtensions.GetStringFromAction(actualPolicy);
        policyDropdown.value = policyDropdown.options.FindIndex(option => option.text == policyStr);

        interactionToggleGroup.allowSwitchOff = false;
        if (mdp.InitialState == state)
        {
            initialStateToggle.isOn = true;
        }
        else if (state.IsTerminal)
        {
            terminalStateToggle.isOn = true;
        }
        else
        {
            nonTerminalStateToggle.isOn = true;
        }

        EnableInteraction();
    }

    public void UpdateState(State state, MDP mdp)
    {
        state.Reward = float.Parse(rewardInputField.text, System.Globalization.CultureInfo.InvariantCulture);

        string policyString = policyDropdown.options[policyDropdown.value].text;
        Action actualPolicy = ActionExtensions.GetActionFromString(policyString);
        mdp.Policy[state] = actualPolicy;

        if (initialStateToggle.isOn)
        {
            mdp.InitialState = state;
            state.IsTerminal = false;
        }
        else
        {
            if (mdp.InitialState == state)
            {
                mdp.InitialState = null;
            }

            state.IsTerminal = terminalStateToggle.isOn;
        }
    }
}
