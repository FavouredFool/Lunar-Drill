using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Rumble : MonoBehaviour
{
    public static Rumble main;
    public static bool rumbleDisabled;
    public static bool rumblePaused;
    public static bool AllowRumble => !rumbleDisabled && !rumblePaused;

    public UnityEngine.InputSystem.Users.InputUser
        shared, 
        drillian, 
        luna;

    private Gamepad 
        drillianGamepad,
        lunaGamepad,
        sharedGamepad;

    public List<Profile>
        drillianRumble= new(),
        lunaRumble= new();
    public List<Profile> sharedRumble
    {
        get
        {
            List<Profile> result= new List<Profile>();
            result.AddRange(drillianRumble);
            result.AddRange(lunaRumble);
            return result;
        }
    }
    [System.Serializable]public class Profile
    {
        public float lowFrequency, highFrequency;
        public float endTime;

        public Profile(float lowF, float highF, float endT)
        {
            lowFrequency = lowF;
            highFrequency = highF;
            endTime = endT;
        }
    }

    static bool isSingleplayer;

    private void Awake()
    {
        if (main != null)
        {
            Destroy(gameObject);
        }
        else
        {
            main = this;
            rumblePaused = false;
            rumbleDisabled = false;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AddGamepad(PlayerInput map, ChosenCharacter c)
    {
        isSingleplayer = false;

        switch (c)
        {
            case ChosenCharacter.drillian:
                drillian = map.user;

                drillianGamepad = drillian.pairedDevices[0] as Gamepad;

                break;
            case ChosenCharacter.luna:
                luna = map.user;

                lunaGamepad = luna.pairedDevices[0] as Gamepad;

                break;
            default:
                shared = map.user;
                isSingleplayer = true;

                sharedGamepad = shared.pairedDevices[0] as Gamepad;

                break;
        }
    }
    public Gamepad GetGamepad(ChosenCharacter c)
    {
        if (isSingleplayer) c = ChosenCharacter.singleplayer;

        switch (c)
        {
            case ChosenCharacter.drillian:
                return drillianGamepad;
            case ChosenCharacter.luna:
                return lunaGamepad;
            default:
                return sharedGamepad;
        }
    }


    public Profile RumbleBoth(float level, float excentricity = 1, float duration = -1)
    {
        (float, float) frequency = InputToFrequency(level, excentricity);
        Profile profile = new Profile(frequency.Item1, frequency.Item2, duration < 0 ? duration : Time.unscaledTime + duration);
        drillianRumble.Add(profile);
        lunaRumble.Add(profile);

        RefreshAllRumble();

        return profile;
    }
    public Profile RumbleDrillian(float level, float excentricity=1, float duration = -1)
    {
        (float, float) frequency = InputToFrequency(level, excentricity);
        Profile profile = new Profile(frequency.Item1, frequency.Item2, duration < 0 ? duration : Time.unscaledTime + duration);
        drillianRumble.Add(profile);

        RefreshAllRumble();

        return profile;
    }
    public Profile RumbleLuna(float level, float excentricity=1, float duration = -1)
    {
        (float, float) frequency = InputToFrequency(level, excentricity);
        Profile profile = new Profile(frequency.Item1, frequency.Item2, duration < 0 ? duration : Time.unscaledTime + duration);
        lunaRumble.Add(profile);

        RefreshAllRumble();

        return profile;
    }

    public (float, float) InputToFrequency(float level, float excentricity = 1)
    {
        float lowFrequency = level * 0.15f;
        float highFrequency = lowFrequency + excentricity * 0.1f;
        return (lowFrequency, highFrequency);
    }

    private void RefreshAllRumble()
    {
        if (!AllowRumble) return;

        if (isSingleplayer)
        {
            RefreshRumble(GetGamepad(ChosenCharacter.singleplayer), sharedRumble);
        }
        else
        {
            RefreshRumble(GetGamepad(ChosenCharacter.drillian), drillianRumble);
            RefreshRumble(GetGamepad(ChosenCharacter.luna), lunaRumble);
        }
    }
    public void RefreshRumble(Gamepad gamepad, List<Profile> rumble)
    {
        if (gamepad == null || !AllowRumble) return;

        Profile combined = new Profile(0,0,0);

        for (int i = 0; i < rumble.Count; i++)
        {
            Profile rp = rumble[i];

            if (rp.endTime<0||Time.unscaledTime<rp.endTime)
            {
                combined.lowFrequency = Mathf.Max(combined.lowFrequency, rp.lowFrequency);
                combined.highFrequency = Mathf.Max(combined.lowFrequency, combined.highFrequency, rp.highFrequency);
            }
            else //Rumble is over and can be removed
            {
                rumble.RemoveAt(i);
                if(isSingleplayer) RemoveRumbleAnywhere(rp);
                i--;
            }
        }
        SetRumble(gamepad,combined);
    }
    public void RemoveRumbleAnywhere(Profile profile)
    {
        if (profile == null) return;

        if (drillianRumble.Contains(profile))
            drillianRumble.Remove(profile);
        if (lunaRumble.Contains(profile))
            lunaRumble.Remove(profile);
    }
    public void SetRumble(Gamepad gamepad, Profile rumble)
    {
        Debug.Log("RUMBLE on " + gamepad);
        if (!AllowRumble)
        {
            Debug.LogError("Rumble set while disabled!");
        }
        gamepad?.SetMotorSpeeds(rumble.lowFrequency, rumble.highFrequency);
    }
    
    public void ClearAndStopAllRumble()
    {
        drillianRumble.Clear();
        lunaRumble?.Clear();
        StopAllRumble();
    }
    public void StopAllRumble()
    {
        StopRumble(drillianGamepad);
        StopRumble(lunaGamepad);
        StopRumble(sharedGamepad);
    }
    public void StopRumble(Gamepad gamepad)=> gamepad?.SetMotorSpeeds(0, 0);


    private void Update()
    {
        RefreshAllRumble();
    }
    private void OnDisable()
    {
        StopAllRumble();
    }

    //private IEnumerator DoRumble(Gamepad gamepad, float lowFrequency, float highFrequency, float duration)
    //{
    //    if (gamepad == null) yield break;

    //    gamepad.SetMotorSpeeds(lowFrequency, highFrequency);

    //    yield return new WaitForSeconds(duration);

    //    gamepad.SetMotorSpeeds(0, 0); // Stop rumble
    //}

    //List<(Vector2, float)>
    //   lunaStack = new(),
    //   drillianStack = new(),
    //   singleplayerStack = new();

    //public UnityEngine.InputSystem.Users.InputUser GetUser(ChosenCharacter c)
    //{
    //    switch (c)
    //    {
    //        case ChosenCharacter.drillian:
    //            return drillian;
    //        case ChosenCharacter.luna:
    //            return luna;
    //        default:
    //            return drillian;
    //    }
    //}
    //public List<(Vector2, float)> GetStack(ChosenCharacter c)
    //{
    //    if (isSingleplayer)
    //        return singleplayerStack;

    //    switch (c)
    //    {
    //        case ChosenCharacter.drillian:
    //            return drillianStack;
    //        case ChosenCharacter.luna:
    //            return lunaStack;
    //        default:
    //            return singleplayerStack;
    //    }
    //}

    //public void AddRumble(ChosenCharacter c, Vector2 vec, float time=-1) //Negative time is Permanent
    //{
    //    List<(Vector2, float)> stack = GetStack(c);
    //    if (stack == null) return;

    //    stack.Add((vec, time==-1? -1: Time.time + time));
    //    Debug.Log($"Added {c} rumble of {vec} for {time}.  Stack size is {stack.Count}");
    //}
    //public void RemovePermanentRumble(ChosenCharacter c, Vector2 vec)
    //{
    //    List<(Vector2, float)> stack = GetStack(c);
    //    if (stack == null) return;

    //    for (int i = 0; i < stack.Count; i++)
    //    {
    //        if (stack[i].Item1 == vec && stack[i].Item2 < 0)
    //        {
    //            stack.RemoveAt(i);
    //            Debug.Log($"Removed {c} rumble of {vec}. Stack size is {stack.Count}");
    //            return;
    //        }
    //    }
    //    Debug.Log($"Failed to remove {c} rumble of {vec}. Stack size remains {stack.Count}");

    //}

    //private void FixedUpdate()
    //{
    //    (UnityEngine.InputSystem.Users.InputUser, List<(Vector2, float)>)[] stacks =
    //        isSingleplayer ?
    //        new (UnityEngine.InputSystem.Users.InputUser, List<(Vector2, float)>)[1]
    //        {(drillian,singleplayerStack)}
    //        :
    //        new (UnityEngine.InputSystem.Users.InputUser, List<(Vector2, float)>)[2]
    //        {(drillian,drillianStack), (luna,lunaStack)};

    //    foreach ((UnityEngine.InputSystem.Users.InputUser, List<(Vector2, float)>) pair in stacks)
    //    {
    //        UnityEngine.InputSystem.Users.InputUser u = pair.Item1;
    //        List<(Vector2, float)> s = pair.Item2;

    //        Vector2 rumbleForce = Vector2.zero;
    //        for (int i = 0; i < s.Count; i++)
    //        {
    //            if (s[i].Item2<0||Time.time < s[i].Item2)
    //            {
    //                Vector2 force = s[i].Item1;
    //                if (rumbleForce.x < force.x) rumbleForce.x = force.x;
    //                if (rumbleForce.y < force.y) rumbleForce.y = force.y;
    //            }
    //            else
    //            {
    //                s.RemoveAt(i);
    //                i--;
    //            }
    //        }
    //        if(u!=null&&u.valid) foreach (var d in u.pairedDevices)
    //        {
    //                if (d is Gamepad)
    //                {
    //                    (d as Gamepad).SetMotorSpeeds(rumbleForce.x, rumbleForce.y);
    //                    if (rumbleForce==Vector2.zero)
    //                        (d as IHaptics).PauseHaptics();
    //                    else
    //                        (d as IHaptics).ResumeHaptics();
    //                }
    //        }

    //    }
    //}

    //public void SetForce(ChosenCharacter c, Vector2 force)
    //{
    //    UnityEngine.InputSystem.Users.InputUser user = GetUser(c);

    //    foreach (var a in user.pairedDevices)
    //    {
    //        if (a is Gamepad)
    //            (a as Gamepad).SetMotorSpeeds(force.x, force.y);
    //    }
    //}

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
