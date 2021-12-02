using System;
using TMPro;
using UnityEngine;

public class GridBlock : MonoBehaviour
{
    public Canvas vFunctionCanvas;
    public TMP_Text policyText;
    public TMP_Text utilityText;

    public Canvas qFunctionCanvas;
    public TMP_Text upQValueText;
    public TMP_Text downQValueText;
    public TMP_Text leftQValueText;
    public TMP_Text rightQValueText;

    private State state;
    private MDP mdp;

    private Vector3Int position;
    private float reward;
    private float utility;
    private bool isTerminal;
    private Action policy;

    private int decimalPlaces = 2;

    public void Update()
    {
        FetchStateInfo();
    }
    public void UpdateBlock(State state, MDP mdp)
    {
        this.state = state;
        this.mdp = mdp;
        FetchStateInfo();
    }

    private void FetchStateInfo()
    {
        this.position = state.Position;
        this.reward = state.Reward;
        this.isTerminal = state.IsTerminal;

        this.utility = mdp.Utility[state];
        this.policy = mdp.Policy[state];

        transform.position = this.state.Position;

        if (mdp.IsUsingVFunction)
        {
            qFunctionCanvas.enabled = false;
            vFunctionCanvas.enabled = true;

            utilityText.text = ToRoundedString(mdp.Utility[state]);
            policyText.text = this.policy.ToString();
        }
        else
        {
            vFunctionCanvas.enabled = false;
            qFunctionCanvas.enabled = true;

            upQValueText.text = ToRoundedString(mdp.QFunction[state, Action.UP]);
            downQValueText.text = ToRoundedString(mdp.QFunction[state, Action.DOWN]);
            leftQValueText.text = ToRoundedString(mdp.QFunction[state, Action.LEFT]);
            rightQValueText.text = ToRoundedString(mdp.QFunction[state, Action.RIGHT]);
        }
    }

    private String ToRoundedString(float number)
    {
        return Math.Round(number, decimalPlaces).ToString();
    }
}
