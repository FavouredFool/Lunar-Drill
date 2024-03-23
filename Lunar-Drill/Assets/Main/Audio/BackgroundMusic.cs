using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] AudioSource _backgroundAudioSource;
    [SerializeField] AudioClip _lobbyTrack, _gameTrack;

    private static BackgroundMusic _instance;
    public static BackgroundMusic Instance { get => _instance; }

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
        // Singelton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

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
