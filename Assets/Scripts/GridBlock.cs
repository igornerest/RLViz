using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridBlock : MonoBehaviour
{
    public DisplayModeManager displayModeManager;

    [SerializeField] private Canvas rewardCanvas;
    [SerializeField] private TMP_Text rewardText;

    [SerializeField] private Canvas policyCanvas;
    [SerializeField] private TMP_Text policyText;

    [SerializeField] private Canvas vFunctionCanvas;
    [SerializeField] private TMP_Text utilityText;

    [SerializeField] private Canvas qFunctionCanvas;
    [SerializeField] private TMP_Text upQValueText;
    [SerializeField] private TMP_Text downQValueText;
    [SerializeField] private TMP_Text leftQValueText;
    [SerializeField] private TMP_Text rightQValueText;

    private Dictionary<DisplayMode, System.Action> canvasUpdateDictionary;

    private State state;
    private MDP mdp;

    private int decimalPlaces = 2;

    private void Start()
    {
        canvasUpdateDictionary = new Dictionary<DisplayMode, System.Action>()
        {
            { DisplayMode.Reward, DisplayRewardCanvas },
            { DisplayMode.Policy, DisplayPolicyCanvas },
            { DisplayMode.Value, DisplayValueCanvas },
        };

    }

    private void Update()
    {
        UpdateGridBlockCanvas();
    }

    public void UpdateBlock(State state, MDP mdp)
    {
        this.state = state;
        this.mdp = mdp;
        UpdateGridBlockCanvas();
    }

    private void UpdateGridBlockCanvas()
    {
        if (displayModeManager != null && canvasUpdateDictionary != null)
        {
            var displayMode = displayModeManager.GetDisplayMode();
            canvasUpdateDictionary[displayMode]();
        }
    }

    private void DisplayRewardCanvas()
    {
        policyCanvas.enabled = false;
        vFunctionCanvas.enabled = false;
        qFunctionCanvas.enabled = false;

        rewardText.text = state.Reward.ToString();

        rewardCanvas.enabled = true;
    }

    private void DisplayPolicyCanvas()
    {
        rewardCanvas.enabled = false;
        vFunctionCanvas.enabled = false;
        qFunctionCanvas.enabled = false;

        policyText.text = mdp.Policy[state].ToString();

        policyCanvas.enabled = true;
    }

    private void DisplayValueCanvas()
    {
        rewardCanvas.enabled = false;
        policyCanvas.enabled = false;

        if (mdp.IsUsingVFunction)
        {
            qFunctionCanvas.enabled = false;

            utilityText.text = ToRoundedString(mdp.Utility[state]);
            vFunctionCanvas.enabled = true;
        }
        else
        {
            vFunctionCanvas.enabled = false;

            upQValueText.text = ToRoundedString(mdp.QFunction[state, Action.UP]);
            downQValueText.text = ToRoundedString(mdp.QFunction[state, Action.DOWN]);
            leftQValueText.text = ToRoundedString(mdp.QFunction[state, Action.LEFT]);
            rightQValueText.text = ToRoundedString(mdp.QFunction[state, Action.RIGHT]);
            qFunctionCanvas.enabled = true;
        }
    }

    private String ToRoundedString(float number)
    {
        return Math.Round(number, decimalPlaces).ToString();
    }
}
