using System;

public enum Action
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}

public enum Deviation
{
    FORWARD,
    RIGHT,
    BACK,
    LEFT
}

public static class ActionExtensions {
    public static Action Deviate(Action action, Deviation deviation)
    {
        int actionCount = Enum.GetNames(typeof(Action)).Length;
        int actionVal = (int)action;
        int deviationVal = (int)deviation;

        return (Action)((actionVal + deviationVal + actionCount) % actionCount);
    }
}

