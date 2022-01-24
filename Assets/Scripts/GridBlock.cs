using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridBlock : MonoBehaviour
{
    public DisplayModeManager displayModeManager;

    [SerializeField] private GameObject rewardCanvas;
    [SerializeField] private TMP_Text rewardText;

    [SerializeField] private GameObject policyCanvas;
    [SerializeField] private List<SpriteRenderer> upArrows;
    [SerializeField] private List<SpriteRenderer> rightArrows;
    [SerializeField] private List<SpriteRenderer> downArrows;
    [SerializeField] private List<SpriteRenderer> leftArrows;

    [SerializeField] private GameObject vFunctionCanvas;
    [SerializeField] private TMP_Text utilityText;

    [SerializeField] private GameObject qFunctionCanvas;
    [SerializeField] private TMP_Text upQValueText;
    [SerializeField] private TMP_Text downQValueText;
    [SerializeField] private TMP_Text leftQValueText;
    [SerializeField] private TMP_Text rightQValueText;

    [SerializeField] private List<MeshRenderer> brickMeshRenderer;
    [SerializeField] private Material opaqueMaterial;
    [SerializeField] private Material transparentMaterial;

    [SerializeField] private GameObject startSign;
    [SerializeField] private GameObject stopSign;

    private Dictionary<DisplayMode, System.Action> canvasUpdateDictionary;

    private Dictionary<Action, List<SpriteRenderer>> arrowDictionary;

    private State state;
    private MDP mdp;

    private int decimalPlaces = 2;

    // Hierarchy: ghostBlock - hovered - selected
    public bool IsHoveredByUser { set; get; }
    public bool IsSelectedByUser { set; get; }
    public bool IsGhostBlock { set; get; }

    public State State { get => state; }

    private void Start()
    {
        canvasUpdateDictionary = new Dictionary<DisplayMode, System.Action>()
        {
            { DisplayMode.Reward, DisplayRewardCanvas },
            { DisplayMode.Policy, DisplayPolicyCanvas },
            { DisplayMode.Value, DisplayValueCanvas },
        };

        arrowDictionary = new Dictionary<Action, List<SpriteRenderer>>()
        {
            { Action.UP, upArrows },
            { Action.RIGHT, rightArrows },
            { Action.DOWN, downArrows },
            { Action.LEFT, leftArrows },
        };
    }

    private void Update()
    {
        UpdateGridBlockCanvas();
        UpdateGridBlockMaterial();
    }

    public void UpdateBlock(State state, MDP mdp)
    {
        this.state = state;
        this.mdp = mdp;
        UpdateGridBlockCanvas();
        UpdateGridBlockMaterial();
    }

    private void UpdateGridBlockMaterial()
    {
        if (IsHoveredByUser || IsGhostBlock)
        {
            UpdateTransparency(true);
        }
        else if (IsSelectedByUser)
        {
            UpdateFlickerEffect();
        }
        else if (mdp.AgentState == this.state)
        {
            UpdateTransparency(false);
            //gridBlockMeshRenderer.material = agentStateMaterial;
        }
        else if (mdp.InitialState == this.state)
        {
            UpdateTransparency(false);
            startSign.SetActive(true);
            stopSign.SetActive(false);
        }
        else if (state.IsTerminal)
        {
            UpdateTransparency(false);
            startSign.SetActive(false);
            stopSign.SetActive(true);
        }
        else
        {
            UpdateTransparency(false);
            startSign.SetActive(false);
            stopSign.SetActive(false);
        }
    }

    private void UpdateFlickerEffect()
    {
        foreach (var renderer in brickMeshRenderer)
        {
            var color = renderer.material.color;
            color.a = 0.3f + Mathf.PingPong(Time.time, 0.7f);
            renderer.material.color = color;
        }
    }

    private void UpdateTransparency(bool isTransparent)
    {
        foreach (var renderer in brickMeshRenderer)
        {
            var color = renderer.material.color;
            color.a = isTransparent ? 0.5f : 1f;

            renderer.material = isTransparent ? transparentMaterial : opaqueMaterial;
            renderer.material.color = color;
        }
    }

    private void UpdateGridBlockCanvas()
    {
        if (displayModeManager != null && canvasUpdateDictionary != null)
        {
            var displayMode = displayModeManager.GetDisplayMode();
            canvasUpdateDictionary[displayMode]();
        }
        else
        {
            DisableAllCanvas();
        }
    }

    private void DisableAllCanvas()
    {
        rewardCanvas.SetActive(false);
        policyCanvas.SetActive(false);
        vFunctionCanvas.SetActive(false);
        qFunctionCanvas.SetActive(false);
    }

    private void DisableAllArrows()
    {
        foreach (var arrowList in arrowDictionary.Values)
        {
            foreach (var arrow in arrowList)
            {
                arrow.enabled = false;
            }
        }
    }

    private void SetPolicyArrow()
    {
        DisableAllArrows();

        Action policy = mdp.Policy[state];
        foreach (var arrow in arrowDictionary[policy])
        {
            arrow.enabled = true;
        }
    }

    private void DisplayRewardCanvas()
    {
        DisableAllCanvas();

        rewardText.text = state.Reward.ToString();

        rewardCanvas.SetActive(true);
    }

    private void DisplayPolicyCanvas()
    {
        DisableAllCanvas();
        SetPolicyArrow();
        policyCanvas.SetActive(true);
    }

    private void DisplayValueCanvas()
    {
        DisableAllCanvas();
        SetPolicyArrow();

        if (mdp.IsUsingVFunction)
        {
            utilityText.text = ToRoundedString(mdp.Utility[state]);
            vFunctionCanvas.SetActive(true);
        }
        else
        {
            upQValueText.text = ToRoundedString(mdp.QFunction[state, Action.UP]);
            downQValueText.text = ToRoundedString(mdp.QFunction[state, Action.DOWN]);
            leftQValueText.text = ToRoundedString(mdp.QFunction[state, Action.LEFT]);
            rightQValueText.text = ToRoundedString(mdp.QFunction[state, Action.RIGHT]);
            qFunctionCanvas.SetActive(true);
        }
    }

    private String ToRoundedString(float number)
    {
        return Math.Round(number, decimalPlaces).ToString();
    }
}
