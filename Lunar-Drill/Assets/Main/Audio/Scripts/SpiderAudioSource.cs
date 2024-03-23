using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAudioSource : MonoBehaviour, IAudioSubscriber<SpiderHit>, IAudioSubscriber<SpiderLaserFiring>, IAudioSubscriber<SpiderLaserCharging>
{
    AudioSource _audioSource;

    [Header("Audio Hit  Options")]
    [SerializeField] AudioClip _hitClip = null; // Clips for when Spider is hit by the laser.
    [SerializeField, Range(0, 1f)] float _hitVolume = 1.0f; // Volume of Laser 

    [Header("Audio Spider Laser Fire Options")]
    [SerializeField] AudioClip _shootLaserClip = null; // Clips for when Luna shoots the laser.
    [SerializeField, Range(0, 1f)] float _shootLaserVolume = 1.0f; // Volume of Laser 
    [SerializeField] Vector2 _laserPitchRange = new Vector2(0.5f, 1.5f); // Pitch, first when on low energy latter on high energy
    [Header("Audio Spider Laser Charge Options")]
    [SerializeField] AudioClip _chargeClip = null; // Clips for when Luna shoots the laser.
    [SerializeField, Range(0, 1f)] float _chargeVolume = 1.0f; // Volume of Laser 

    public void OnAudioEvent(SpiderHit audioEvent)
    {
        _audioSource.PlayOneShot(_hitClip, _hitVolume);
    }

    public void OnAudioEvent(SpiderLaserFiring audioEvent)
    {
        if (!_audioSource.isPlaying && audioEvent.CurrentState == SpiderLaserFiring.LaserState.LaserFiring)
        {
            _audioSource.clip = _shootLaserClip;
            _audioSource.volume = _hitVolume;
            _audioSource.Play();
        }
        if (audioEvent.CurrentState == SpiderLaserFiring.LaserState.LaserStopped)
        {
            _audioSource.Stop(); // should be a fadeout but this i okay for now
        }

    }
    public void OnAudioEvent(SpiderLaserCharging audioEvent)
    {
        if (!_audioSource.isPlaying && audioEvent.CurrentState == SpiderLaserCharging.ChargeState.ChargingStarted)
        {
            _audioSource.clip = _chargeClip;
            _audioSource.volume = _chargeVolume;
            _audioSource.Play();
        }
        if (audioEvent.CurrentState == SpiderLaserCharging.ChargeState.ChargingStopped)
        {
            _audioSource.Stop(); // should be a fadeout but this okay for now
        }

    }


    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioController.Subscribe<SpiderHit>(this);
        AudioController.Subscribe<SpiderLaserFiring>(this);
        AudioController.Subscribe<SpiderLaserCharging>(this);
    }

    private void OnDestroy()
    {
        AudioController.Unsubscribe<SpiderHit>(this);
        AudioController.Unsubscribe<SpiderLaserFiring>(this);
        AudioController.Unsubscribe<SpiderLaserCharging>(this);
    }


}
