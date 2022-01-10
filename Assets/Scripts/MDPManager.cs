using System.Collections.Generic;
using UnityEngine;

public class MDPManager : MonoBehaviour
{
    private static MDPManager instance = null;

    private MDP mdp;

    private Dictionary<Deviation, float> defaultDeviationProbs =
        new Dictionary<Deviation, float> {
            { Deviation.FORWARD,    0.8f },
            { Deviation.LEFT,       0.1f },
            { Deviation.RIGHT,      0.1f }
        };

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
        mdp = new MDP();

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
                mdp.AddState(state, isInitial);
            }
        }

        mdp.EvaluateProbabilities(defaultDeviationProbs);

        return mdp;
    }
}
