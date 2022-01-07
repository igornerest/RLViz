public class RLAlgorithmState
{
    public int MaxIt { set; get; }
    public int CurrIt { set; get; } = 0;
    public int ItCount { set; get; } = 0;

    public RLAlgorithmState(int maxIterations)
    {
        MaxIt = maxIterations;
    }

    public bool IsActive()
    {
        return CurrIt < MaxIt;
    }
}
