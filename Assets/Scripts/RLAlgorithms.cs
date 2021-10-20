using System;
using System.Linq;

public static class RLAlgorithms
{
    public static void valueIteration(MDP mdp, float gamma, float episilon)
    {
        float delta = episilon * (1 - gamma) / gamma + 1;

        Utility utility = new Utility(mdp.getAllStates());

        while (delta > episilon * (1 - gamma) / gamma)
        {
            Utility newUtility = new Utility(utility);

            delta = 0;
            
            foreach (State state in mdp.getAllStates())
            {
                if (state.IsTerminal)
                {
                    utility[state] = state.Reward;
                }
                else
                {
                    float maxUtility = newUtility.GetExpectedValues(state).Max();
                    utility[state] = state.Reward + gamma * maxUtility;
                }

                delta = Math.Max(delta, Math.Abs(utility[state] - newUtility[state]));
            }
        }

        mdp.UpdateUtility(utility);
    }
}
