using System;
using System.Collections.Generic;

public enum Action
{
    UP,
    RIGHT,
    DOWN,
    LEFT,
    NONE,
}

public enum Deviation
{
    FORWARD,
    RIGHT,
    BACK,
    LEFT
}

public static class ActionExtensions {

    public static List<Action> GetValidActions()
    {
        var allActions = (Action[])Enum.GetValues(typeof(Action));
        return new List<Action>(allActions).FindAll(action => action != Action.NONE);
    }

    public static Action Deviate(Action action, Deviation deviation)
    {
        if (action == Action.NONE)
        {
            throw new Exception("No deviation expected for action NONE");
        }

        int actionCount = Enum.GetNames(typeof(Action)).Length - 1;
        int actionVal = (int)action;
        int deviationVal = (int)deviation;

        return (Action)((actionVal + deviationVal + actionCount) % actionCount);
    }
}

