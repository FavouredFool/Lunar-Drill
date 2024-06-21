using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FmodUtil
{
    public static void SetMuteMaster(bool value)
    {
        Bus _bus = RuntimeManager.GetBus("bus:/");
        _bus.setMute(value);
    }
    public static void SetMuteVFX(bool value)
    {
        Bus _bus = RuntimeManager.GetBus("bus:/VFX");
        _bus.setMute(value);
    }
    public static void SetMuteMusic(bool value)
    {
        Bus _bus = RuntimeManager.GetBus("bus:/Music");
        _bus.setMute(value);
    }
    public static bool getMuteMaster()
    {
        Bus _bus = RuntimeManager.GetBus("bus:/");
        bool ret;
        _bus.getMute(out ret);
        return ret;
    }
    public static bool getMuteVFX()
    {
        Bus _bus = RuntimeManager.GetBus("bus:/VFX");
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
