using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem;
using DG.Tweening;

public static class Rumble
{
    public static UnityEngine.InputSystem.Users.InputUser
        drillian, luna;

    //static Tween drillianT, lunaT;

    public static void AddGamepad(PlayerInput map, PlayerConnectController.ChosenCharacter c)
    {
        switch (c)
        {
            case PlayerConnectController.ChosenCharacter.drillian:
                drillian = map.user;
                break;
            case PlayerConnectController.ChosenCharacter.luna:
                luna = map.user;
                break;
            default:
                drillian = map.user;
                luna = map.user;
                break;
        }

        if (drillian != null)
            foreach (var a in drillian.pairedDevices)
            {
                if (a is Gamepad)
                    (a as Gamepad).SetMotorSpeeds(0.5f, 0.5f);
            }
        if (luna != null)
            foreach (var a in luna.pairedDevices)
            {
                if (a is Gamepad)
                    (a as Gamepad).SetMotorSpeeds(0.5f, 0.5f);
            }
    }
    public static void DrillianRumble(bool on)
    {
        foreach (IHaptics haptics in drillian.pairedDevices)
        {
            if (on) haptics.ResumeHaptics();
            else haptics.PauseHaptics();
        }
    }
    public static void LunaRumble(bool on)
    {
        foreach (IHaptics haptics in luna.pairedDevices)
        {
            if (on) haptics.ResumeHaptics();
            else haptics.PauseHaptics();
        }
    }
}
