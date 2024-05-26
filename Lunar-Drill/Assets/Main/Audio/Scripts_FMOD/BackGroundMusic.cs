using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackGroundMusic : MonoBehaviour
{
    [SerializeField]
    EventReference _musicEvent; // the event that plays the music

    EventInstance _music;

    public static BackGroundMusic Instance;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!(scene.name == "MainScene" || scene.name == "SelectScreen" || scene.name == "MainMenuScene" || scene.name == "AudioTestScene"))
        {
            Destroy(gameObject);
        }
    }


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;

        _music = RuntimeManager.CreateInstance(_musicEvent);
        _music.start();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // Stop  audio if it is running
        PLAYBACK_STATE ps;
        _music.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.STOPPED)
        {
            _music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
