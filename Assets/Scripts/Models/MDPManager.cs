using System.Collections.Generic;
using UnityEngine;

public class MDPManager : MonoBehaviour
{
    private static MDPManager instance = null;

    private MDP mdp;

    public static MDPManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<MDPManager>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    public MDP Mdp
    {
        get
        {
            if (mdp == null)
            {
                mdp = GetDefaultMDP();
            }
            return mdp;
        }
    }

    public RLAlgorithmState AlgorithmState { get; } = new RLAlgorithmState();

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    private MDP GetDefaultMDP()
    {
        List<State> states = new List<State>();

        int rows = 3;
        int columns = 4;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if ((j, i) == (1, 1))
                    continue;

                bool isInitial = ((j, i) == (0, 0));
                bool isTerminal = ((j, i) == (3, 2) || (j, i) == (3, 1));
                float terminalReward = (j, i) == (3, 2) ? 1f : -1f;
                float reward = isTerminal ? terminalReward : -0.04f;

                State state = new State(j, i, reward, isTerminal);
                states.Add(new State(j, i, reward, isTerminal));
            }
        }

        mdp = new MDP();
        mdp.AddStates(states);
        mdp.PromoteStateToInitial(states.Find(x => x.Position == new Vector3Int(0, 0, 0)));

        return mdp;
    }
}
