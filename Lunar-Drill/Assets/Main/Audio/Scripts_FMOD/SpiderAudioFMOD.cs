using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SpiderAudioFMOD : MonoBehaviour, 
    IAudioSubscriber<SpiderHit>,
    IAudioSubscriber<SpiderLaserCharging>,
    IAudioSubscriber<SpiderLaserFiring>,
    IAudioSubscriber<SpiderVulnurable>
{

    [SerializeField]
    EventReference _spiderHit;
    [SerializeField]
    EventReference _spiderLaserCharging;
    [SerializeField]
    EventReference _spiderLaserFiring;
    [SerializeField]
    EventReference _spiderVulnurable;

    EventInstance _spiderLaserChargingInstance;
    EventInstance _spiderLaserFiringInstance;
    EventInstance _spiderVulnurableInstance;

    public void OnAudioEvent(SpiderHit audioEvent)
    {
        RuntimeManager.PlayOneShot(_spiderHit);
    }

    public void OnAudioEvent(SpiderLaserCharging audioEvent)
    {
        PLAYBACK_STATE ps;
        _spiderLaserChargingInstance.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.PLAYING && audioEvent.CurrentState == SpiderLaserCharging.ChargeState.ChargingStarted)
        {
            _spiderLaserChargingInstance.start();
        }
        else if(ps != PLAYBACK_STATE.STOPPED)
        {
            _spiderLaserChargingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void OnAudioEvent(SpiderLaserFiring audioEvent)
    {
        
        if (audioEvent.CurrentState == SpiderLaserFiring.LaserState.LaserFiring)
        {
            _spiderLaserFiringInstance.setParameterByName("LaserState", 1);
        }
        else
        {
            _spiderLaserFiringInstance.setParameterByName("LaserState", 0);
        }
    }

    public void OnAudioEvent(SpiderVulnurable audioEvent)
    {
        PLAYBACK_STATE ps;
        _spiderVulnurableInstance.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.PLAYING && audioEvent.CurrentState == SpiderVulnurable.VulnurableState.SpiderVulnurable)
        {
            _spiderVulnurableInstance.start();
        }
        else if (ps != PLAYBACK_STATE.STOPPED)
        {
            _spiderVulnurableInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }


    void Awake()
    {
        AudioController.Subscribe<SpiderHit>(this);
        AudioController.Subscribe<SpiderLaserCharging>(this);
        AudioController.Subscribe<SpiderLaserFiring>(this);
        AudioController.Subscribe<SpiderVulnurable>(this);


        _spiderLaserChargingInstance = RuntimeManager.CreateInstance(_spiderLaserCharging);
        _spiderLaserFiringInstance = RuntimeManager.CreateInstance(_spiderLaserFiring);
        _spiderVulnurableInstance = RuntimeManager.CreateInstance(_spiderVulnurable);
        _spiderLaserFiringInstance.start();
    }


    void OnDestroy()
    {
        AudioController.Unsubscribe<SpiderHit>(this);
        AudioController.Unsubscribe<SpiderLaserCharging>(this);
        AudioController.Unsubscribe<SpiderLaserFiring>(this);
        AudioController.Unsubscribe<SpiderVulnurable>(this);

        // Stop  audio if it is running
        PLAYBACK_STATE ps;
        _spiderLaserChargingInstance.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.STOPPED)
        {
            _spiderLaserChargingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        _spiderLaserFiringInstance.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.STOPPED)
        {
            _spiderLaserFiringInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        _spiderVulnurableInstance.getPlaybackState(out ps);
        if (ps != PLAYBACK_STATE.STOPPED)
        {
            _spiderVulnurableInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

    }
}
