using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAudioFMOD : MonoBehaviour, IAudioSubscriber<MenuSelectAudio>, IAudioSubscriber<MenuClickAudio>, IAudioSubscriber<MenuPauseAudio>
{
    [SerializeField]
    EventReference _selectEvent;
    [SerializeField]
    EventReference _clickEvent; 
    [SerializeField]
    EventReference _pauseEvent;

    EventInstance _pauseEventInstance;

    public void OnAudioEvent(MenuSelectAudio audioEvent)
    {
        RuntimeManager.PlayOneShot(_selectEvent);
    }

    public void OnAudioEvent(MenuClickAudio audioEvent)
    {
        RuntimeManager.PlayOneShot(_clickEvent);
    }

    public void OnAudioEvent(MenuPauseAudio audioEvent)
    {
        PLAYBACK_STATE ps;
        _pauseEventInstance.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.PLAYING)
        {
            _pauseEventInstance.start();
        }


        if (audioEvent.State == MenuPauseAudio.PauseState.GameRunning)
        {
            
            _pauseEventInstance.setParameterByName("Paused", 0);
        }
        else
        {
            _pauseEventInstance.setParameterByName("Paused", 1);
        }
    }

    void Awake()
    {
        AudioController.Subscribe<MenuSelectAudio>(this);
        AudioController.Subscribe<MenuClickAudio>(this);
        AudioController.Subscribe<MenuPauseAudio>(this);
        _pauseEventInstance = RuntimeManager.CreateInstance(_pauseEvent);
    }
    void OnDestroy()
    {
        AudioController.Unsubscribe<MenuSelectAudio>(this);
        AudioController.Unsubscribe<MenuClickAudio>(this);
        AudioController.Unsubscribe<MenuPauseAudio>(this);

        PLAYBACK_STATE ps;
        _pauseEventInstance.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.STOPPED)
        {
            _pauseEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

    }

}
