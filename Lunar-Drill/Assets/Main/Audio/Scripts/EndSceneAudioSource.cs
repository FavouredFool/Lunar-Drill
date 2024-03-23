using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script attached to a sound source, will play the sounds for the end scene.
public class EndSceneAudioSource : MonoBehaviour,
    IAudioSubscriber<EndSceneGame>,
    IAudioSubscriber<EndSceneOver>,
    IAudioSubscriber<EndSceneLunar>,
    IAudioSubscriber<EndSceneDrill>,
    IAudioSubscriber<EndSceneShing>
{
    AudioSource _audioSource;


    [Header("End Screen Game")]
    [SerializeField] AudioClip _gameClip = null; // Clip that is played for "Game" Voiceline.
    [SerializeField, Range(0, 1f)] float _gameVolume = 1.0f; // Volume of "Game" voiceline
    [Header("End Screen Over")]
    [SerializeField] AudioClip _overClip = null; // Clip that is played for "Over" Voiceline.
    [SerializeField, Range(0, 1f)] float _overVolume = 1.0f; // Volume "Over" Voiceline.
    [Header("End Screen Lunar")]
    [SerializeField] AudioClip _lunarClip = null; // Clip that is played on "Lunar" Voiceline trigger.
    [SerializeField, Range(0, 1f)] float _lunarVolume = 1.0f; // Volume of Lunar voiceline
    [Header("End Screen Drill")]
    [SerializeField] AudioClip _drillClip = null; // Clip that is played on "Drill" Voiceline trigger.
    [SerializeField, Range(0, 1f)] float _drillVolume = 1.0f; // Volume of Drill Voiceline
    [Header("Audio Confirm Options")]
    [SerializeField] AudioClip _shingClip = null; // Clip that is played on End screen Slash.
    [SerializeField, Range(0, 1f)] float _shingVolume = 1.0f; // Volume of Slash

    // On awake get the audio source and subscribe to the events
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioController.Subscribe<EndSceneGame>(this);
        AudioController.Subscribe<EndSceneOver>(this);
        AudioController.Subscribe<EndSceneLunar>(this);
        AudioController.Subscribe<EndSceneDrill>(this);
        AudioController.Subscribe<EndSceneShing>(this);
    }

    // On destroy unsubscribe from the events
    private void OnDestroy()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioController.Unsubscribe<EndSceneGame>(this);
        AudioController.Unsubscribe<EndSceneOver>(this);
        AudioController.Unsubscribe<EndSceneLunar>(this);
        AudioController.Unsubscribe<EndSceneDrill>(this);
        AudioController.Unsubscribe<EndSceneShing>(this);
    }


    public void OnAudioEvent(EndSceneOver audioEvent)
    {
        _audioSource.PlayOneShot(_overClip,_overVolume);
    }

    public void OnAudioEvent(EndSceneGame audioEvent)
    {
        _audioSource.PlayOneShot(_gameClip, _gameVolume);
    }

    public void OnAudioEvent(EndSceneLunar audioEvent)
    {
        _audioSource.PlayOneShot(_lunarClip, _lunarVolume);
    }

    public void OnAudioEvent(EndSceneDrill audioEvent)
    {
        _audioSource.PlayOneShot(_drillClip, _drillVolume);
    }

    public void OnAudioEvent(EndSceneShing audioEvent)
    {
        _audioSource.PlayOneShot(_shingClip, _shingVolume);
    }
}
