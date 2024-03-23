using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillianAudioSource : MonoBehaviour, IAudioSubscriber<OreCrackedAudioEvent>, IAudioSubscriber<DrillianHitLaser>, IAudioSubscriber<DrillianHitSpider>, IAudioSubscriber<DrillianChangeMode>
{
    AudioSource _audioSource;

    [Header("Audio Ore Cracked Options")]
    [SerializeField] List<AudioClip> _oreCrackedClipPitches = null; // Clips for ore crack is played on crack.
    [SerializeField, Range(0, 1f)] float _oreCrackedVolume = 1.0f; // Volume of Crack Click
    [Header("Audio Laser Hit  Options")]
    [SerializeField] AudioClip _hitByLaserClip = null; // Clips for when Drillian is hit by the laser.
    [SerializeField, Range(0, 1f)] float _hitByLaserVolume = 1.0f; // Volume of Laser 
    [Header("Audio Hit by  Spider Options")]
    [SerializeField] AudioClip _hitBySpiderClip = null; // Clips for when Drillian takes damage when hitting the spider.
    [SerializeField, Range(0, 1f)] float _hitBySpiderVolume = 1.0f; // Volume of Spider hit
    [Header("Audio Change Mode")]
    [SerializeField] AudioClip _changingModeClip = null; // Clips for when Drillian takes damage when hitting the spider.
    [SerializeField, Range(0, 1f)] float _changingModeVolume = 0.5f; // Volume of Spider hit


    public void OnAudioEvent(OreCrackedAudioEvent audioEvent)
    {
        _audioSource.PlayOneShot(_oreCrackedClipPitches[Mathf.Min(audioEvent.PitchNumber,_oreCrackedClipPitches.Count)], _oreCrackedVolume);
    }

    public void OnAudioEvent(DrillianHitLaser audioEvent)
    {
        _audioSource.PlayOneShot(_hitByLaserClip, _hitByLaserVolume);
    }

    public void OnAudioEvent(DrillianHitSpider audioEvent)
    {
        _audioSource.PlayOneShot(_hitBySpiderClip,_hitBySpiderVolume);
    }

    public void OnAudioEvent(DrillianChangeMode audioEvent)
    {
        _audioSource.PlayOneShot(_changingModeClip, _changingModeVolume);
    }

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioController.Subscribe<OreCrackedAudioEvent>(this);
        AudioController.Subscribe<DrillianHitSpider>(this);
        AudioController.Subscribe<DrillianHitLaser>(this);
        AudioController.Subscribe<DrillianChangeMode>(this);
    }

    private void OnDestroy()
    {
        AudioController.Unsubscribe<OreCrackedAudioEvent>(this);
        AudioController.Unsubscribe<DrillianHitLaser>(this);
        AudioController.Unsubscribe<DrillianChangeMode>(this);
    }

}
