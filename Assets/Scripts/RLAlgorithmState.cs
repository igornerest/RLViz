public class RLAlgorithmState
{
    public int MaxIt { set; get; }
    public int CurrIt { set; get; } = 0;
    public int ItCount { set; get; } = 0;

    public bool IsRunning { set; get; } = false;

    public State AgentState { get; set; }

    public void Reset(int maxIterations)
    {
        MaxIt = maxIterations;
        CurrIt = 0;
        ItCount = 0;
        IsRunning = false;
    }

    public bool HasFinishedIterations()
    {
        return CurrIt < MaxIt;
    }
}
