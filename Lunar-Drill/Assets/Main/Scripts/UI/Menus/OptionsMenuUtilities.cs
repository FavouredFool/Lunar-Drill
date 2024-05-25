using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

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

    //--- Unity Methods ------------------------

    private void Awake()
    {
        PopulateAudioOptions();
        PopulateDisplayOptions();
        PopulateVibrationOptions();
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
        if (_audioMixer.GetFloat("MasterVolume", out _currentMasterVolume))
            _masterSlider.value = Mathf.Pow(2, (_currentMasterVolume / 10));
        _musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        float _currentMusicVolume;
        if (_audioMixer.GetFloat("PreMusicVolume", out _currentMusicVolume))
        {
            _musicSlider.value = Mathf.Pow(2, (_currentMusicVolume / 10));
            _audioMixer.SetFloat("PostMusicVolume", _currentMusicVolume); // Uses a logarithmic Scaling since that is more in line with our perception. (e.g. -10 db corresponds roughly to haling the  preceived noise)

        }
        _sfxSlider.onValueChanged.AddListener(ChangeFXVolume);
        float _currentSfxVolume;
        if (_audioMixer.GetFloat("PreSFXVolume", out _currentSfxVolume))
        {
            _audioMixer.SetFloat("PreSFXVolume", _currentSfxVolume);
            _sfxSlider.value = Mathf.Pow(2, (_currentSfxVolume / 10));
        }

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
        _audioMixer.SetFloat("MasterVolume", Mathf.Log(value, 2) * 10f); // Uses a logarithmic Scaling since that is more in line with our perception. (e.g. -10 db corresponds roughly to haling the  preceived noise)
    }

    /* Function to change the MUsic Volume */
    public void ChangeMusicVolume(float value)
    {
        _audioMixer.SetFloat("PreMusicVolume", Mathf.Log(value, 2) * 10f); // Uses a logarithmic Scaling since that is more in line with our perception. (e.g. -10 db corresponds roughly to haling the  preceived noise)
        _audioMixer.SetFloat("PostMusicVolume", Mathf.Log(value, 2) * 10f); // Uses a logarithmic Scaling since that is more in line with our perception. (e.g. -10 db corresponds roughly to haling the  preceived noise)
    }
    /* Function to change the FX Volume */
    public void ChangeFXVolume(float value)
    {
        _audioMixer.SetFloat("PreSFXVolume", Mathf.Log(value, 2) * 10f); // Uses a logarithmic Scaling since that is more in line with our perception. (e.g. -10 db corresponds roughly to haling the  preceived noise)
        _audioMixer.SetFloat("PostSFXVolume", Mathf.Log(value, 2) * 10f); // Uses a logarithmic Scaling since that is more in line with our perception. (e.g. -10 db corresponds roughly to haling the  preceived noise)
    }

}
