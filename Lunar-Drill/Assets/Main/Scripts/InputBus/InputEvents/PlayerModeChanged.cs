using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModeChanged : IInputSignal
{
    public bool Coop;

    public PlayerModeChanged(bool coop)
    {
        Coop = coop;
    }
}
public class PlayerModeReset : IInputSignal
{
    public PlayerModeReset()
    {
    }
}
public class PlayerModeConfirmed : IInputSignal
{
    public PlayerModeConfirmed()
    {
    }
}

public class TeamNameChanged : IInputSignal
{
    public string TeamName;
    public string LunarName,DrillianName;
    public TeamNameChanged(string teamName, string lName, string dName)
    {
        TeamName = teamName;
        LunarName = lName;
        DrillianName = dName;
    }
}
