using System.Collections.Generic;

public class RLAlgorithmState
{
    public int MaxIt { set; get; }

    public bool IsRunning { set; get; } = false;
    public bool HasFinishedIterations { set; get; } = false;

    public State AgentState { get; set; }

    public Stack<QFunction> qFunctionStack { get; } = new Stack<QFunction>();
    public Stack<Utility> utilityStack { get; } = new Stack<Utility>();
    public Stack<State> agentStateStack { get; } = new Stack<State>();
    public Stack<Policy> policyStack { get; } = new Stack<Policy>();

    public void Reset(int maxIterations)
    {
        MaxIt = maxIterations;
        IsRunning = false;
        HasFinishedIterations = false;
        qFunctionStack.Clear();
        utilityStack.Clear();
        agentStateStack.Clear();
        policyStack.Clear();

    }

    public void AddIterationState(Utility utility, Policy policy)
    {
        utilityStack.Push(utility);
        policyStack.Push(policy);
    }

    public void AddIterationState(QFunction qFunction, State agentState)
    {
        qFunctionStack.Push(qFunction);
        agentStateStack.Push(agentState);
    }

    public bool HasVFunctionStates()
    {
        return utilityStack.Count > 0 && policyStack.Count > 0;
    }

    public bool HasQFunctionStates()
    {
        return qFunctionStack.Count > 0 && agentStateStack.Count > 0;
    }
}
