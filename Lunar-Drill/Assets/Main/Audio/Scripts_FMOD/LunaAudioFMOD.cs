using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaAudioFMOD : MonoBehaviour, 
    IAudioSubscriber<LunaHitLaser>,
    IAudioSubscriber<LunaHitDrillian>,
    IAudioSubscriber<LunaEnergyPickup>,
    IAudioSubscriber<LunaLaserFiring>
{
    [SerializeField]
    private EventReference _lunaHitLaser;
    [SerializeField]
    private EventReference _lunaHitDrillian;
    [SerializeField]
    private EventReference _lunaEnergyPickup;
    [SerializeField]
    private EventReference _lunaLaserFiring;

    EventInstance _laser;

    public void OnAudioEvent(LunaHitLaser audioEvent)
    {
        RuntimeManager.PlayOneShot(_lunaHitLaser);
    }

    public void OnAudioEvent(LunaHitDrillian audioEvent)
    {
        RuntimeManager.PlayOneShot(_lunaHitDrillian);
    }

    public void OnAudioEvent(LunaEnergyPickup audioEvent)
    {
        RuntimeManager.PlayOneShot(_lunaEnergyPickup);
    }

    public void OnAudioEvent(LunaLaserFiring audioEvent)
    {
        PLAYBACK_STATE ps;
        _laser.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.PLAYING)
        {
            _laser.start();
        }
        //Debug.Log($"Laser {audioEvent.CurrentState}, Energy {audioEvent.LaserEnergyRemaining}");
        _laser.setParameterByName("Luna Laser Firing", (int)audioEvent.CurrentState);
        _laser.setParameterByName("Luna Laser Charge", audioEvent.LaserEnergyRemaining);
    }

    void Awake()
    {
        AudioController.Subscribe<LunaHitLaser>(this);
        AudioController.Subscribe<LunaHitDrillian>(this);
        AudioController.Subscribe<LunaEnergyPickup>(this);
        AudioController.Subscribe<LunaLaserFiring>(this);

        _laser = RuntimeManager.CreateInstance(_lunaLaserFiring);
    }

    void OnDestroy()
    {
        AudioController.Unsubscribe<LunaHitLaser>(this);
        AudioController.Unsubscribe<LunaHitDrillian>(this);
        AudioController.Unsubscribe<LunaEnergyPickup>(this);
        AudioController.Unsubscribe<LunaLaserFiring>(this);

        // Stop  audio if it is running
        PLAYBACK_STATE ps;
        _laser.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.STOPPED)
        {
            _laser.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
