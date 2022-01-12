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

    private static List<(string, Action)> actionStringPairs = new List<(string, Action)>()
    {
        ("Up", Action.UP),
        ("Right", Action.RIGHT),
        ("Down", Action.DOWN),
        ("Left", Action.LEFT),
    };


    public static List<Action> GetValidActions()
    {
        var allActions = (Action[])Enum.GetValues(typeof(Action));
        return new List<Action>(allActions).FindAll(action => action != Action.NONE);
    }

    public static List<string> GetValidActionStrings()
    {
        return actionStringPairs.ConvertAll(pair => pair.Item1);
    }

    public static string GetStringFromAction(Action action)
    {
        return actionStringPairs.Find(pair => pair.Item2 == action).Item1;
    }

    public static Action GetActionFromString(String actionStr)
    {
        return actionStringPairs.Find(pair => pair.Item1 == actionStr).Item2;
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

