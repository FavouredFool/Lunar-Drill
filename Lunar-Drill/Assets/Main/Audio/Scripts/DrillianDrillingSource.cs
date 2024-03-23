using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that defines 
public class DrillianDrillSource : MonoBehaviour, IAudioSubscriber<DrillianDrilling>
{
    AudioSource _audioSource;
    
    [Header("Audio Drillian Drill Options")]
    [SerializeField] AudioClip _drillingClip = null; // Clips for when Luna alarm the laser.
    [SerializeField, Range(0, 1f)] float _drillVolume = 0.5f; // Volume of alarm 

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _drillingClip;
        _audioSource.volume = _drillVolume;
        _audioSource.loop = true;
        AudioController.Subscribe<DrillianDrilling>(this);
    }

    private void OnDestroy()
    {
        AudioController.Unsubscribe<DrillianDrilling>(this);
    }

    public void OnAudioEvent(DrillianDrilling audioEvent)
    {
        if(audioEvent.CurrentState == DrillianDrilling.DrillState.DrillingStarted)
        {
            _audioSource?.Play();
        }
        else
        {
            _audioSource?.Stop();
        }
    }
}
