using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class OptionsMenuUtilities : MonoBehaviour
{
    //--- Exposed Fields ------------------------
    [SerializeField] private Slider _masterSlider; // The UI slider that corresponds to the master volume.
    [SerializeField] private Slider _musicSlider; // The UI slider that corresponds to the music volume.
    [SerializeField] private Slider _sfxSlider; // The UI slider that corresponds to the sound effects volume.
    [SerializeField] AudioMixer _audioMixer; // The Audio mixer that is being changed.
    [SerializeField] private Toggle _fsSetting; // UI toggle for setting full screen mode 
    [SerializeField] private Toggle _vibrationSetting; // UI toggle for setting vibration on/off 

    //--- Private Fields ------------------------
    private Bus _masterBus;
    private VCA _sfxVCA;
    private VCA _musicVCA;

    //--- Unity Methods ------------------------

    private void Awake()
    {
        PopulateAudioOptions();
        PopulateDisplayOptions();
        PopulateVibrationOptions();
        _masterBus = RuntimeManager.GetBus("bus:/");
        _sfxVCA = RuntimeManager.GetVCA("vca:/SFX");
        _musicVCA = RuntimeManager.GetVCA("vca:/Music");
    }

    //--- Public Methods ------------------------

    //--- Private Methods ------------------------

    /* 
     * Populates audio options with default values. 
     */
    private void PopulateAudioOptions()
    {
        _masterSlider.onValueChanged.AddListener(ChangeMasterVolume);
        float _currentMasterVolume;
        _masterBus.getVolume(out _currentMasterVolume);
        _masterSlider.value = _currentMasterVolume;

        _musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        float _currentMusicVolume;
        _musicVCA.getVolume(out _currentMusicVolume);
        _musicSlider.value = _currentMusicVolume;

        _sfxSlider.onValueChanged.AddListener(ChangeFXVolume);
        float _currentSfxVolume;
        _sfxVCA.getVolume(out _currentSfxVolume);
        _sfxSlider.value = _currentSfxVolume;

    }

    /* Populates display options with default values. */
    private void PopulateDisplayOptions()
    {
        /* Full Screen */
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            _fsSetting.isOn = false;
        }
        else
        {
            _fsSetting.isOn = true; // Default = full screen
        }
        _fsSetting.onValueChanged.AddListener(ChangeFullScreen);
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            _fsSetting.isOn = false;
        }
        else
        {
            _fsSetting.isOn = true; // Default = full screen
        }
    }

    /* Populates vibration options with default values. */
    private void PopulateVibrationOptions()
    {
        _vibrationSetting.isOn = Rumble.rumbleEnabled;
        _vibrationSetting.onValueChanged.AddListener(ChangeVibration);
    }

    /* Toggles full screen mode. */
    private void ChangeFullScreen(bool on)
    {
        if (on)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        CameraAdjuster.main.Adjust();
    }

    /* Toggles vobration. */
    private void ChangeVibration(bool on)
    {
        Rumble.rumbleEnabled = on;
        Debug.Log(Rumble.rumbleEnabled);
        Rumble.main?.ClearAndStopAllRumble();
    }

    /* Function to change the Master Volume */
    public void ChangeMasterVolume(float value)
    {
        _masterBus.setVolume(value);
    }

    /* Function to change the MUsic Volume */
    public void ChangeMusicVolume(float value)
    {
        _musicVCA.setVolume(value);
    }
    /* Function to change the FX Volume */
    public void ChangeFXVolume(float value)
    {
        _sfxVCA.setVolume(value);
    }

}
