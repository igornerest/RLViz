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

    [SerializeField] private TMP_Text interactionModeText;

    private Dictionary<Toggle, InteractionMode> interactionToggleDictionary;

    private InteractionMode currInteractionMode;

    private List<string> dropdownOptionKeys;

    private void Start()
    {
        interactionModeText.text = LocalizationLanguageManager.Localize("key_Interaction_mode");

        interactionToggleDictionary = new Dictionary<Toggle, InteractionMode>()
        {
            { simulateToggle, InteractionMode.Simulate },
            { deleteToggle, InteractionMode.Delete },
            { createToggle, InteractionMode.Create },
            { editToggle, InteractionMode.Edit }
        };

        ResetPanel();
        UpdateTextLocalization();
    }

    private void Update()
    {
        UpdateStateUIInteraction();
        GetPolicyActionFromDropdown();
    }

    public void EnableToggles()
    {
        simulateToggle.interactable = true;
        createToggle.interactable = true;
        deleteToggle.interactable = true;
        editToggle.interactable = true;
    }

    public void DisableToggles()
    {
        simulateToggle.interactable = false;
        createToggle.interactable = false;
        deleteToggle.interactable = false;
        editToggle.interactable = false;
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
        dropdownOptionKeys = ActionExtensions.GetValidActionStringKeys();
        UpdateDropdownTextLocalization(resetSelectedValue: true);
    }

    private void UpdateDropdownTextLocalization(bool resetSelectedValue = false)
    {
        int previousSelectedValue = policyDropdown.value;
        policyDropdown.ClearOptions();
        policyDropdown.AddOptions(dropdownOptionKeys.Select(key => LocalizationLanguageManager.Localize(key)).ToList());

        if (resetSelectedValue == false)
        {
            policyDropdown.value = previousSelectedValue;
        }
    }

    // Used by LanguagePanel
    public void UpdateTextLocalization()
    {
        interactionModeText.text = LocalizationLanguageManager.Localize("key_Interaction_mode");

        simulateToggle.GetComponentInChildren<Text>().text = LocalizationLanguageManager.Localize("key_simulate");
        createToggle.GetComponentInChildren<Text>().text = LocalizationLanguageManager.Localize("key_create");
        deleteToggle.GetComponentInChildren<Text>().text = LocalizationLanguageManager.Localize("key_delete");
        editToggle.GetComponentInChildren<Text>().text = LocalizationLanguageManager.Localize("key_edit");

        terminalStateToggle.GetComponentInChildren<Text>().text = LocalizationLanguageManager.Localize("key_terminal_state");
        nonTerminalStateToggle.GetComponentInChildren<Text>().text = LocalizationLanguageManager.Localize("key_non_terminal_state");
        initialStateToggle.GetComponentInChildren<Text>().text = LocalizationLanguageManager.Localize("key_initial_state");

        if (!rewardInputField.IsInteractable())
        {
            rewardInputField.text = LocalizationLanguageManager.Localize("key_No_reward");
        }

        UpdateDropdownTextLocalization(resetSelectedValue: false);
    }

    private void EmptyDropdownOptions()
    {
        dropdownOptionKeys = new List<string>() { "key_No_policy" };
        UpdateDropdownTextLocalization(resetSelectedValue: true);
    }

    private void ResetPanel()
    {
        DisableInteraction();

        EmptyDropdownOptions();

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

    private Action GetPolicyActionFromDropdown()
    {
        string policyString = dropdownOptionKeys[policyDropdown.value];
        return ActionExtensions.GetActionFromStringKey(policyString);
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
        rewardInputField.text = (-0.04f).ToString("0.00");
        string localizedDefaultPolicy = LocalizationLanguageManager.Localize("key_Up");
        policyDropdown.value = policyDropdown.options.FindIndex(option => option.text == localizedDefaultPolicy);

        interactionToggleGroup.allowSwitchOff = false;
        nonTerminalStateToggle.isOn = true;

        EnableInteraction();
    }

    public void UpdatePanelWithStateInfo(State state, MDP mdp)
    {
        SetupDropdownOptions();

        rewardInputField.text = state.Reward.ToString("0.00");

        Action actualPolicy = mdp.Policy[state];
        string policyStr = ActionExtensions.GetStringKeyFromAction(actualPolicy);
        string localizedPolicyStr = LocalizationLanguageManager.Localize(policyStr);
        policyDropdown.value = policyDropdown.options.FindIndex(option => option.text == localizedPolicyStr);

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
        state.Reward = float.Parse(rewardInputField.text);

        Action actualPolicy = GetPolicyActionFromDropdown();
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
