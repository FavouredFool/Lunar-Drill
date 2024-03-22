using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaAudioSource : MonoBehaviour, IAudioSubscriber<LunaHitLaser>, IAudioSubscriber<LunaHitDrillian>
{
    AudioSource _audioSource;

    [Header("Audio Laser Hit  Options")]
    [SerializeField] AudioClip _hitByLaserClip = null; // Clips for when Luna is hit by the laser.
    [SerializeField, Range(0, 1f)] float _hitByLaserVolume = 1.0f; // Volume of Laser 
    [Header("Audio Hit by  Spider Options")]
    [SerializeField] AudioClip _hitByDrillianClip = null; // Clips for when Luna takes damage caused by Drillian.
    [SerializeField, Range(0, 1f)] float _hitByDrillianVolume = 1.0f; // Volume of Spider hit



    public void OnAudioEvent(LunaHitLaser audioEvent)
    {
        _audioSource.PlayOneShot(_hitByLaserClip, _hitByLaserVolume);
    }

    public void OnAudioEvent(LunaHitDrillian audioEvent)
    {
        _audioSource.PlayOneShot(_hitByDrillianClip,_hitByDrillianVolume);
    }

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioController.Subscribe<LunaHitLaser>(this);
        AudioController.Subscribe<LunaHitDrillian>(this);
    }

    private void OnDestroy()
    {
        AudioController.Unsubscribe<LunaHitLaser>(this);
        AudioController.Unsubscribe<LunaHitDrillian>(this);
    }

}
