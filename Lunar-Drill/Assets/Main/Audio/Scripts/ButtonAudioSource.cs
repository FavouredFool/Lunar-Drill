using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that is attached to the Source of all Audio sources
public class ButtonAudioSource : MonoBehaviour, IAudioSubscriber<MenuSelectAudio>, IAudioSubscriber<MenuClickAudio>
{
    AudioSource _audioSource; // The audio source that should be used.
    [Header("Audio Select Options")]
    [SerializeField] AudioClip _selectedClip = null; // Clip that is played on Select.
    [SerializeField, Range(0,1f)] float _selectVolume = 1.0f; // Volume of the Select Clip
    [SerializeField] List<float> _selectPitchVariations = new(); // List of Pitch Variations; simple way to make the sound less repetitive.
    

    [Header("Audio Confirm Options")]
    [SerializeField] AudioClip _confirmClip = null; // Clip that is played on Confirm.
    [SerializeField, Range(0, 1f)] float _confirmVolume = 1.0f; // Volume of Conform Click

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioController.Subscribe<MenuSelectAudio>(this);
        AudioController.Subscribe<MenuClickAudio>(this);
    }

    public void OnAudioEvent(MenuSelectAudio audioEvent)
    {
        _audioSource.volume = _selectVolume;
        _audioSource.pitch = _selectPitchVariations[Random.Range(0,_selectPitchVariations.Count-1)];
        _audioSource.PlayOneShot(_selectedClip);

    }
    public void OnAudioEvent(MenuClickAudio audioEvent)
    {
        _audioSource.volume = _confirmVolume;
        _audioSource.PlayOneShot(_confirmClip);
    }

    private void OnDestroy()
    {
        AudioController.Unsubscribe<MenuClickAudio>(this);
        AudioController.Unsubscribe<MenuSelectAudio>(this);
    }
}
