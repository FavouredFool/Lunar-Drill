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
