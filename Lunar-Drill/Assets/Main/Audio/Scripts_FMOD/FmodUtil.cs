using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public static class FmodUtil
{
    public static void SetMuteMaster(bool value)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.audioMasterMute = value;
#else
        Bus _bus = RuntimeManager.GetBus("bus:/");
        _bus.setMute(value);
#endif

    }
    public static void SetMuteSFX(bool value)
    {
        Bus _bus = RuntimeManager.GetBus("bus:/SFX");
        _bus.setMute(value);
    }
    public static void SetMuteMusic(bool value)
    {
        Bus _bus = RuntimeManager.GetBus("bus:/Music");
        _bus.setMute(value);
    }
    public static bool getMuteMaster()
    {
#if UNITY_EDITOR
        return UnityEditor.EditorUtility.audioMasterMute;
#else
        Bus _bus = RuntimeManager.GetBus("bus:/");
        bool ret;
        _bus.getMute(out ret);
        return ret;
#endif

    }
    public static bool getMuteSFX()
    {
        Bus _bus = RuntimeManager.GetBus("bus:/SFX");
        bool ret;
        _bus.getMute(out ret);
        return ret;
    }
    public static bool getMuteMusic()
    {
        Bus _bus = RuntimeManager.GetBus("bus:/Music");
        bool ret;
        _bus.getMute(out ret);
        return ret;
    }
}
