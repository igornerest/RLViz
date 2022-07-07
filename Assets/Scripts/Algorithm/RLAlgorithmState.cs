using System.Collections.Generic;

public class RLAlgorithmState
{
    public int MaxIt { set; get; }

    public bool IsRunning { set; get; } = false;
    public bool HasFinishedIterations { set; get; } = false;

    public State AgentState { get; set; }

    public Stack<QFunction> QFunctionStack { get; } = new Stack<QFunction>();
    public Stack<Utility> UtilityStack { get; } = new Stack<Utility>();
    public Stack<State> AgentStateStack { get; } = new Stack<State>();
    public Stack<Policy> PolicyStack { get; } = new Stack<Policy>();

    public void Reset(int maxIterations)
    {
        MaxIt = maxIterations;
        IsRunning = false;
        HasFinishedIterations = false;
        AgentState = null;
        QFunctionStack.Clear();
        UtilityStack.Clear();
        AgentStateStack.Clear();
        PolicyStack.Clear();

    }

    public void AddIterationState(Utility utility, Policy policy)
    {
        UtilityStack.Push(utility);
        PolicyStack.Push(policy);
    }

    public void AddIterationState(QFunction qFunction, State agentState)
    {
        QFunctionStack.Push(qFunction);
        AgentStateStack.Push(agentState);
    }

    public bool HasVFunctionStates()
    {
        return UtilityStack.Count > 0 && PolicyStack.Count > 0;
    }

    public bool HasQFunctionStates()
    {
        return QFunctionStack.Count > 0 && AgentStateStack.Count > 0;
    }
}
