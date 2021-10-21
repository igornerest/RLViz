using TMPro;
using UnityEngine;

public class GridBlock : MonoBehaviour
{
    public TMP_Text policyText;
    public TMP_Text utilityText;

    private State state;
    
    private Vector3Int position;
    private float reward;
    private float utility;
    private bool isTerminal;
    private Action policy;

    public void Update()
    {
        FetchStateInfo();
    }
    public void UpdateBlock(State state)
    {
        this.state = state;
        FetchStateInfo();
    }

    private void FetchStateInfo()
    {
        this.position = state.Position;
        this.reward = state.Reward;
        this.isTerminal = state.IsTerminal;
        this.utility = state.Utility;
        this.policy = state.Policy;

        transform.position = state.Position;
        utilityText.text = state.Utility.ToString();
        policyText.text = state.Policy.ToString();
    }
}
