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

        _music = RuntimeManager.CreateInstance(_musicEvent);
        _music.start();
    }

    void OnDestroy()
    {
        // Stop  audio if it is running
        PLAYBACK_STATE ps;
        _music.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.STOPPED)
        {
            _music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
