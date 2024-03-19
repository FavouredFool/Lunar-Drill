using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenuUtilities : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private Toggle _fsSetting;
    [SerializeField] private TMP_Dropdown _srSetting;

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
     * Populates audio options with defualt values. 
     * TODO
     */
    private void PopulateAudioOptions()
    {
        Debug.Log("Needs to be implemented.");
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


}
