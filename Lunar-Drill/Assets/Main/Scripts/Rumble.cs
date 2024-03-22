using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Rumble : MonoBehaviour
{
    public static Rumble main;

    public UnityEngine.InputSystem.Users.InputUser
        drillian, luna;

    static bool isSingleplayer;

    public List<(Vector2, float)>
        lunaStack = new(),
        drillianStack = new(),
        singleplayerStack = new();

    public UnityEngine.InputSystem.Users.InputUser GetUser(ChosenCharacter c)
    {
        switch (c)
        {
            case ChosenCharacter.drillian:
                return drillian;
            case ChosenCharacter.luna:
                return luna;
            default:
                return drillian;
        }
    }
    public List<(Vector2, float)> GetStack(ChosenCharacter c)
    {
        if (isSingleplayer)
            return singleplayerStack;

        switch (c)
        {
            case ChosenCharacter.drillian:
                return drillianStack;
            case ChosenCharacter.luna:
                return lunaStack;
            default:
                return singleplayerStack;
        }
    }

    public void AddGamepad(PlayerInput map, ChosenCharacter c)
    {
        isSingleplayer = false;

        switch (c)
        {
            case ChosenCharacter.drillian:
                drillian = map.user;
                break;
            case ChosenCharacter.luna:
                luna = map.user;
                break;
            default:
                drillian = map.user;
                luna = map.user;
                isSingleplayer = true;
                break;
        }

        if (drillian != null)
            foreach (var a in drillian.pairedDevices)
            {
                if (a is Gamepad)
                    (a as Gamepad).SetMotorSpeeds(0.1f, 0.3f);
            }
        if (luna != null)
            foreach (var a in luna.pairedDevices)
            {
                if (a is Gamepad)
                    (a as Gamepad).SetMotorSpeeds(0.1f, 0.3f);
            }
    }

    public void AddRumble(ChosenCharacter c, Vector2 vec, float time=-1) //Negative time is Permanent
    {
        List<(Vector2, float)> stack = GetStack(c);
        if (stack == null) return;

        stack.Add((vec, Time.time + time));
    }
    public void RemovePermanentRumble(ChosenCharacter c, Vector2 vec)
    {
        List<(Vector2, float)> stack = GetStack(c);
        if (stack == null) return;

        for (int i = 0; i < stack.Count; i++)
        {
            if (stack[i].Item1 == vec && stack[i].Item2 < 0)
            {
                stack.RemoveAt(i);
                return;
            }
        }
    }

    private void Awake()
    {
        if (main != null)
        {
            Destroy(gameObject);
        }
        else
        {
            main = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void FixedUpdate()
    {
        (UnityEngine.InputSystem.Users.InputUser, List<(Vector2, float)>)[] stacks =
            isSingleplayer ?
            new (UnityEngine.InputSystem.Users.InputUser, List<(Vector2, float)>)[1]
            {(drillian,singleplayerStack)}
            :
            new (UnityEngine.InputSystem.Users.InputUser, List<(Vector2, float)>)[2]
            {(drillian,drillianStack), (luna,lunaStack)};

        foreach ((UnityEngine.InputSystem.Users.InputUser, List<(Vector2, float)>) pair in stacks)
        {
            UnityEngine.InputSystem.Users.InputUser u = pair.Item1;
            List<(Vector2, float)> s = pair.Item2;

            Vector2 rumbleForce = Vector2.zero;
            for (int i = 0; i < s.Count; i++)
            {
                if (s[i].Item2<0||Time.time < s[i].Item2)
                {
                    Vector2 force = s[i].Item1;
                    if (rumbleForce.x < force.x) rumbleForce.x = force.x;
                    if (rumbleForce.y < force.y) rumbleForce.y = force.y;
                }
                else
                {
                    s.RemoveAt(i);
                    i--;
                }
            }
            if(u!=null) foreach (var d in u.pairedDevices)
            {
                    if (d is Gamepad)
                    {
                        (d as Gamepad).SetMotorSpeeds(rumbleForce.x, rumbleForce.y);
                        if (rumbleForce==Vector2.zero)
                            (d as IHaptics).PauseHaptics();
                        else
                            (d as IHaptics).ResumeHaptics();
                    }
            }

        }
    }

    public void SetForce(ChosenCharacter c, Vector2 force)
    {
        UnityEngine.InputSystem.Users.InputUser user = GetUser(c);

        foreach (var a in user.pairedDevices)
        {
            if (a is Gamepad)
                (a as Gamepad).SetMotorSpeeds(force.x, force.y);
        }
    }

    //public static void DrillianRumble(Vector2 force, bool on, float time = -1)
    //{
    //    foreach (var a in drillian.pairedDevices)
    //    {
    //        if (a is Gamepad)
    //            (a as Gamepad).SetMotorSpeeds(force.x, force.y);
    //    }



    //    DrillianRumble(on);
    //}
    //public static void DrillianRumble(bool on)
    //{
    //    drillianRumble = on;
    //    if (isSingleplayer) on = lunaRumble || drillianRumble;

    //    foreach (IHaptics haptics in drillian.pairedDevices)
    //    {
    //        if (on) haptics.ResumeHaptics();
    //        else haptics.PauseHaptics();
    //    }
    //}

    //public static void LunaRumble(Vector2 force, bool on, float time=-1)
    //{
    //    foreach (var a in luna.pairedDevices)
    //    {
    //        if (a is Gamepad)
    //            (a as Gamepad).SetMotorSpeeds(force.x, force.y);
    //    }



    //    LunaRumble(on);
    //}
    //public static void LunaRumble(bool on)
    //{
    //    lunaRumble = on;
    //    if (isSingleplayer) on = lunaRumble || drillianRumble;

    //    foreach (IHaptics haptics in luna.pairedDevices)
    //    {
    //        if (on) haptics.ResumeHaptics();
    //        else haptics.PauseHaptics();
    //    }
    //}
}
