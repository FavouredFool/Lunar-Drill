using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that defines 
public class SpiderAlarmSource : MonoBehaviour, IAudioSubscriber<SpiderVulnurable>
{
    AudioSource _audioSource;
    
    [Header("Audio Spider Alarm Options")]
    [SerializeField] AudioClip _alarmClip = null; // Clips for when Luna alarm the laser.
    [SerializeField, Range(0, 1f)] float _alarmVolume = 0.5f; // Volume of alarm 

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _alarmClip;
        _audioSource.volume = _alarmVolume;
        _audioSource.loop = true;
        AudioController.Subscribe<SpiderVulnurable>(this);
    }

    private void OnDestroy()
    {
        AudioController.Unsubscribe<SpiderVulnurable>(this);
    }

    public void OnAudioEvent(SpiderVulnurable audioEvent)
    {
        if(audioEvent.CurrentState == SpiderVulnurable.VulnurableState.SpiderVulnurable)
        {
            _audioSource?.Play();
        }
        else
        {
            _audioSource?.Stop();
        }
    }
}
