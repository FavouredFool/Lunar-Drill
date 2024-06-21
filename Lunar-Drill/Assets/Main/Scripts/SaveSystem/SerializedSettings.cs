using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedSettings
{
    public float MasterVolume;
    public bool MasterMuted;
    public float MusicVolume;
    public bool MusicMuted;
    public float SFXVolume;
    public bool SFXMuted;

    public bool FullScreen;
    public bool Vibration;


    public void Serialize()
    {
        // Get Music settings
        Bus _masterBus = RuntimeManager.GetBus("bus:/");
        Bus _sfxBus = RuntimeManager.GetBus("Bus:/SFX");
        Bus _musicBus = RuntimeManager.GetBus("bus:/Music");
        _masterBus.getVolume(out MasterVolume);
        _masterBus.getMute(out MasterMuted);
        _musicBus.getVolume(out MusicVolume);
        _masterBus.getMute(out MusicMuted);
        _sfxBus.getVolume(out SFXVolume);
        _sfxBus.getMute(out SFXMuted);

        // Get Full Screen Settings
        FullScreen = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;

        // Get Rumble Setting
        Vibration = !Rumble.rumbleDisabled;
    }
    public void Deserialize()
    {
        // Set Music settings
        Bus _masterBus = RuntimeManager.GetBus("bus:/");
        Bus _sfxBus = RuntimeManager.GetBus("bus:/SFX");
        Bus _musicBus = RuntimeManager.GetBus("bus:/Music");
        _masterBus.setVolume(MasterVolume);
        _musicBus.setVolume(MusicVolume);
        _sfxBus.setVolume(SFXVolume);
        _masterBus.setMute(MusicMuted);
        _sfxBus.setVolume(SFXVolume);
        _sfxBus.setMute(SFXMuted);

        // Set Full Screen Settings
        Screen.fullScreenMode = FullScreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;

        // Set Rumble Setting
        Rumble.rumbleDisabled = !Vibration;
    }
}
