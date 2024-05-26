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
    public float MusicVolume;
    public float SFXVolume;

    public bool FullScreen;
    public bool Vibration;


    public void Serialize()
    {
        // Get Music settings
        Bus _masterBus = RuntimeManager.GetBus("bus:/");
        VCA _sfxVCA = RuntimeManager.GetVCA("vca:/SFX");
        VCA _musicVCA = RuntimeManager.GetVCA("vca:/Music");
        _masterBus.getVolume(out MasterVolume);
        _musicVCA.getVolume(out MusicVolume);
        _sfxVCA.getVolume(out SFXVolume);

        // Get Full Screen Settings
        FullScreen = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;

        // Get Rumble Setting
        Vibration = !Rumble.rumbleDisabled;
    }
    public void Deserialize()
    {
        // Set Music settings
        Bus _masterBus = RuntimeManager.GetBus("bus:/");
        VCA _sfxVCA = RuntimeManager.GetVCA("vca:/SFX");
        VCA _musicVCA = RuntimeManager.GetVCA("vca:/Music");
        _masterBus.setVolume(MasterVolume);
        _musicVCA.setVolume(MusicVolume);
        _sfxVCA.setVolume(SFXVolume);

        // Set Full Screen Settings
        Screen.fullScreenMode = FullScreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;

        // Set Rumble Setting
        Rumble.rumbleDisabled = !Vibration;
    }
}
