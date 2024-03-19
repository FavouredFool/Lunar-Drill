using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenuUtilities : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] private GameObject _optionsPanel; // Panel containing option UI
    [SerializeField] private GameObject _firstSelected; // Object that should be first selected

    [SerializeField] private Slider _masterSlider; // The UI slider that corresponds to the master volume.
    [SerializeField] private Slider _musicSlider; // The UI slider that corresponds to the music volume.
    [SerializeField] private Slider _sfxSlider; // The UI slider that corresponds to the sound effects volume.
    [SerializeField] AudioMixer _audioMixer; // The Audio mixer that is being changed.
    [SerializeField] private Toggle _fsSetting; // UI toggle for setting full screen mode 
    [SerializeField] private TMP_Dropdown _srSetting; // UI dropdown for setting resolution

    //--- Private Fields ------------------------

    private bool _isOpen = false;
    private List<TMP_Dropdown.OptionData> _resolutions = new();

    //--- Unity Methods ------------------------

    private void Awake()
    {
        _optionsPanel.SetActive(false); // Do not display options at beginning
        PopulateAudioOptions();
        PopulateDisplayOptions();
    }

    //--- Public Methods ------------------------

    /* Opens and closes options panel. */
    public void Toggle()
    {
        if (_isOpen)
        {
            _optionsPanel.SetActive(false);

            // Continue game
            Time.timeScale = 1;
        }
        else
        {
            _optionsPanel.SetActive(true);
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(_firstSelected, new BaseEventData(eventSystem));

            // Pause game
            Time.timeScale = 0;
        }
        _isOpen = !_isOpen;
    }

    /* Switches the scene to main menu scene. */
    public void ToMainMenu(string sceneName)
    {
        Toggle();
        Debug.Log(sceneName);
        SceneManager.LoadScene(sceneName);
    }

    //--- Private Methods ------------------------

    /* 
     * Populates audio options with default values. 
     * TODO
     */
    private void PopulateAudioOptions()
    {
        _masterSlider.onValueChanged.AddListener(changeMasterVolume);
        float _currentMasterVolume;
        if (_audioMixer.GetFloat("MasterVolume", out _currentMasterVolume))
            _masterSlider.value = Mathf.Pow(2, (_currentMasterVolume / 10));
        _musicSlider.onValueChanged.AddListener(changeMusicVolume);
        float _currentMusicVolume;
        if (_audioMixer.GetFloat("PreMusicVolume", out _currentMusicVolume))
            _musicSlider.value = Mathf.Pow(2, (_currentMusicVolume / 10));
        _sfxSlider.onValueChanged.AddListener(changeFXVolume);
        float _currentSfxVolume;
        if (_audioMixer.GetFloat("PreSFXVolume", out _currentSfxVolume))
            _sfxSlider.value = Mathf.Pow(2, (_currentSfxVolume / 10));

    }

    /* Populates display options with default values. */
    private void PopulateDisplayOptions()
    {
        /* Full Screen */
        _fsSetting.isOn = true; // Default = full screen
        _fsSetting.onValueChanged.AddListener(ChangeFullScreen);

        /* Resolution */
        List<string> resolutionStrings = new List<string>();
        for (int i = 0; i < Screen.resolutions.Count(); i++)
        {
            if (!resolutionStrings.Contains(Screen.resolutions[i].width.ToString() + " x " + Screen.resolutions[i].height.ToString()))
            {
                resolutionStrings.Add(Screen.resolutions[i].width.ToString() + " x " + Screen.resolutions[i].height.ToString());
            }
        }
        _resolutions = resolutionStrings.Select(r => new TMP_Dropdown.OptionData(r)).ToList();
        _srSetting.options = _resolutions;
        // Listen to future changes
        _srSetting.onValueChanged.AddListener(ChangeScreenResolution);
        // Set current value as default
        _srSetting.value = _srSetting.options.FindIndex(option => option.text == Screen.currentResolution.width.ToString() + " x " + Screen.currentResolution.height.ToString());
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
    }

    /* Changes screen resolution given index of dropdown menu. */
    private void ChangeScreenResolution(int idx)
    {
        var x = _resolutions[idx].text.Split(" ")[0];
        var y = _resolutions[idx].text.Split(" ")[2];
        Screen.SetResolution(Int32.Parse(x), Int32.Parse(y), FullScreenMode.ExclusiveFullScreen);
    }

    /* Function to change the Master Volume */
    public void changeMasterVolume(float value)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log(value, 2) * 10f); // Uses a logarithmic Scaling since that is more in line with our perception. (e.g. -10 db corresponds roughly to haling the  preceived noise)
    }

    /* Function to change the MUsic Volume */
    public void changeMusicVolume(float value)
    {
        _audioMixer.SetFloat("PreMusicVolume", Mathf.Log(value, 2) * 10f); // Uses a logarithmic Scaling since that is more in line with our perception. (e.g. -10 db corresponds roughly to haling the  preceived noise)
    }
    /* Function to change the FX Volume */
    public void changeFXVolume(float value)
    {
        _audioMixer.SetFloat("PreSFXVolume", Mathf.Log(value, 2) * 10f); // Uses a logarithmic Scaling since that is more in line with our perception. (e.g. -10 db corresponds roughly to haling the  preceived noise)
    }

}
