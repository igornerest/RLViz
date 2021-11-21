using TMPro;
using UnityEngine;

public class GridBlock : MonoBehaviour
{
    public TMP_Text policyText;
    public TMP_Text utilityText;

    private State state;
    private MDP mdp;

    private Vector3Int position;
    private float reward;
    private float utility;
    private bool isTerminal;
    private Action policy;

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
        utilityText.text = this.utility.ToString();
        policyText.text = this.policy.ToString();
    }
}
