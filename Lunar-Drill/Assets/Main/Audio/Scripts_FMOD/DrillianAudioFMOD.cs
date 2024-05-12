using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DrillianAudioFMOD : MonoBehaviour, 
    IAudioSubscriber<OreCrackedAudioEvent>,
    IAudioSubscriber<DrillianHitLaser>,
    IAudioSubscriber<DrillianHitSpider>,
    IAudioSubscriber<DrillianChangeMode>,
    IAudioSubscriber<DrillianDrilling>
{
    [SerializeField]
    EventReference OreCracked = new();
    [SerializeField]
    EventReference DrillianHitLaser = new();
    [SerializeField]
    EventReference DrillianHitSpider = new();
    [SerializeField]
    EventReference DrillianChangeMode = new(); 
    [SerializeField]
    EventReference DrillianDrilling = new();

    EventInstance _drilling;


    public void OnAudioEvent(OreCrackedAudioEvent audioEvent)
    {
        EventInstance ore_cracked = RuntimeManager.CreateInstance(OreCracked);
        ore_cracked.setParameterByName("PitchNumber", Mathf.Min(audioEvent.PitchNumber,5));
        ore_cracked.start();
        ore_cracked.release();
    }

    public void OnAudioEvent(DrillianHitLaser audioEvent)
    {
        RuntimeManager.PlayOneShot(DrillianHitLaser);


    }

    public void OnAudioEvent(DrillianHitSpider audioEvent)
    {
        RuntimeManager.PlayOneShot(DrillianHitSpider);

    }

    public void OnAudioEvent(DrillianChangeMode audioEvent)
    {
        RuntimeManager.PlayOneShot(DrillianChangeMode);
    }

    public void OnAudioEvent(DrillianDrilling audioEvent)
    {
        PLAYBACK_STATE ps;
        _drilling.getPlaybackState(out ps);
        if ( ps != PLAYBACK_STATE.PLAYING)
        {
            _drilling.start();
        }
        _drilling.setParameterByName("Drillian Drill State", (int)audioEvent.CurrentState);
    }

    // On awake Subscribe to the events
    void Awake()
    {
        AudioController.Subscribe<OreCrackedAudioEvent>(this);
        AudioController.Subscribe<DrillianHitSpider>(this);
        AudioController.Subscribe<DrillianHitLaser>(this);
        AudioController.Subscribe<DrillianChangeMode>(this);
        AudioController.Subscribe<DrillianDrilling>(this);

        _drilling = RuntimeManager.CreateInstance(DrillianDrilling);

    }

    private void OnDestroy()
    {
        AudioController.Unsubscribe<OreCrackedAudioEvent>(this);
        AudioController.Unsubscribe<DrillianHitSpider>(this);
        AudioController.Unsubscribe<DrillianHitLaser>(this);
        AudioController.Unsubscribe<DrillianChangeMode>(this);
        AudioController.Unsubscribe<DrillianDrilling>(this);

        // Stop  audio if it is running
        PLAYBACK_STATE ps;
        _drilling.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.STOPPED)
        {
            _drilling.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
