using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaLaserAudioSource : MonoBehaviour, IAudioSubscriber<LunaLaserFiring>
{
    AudioSource _audioSource;

    [Header("Audio Luna Laser Fire Options")]
    [SerializeField] AudioClip _shootLaserClip = null; // Clips for when Luna shoots the laser.
    [SerializeField, Range(0, 1f)] float _shootLaserVolume = 1.0f; // Volume of Laser 
    [SerializeField] Vector2 _laserPitchRange = new Vector2(0.5f, 1.5f); // Pitch, first when on low energy latter on high energy

    public void OnAudioEvent(LunaLaserFiring audioEvent)
    {
        if (!_audioSource.isPlaying && audioEvent.CurrentState == LunaLaserFiring.LaserState.LaserFiring)
        {
            _audioSource.clip = _shootLaserClip;
            _audioSource.Play();
        }
        _audioSource.pitch = Mathf.Lerp(_laserPitchRange[0], _laserPitchRange[1], audioEvent.LaserEnergyRemaining);
        _audioSource.volume = _shootLaserVolume;
        
        // Logic for when theres no energy remaining
        if (audioEvent.LaserEnergyRemaining <= 0)
        {
            //for now just stop playing
            _audioSource.Stop();
        }

        if (audioEvent.CurrentState == LunaLaserFiring.LaserState.LaserStopped)
        {
            _audioSource.Stop();
        }

    }

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioController.Subscribe<LunaLaserFiring>(this);
    }

    private void OnDestroy()
    {
        AudioController.Unsubscribe<LunaLaserFiring>(this);
    }

}
