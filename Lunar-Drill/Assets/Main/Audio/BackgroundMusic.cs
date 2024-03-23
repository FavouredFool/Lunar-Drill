using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] AudioSource _backgroundAudioSource;
    [SerializeField] AudioClip _lobbyTrack, _gameTrack;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "MainMenuScene")
        {
            _backgroundAudioSource.clip = _lobbyTrack;
        }

        if(scene.name == "MainScene")
        {
            _backgroundAudioSource.clip = _gameTrack;
        }

        if (!_backgroundAudioSource.isPlaying)
        {
            _backgroundAudioSource.Play();
        }

      
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (SceneManager.GetActiveScene().name == "MainMenuScene" || SceneManager.GetActiveScene().name == "SelectScreen")
        {
            _backgroundAudioSource.clip = _lobbyTrack;
        }

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            _backgroundAudioSource.clip = _gameTrack;
        }

        if (!_backgroundAudioSource.isPlaying)
        {
            _backgroundAudioSource.Play();
        }
    }
}
