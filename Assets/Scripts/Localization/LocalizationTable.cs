using System.Collections.Generic;
using UnityEngine;

public class LocalizationTable
{
    public LocalizationEntry this[string key]
    {
        get { return localizationTable[key]; }

    }

    private static Dictionary<string, LocalizationEntry> localizationTable =
        new Dictionary<string, LocalizationEntry>()
        {
            { "key_Gamma", new LocalizationEntry
                {
                    EnglishEntry = "Gamma" ,
                    PortugueseEntry = "Gamma"
                }
            },
            { "key_Epsilon", new LocalizationEntry
                {
                    EnglishEntry = "Epsilon",
                    PortugueseEntry = "Epsilon",
                }
            },
            { "key_Alpha", new LocalizationEntry
                {
                    EnglishEntry = "Alpha",
                    PortugueseEntry = "Alpha",
                }
            },
            { "key_Time_ms", new LocalizationEntry
                {
                    EnglishEntry = "Time (ms)",
                    PortugueseEntry = "Tempo (ms)",
                }
            },
            { "key_Iterations", new LocalizationEntry
                {
                    EnglishEntry = "Iterations",
                    PortugueseEntry = "Iterações",
                }
            },
            { "key_Display_mode", new LocalizationEntry
                {
                    EnglishEntry = "Display Mode",
                    PortugueseEntry = "Modo de exibição",
                }
            },
            { "key_Probabilities", new LocalizationEntry
                {
                    EnglishEntry = "Probabilities",
                    PortugueseEntry = "Probabilidades",
                }
            },
            { "key_Interaction_mode", new LocalizationEntry
                {
                    EnglishEntry = "Interaction mode",
                    PortugueseEntry = "Modo de interação",
                }
            },
            { "key_simulate", new LocalizationEntry
                {
                    EnglishEntry = "simulate",
                    PortugueseEntry = "simular",
                }
            },
            { "key_delete", new LocalizationEntry
                {
                    EnglishEntry = "delete",
                    PortugueseEntry = "remover",
                }
            },
            { "key_create", new LocalizationEntry
                {
                    EnglishEntry = "create",
                    PortugueseEntry = "criar",
                }
            },
            { "key_edit", new LocalizationEntry
                {
                    EnglishEntry = "edit",
                    PortugueseEntry = "editar",
                }
            },
            { "key_values", new LocalizationEntry
                {
                    EnglishEntry = "values",
                    PortugueseEntry = "valores",
                }
            },
            { "key_policies", new LocalizationEntry
                {
                    EnglishEntry = "policies",
                    PortugueseEntry = "políticas",
                }
            },
            { "key_rewards", new LocalizationEntry
                {
                    EnglishEntry = "rewards",
                    PortugueseEntry = "recompensas",
                }
            },
            { "key_terminal_state", new LocalizationEntry
                {
                    EnglishEntry = "terminal state",
                    PortugueseEntry = "estado terminal",
                }
            },
            { "key_non_terminal_state", new LocalizationEntry
                {
                    EnglishEntry = "non terminal state",
                    PortugueseEntry = "estado não-terminal",
                }
            },
            { "key_initial_state", new LocalizationEntry
                {
                    EnglishEntry = "initial state",
                    PortugueseEntry = "estado inicial",
                }
            },
            { "key_Up", new LocalizationEntry
                {
                    EnglishEntry = "Up",
                    PortugueseEntry =  "Cima",
                }
            },
            { "key_Down", new LocalizationEntry
                {
                    EnglishEntry = "Down",
                    PortugueseEntry = "Baixo",
                }
            },
            { "key_Left", new LocalizationEntry
                {
                    EnglishEntry = "Left",
                    PortugueseEntry = "Esquerda",
                }
            },
            { "key_Right", new LocalizationEntry
                {
                    EnglishEntry = "Right",
                    PortugueseEntry = "Direita",
                }
            },
            { "key_No_policy", new LocalizationEntry
                {
                    EnglishEntry = "No policy",
                    PortugueseEntry = "Sem política",
                }
            },
            { "key_No_reward", new LocalizationEntry
                {
                    EnglishEntry = "No reward",
                    PortugueseEntry =  "Sem recompensa",
                }
            },
            { "key_Value_Iteration", new LocalizationEntry
                {
                    EnglishEntry = "Value Iteration",
                    PortugueseEntry = "Iteração-Valor",
                }
            },
            { "key_Policy_Iteration", new LocalizationEntry
                {
                    EnglishEntry = "Policy Iteration",
                    PortugueseEntry = "Iteração-Política",
                }
            },
            { "key_QLearning", new LocalizationEntry
                {
                    EnglishEntry = "Q-Learning",
                    PortugueseEntry = "Aprendizagem Q",
                }
            },
            { "key_SARSA", new LocalizationEntry
                {
                    EnglishEntry = "SARSA",
                    PortugueseEntry = "SARSA",
                }
            },
            { "key_Reset", new LocalizationEntry
                {
                    EnglishEntry = "Reset",
                    PortugueseEntry = "Resetar",
                }
            },
            { "key_Stop", new LocalizationEntry
                {
                    EnglishEntry = "Stop",
                    PortugueseEntry = "Parar",
                }
            },
            { "key_gamma_tooltip", new LocalizationEntry
                {
                    EnglishEntry = "Discount factor, which quantifies how much importance we give for future rewards",
                    PortugueseEntry = "Fator de desconto, que quantifica a importância que damos para recompensas futuras",
                }
            },
            { "key_epsilon_tooltip", new LocalizationEntry
                {
                    EnglishEntry = "Probability of taking a random action instead of the following the current policy  (only for Q-Learning and SARSA)",
                    PortugueseEntry = "Probabilidade de realizar uma ação aleatória em  vez de seguir a política atual (somente para Aprendizagem Q e SARSA)",
                }
            },
            { "key_alpha_tooltip", new LocalizationEntry
                {
                    EnglishEntry = "Learning rate, which represents the magnitude of step that is taken towards the solution, or simply how much we should accept the new computed value",
                    PortugueseEntry = "Taxa de aprendizado, que representa a magnitude do passo que é dado em direção à solução, ou simplesmente o quanto devemos aceitar o novo computado",
                }
            },
            { "key_forward_prob_tooltip", new LocalizationEntry
                {
                    EnglishEntry = "Probability of moving forward, following the desired direction",
                    PortugueseEntry = "Probabilidade de se mover para frente, seguindo a direção desejada",
                }
            },
            { "key_right_prob_tooltip", new LocalizationEntry
                {
                    EnglishEntry = "Probability of moving to the right of the desired direction",
                    PortugueseEntry = "Probabilidade de se mover à direita da direção desejada",
                }
            },
            { "key_backwards_prob_tooltip", new LocalizationEntry
                {
                    EnglishEntry = "Probability of moving in the opposite direction",
                    PortugueseEntry = "Probabilidade de se mover na direção oposta",
                }
            },
            { "key_left_prob_tooltip", new LocalizationEntry
                {
                    EnglishEntry = "Probability of moving to the left of the desired direction",
                    PortugueseEntry = "Probabilidade de se mover à esquerda da direção desejada",
                }
            },
        };
}
