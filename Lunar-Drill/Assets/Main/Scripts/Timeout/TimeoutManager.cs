using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class TimeoutManager : MonoBehaviour, IInputSubscriber<Signal_AnyFire>
{
    VideoPlayer player;

    [SerializeField] private GameMenuUIManager optionalPauseMenu;
    [SerializeField] private InputActionReference 
        anyInputAction,
        forcePlayInputAction;
    [SerializeField] int returnSceneIndex;

    [SerializeField] [Range(1, 120)] float secondsUntilReplay=30;
    static float lastForcePlay;
    static float lastInputTime;
    float secondsSinceInput => Time.unscaledTime - lastInputTime;
    bool forcePlay => lastForcePlay == lastInputTime;
    bool shouldPlay => forcePlay || secondsUntilReplay < secondsSinceInput;
    bool isPlaying => player.isPlaying;

    public void PlayVideo()
    {
        Debug.Log("Play Video");
        player.gameObject.SetActive(true);
        player.Play();
    }
    public void StopVideo()
    {
        Debug.Log("Stop Video");
        player.gameObject.SetActive(false);
        player.Stop();
    }

    public void Return()
    {
        if (SceneManager.GetActiveScene().buildIndex!=returnSceneIndex)
            SceneManager.LoadScene(returnSceneIndex);
    }

    private void OnEnable()
    {
        InputBus.Subscribe(this);

        anyInputAction.action.Enable();
        anyInputAction.action.performed += OnAnyInputPerformed;

        forcePlayInputAction.action.Enable();
        forcePlayInputAction.action.performed += OnForcePlayInputPerformed;

        lastForcePlay = float.NegativeInfinity;
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe(this);

        anyInputAction.action.performed -= OnAnyInputPerformed;
        anyInputAction.action.Disable();

        forcePlayInputAction.action.performed -= OnForcePlayInputPerformed;
        forcePlayInputAction.action.Disable();
    }

    private void Awake()
    {
        player= GetComponentInChildren<VideoPlayer>();
        player.targetCamera = Camera.main;
    }
    private void Start()
    {
        StopVideo();
    }
    private void Update()
    {
        if (shouldPlay!=isPlaying)
        {
            if (shouldPlay)
            {
                PlayVideo();
                if (optionalPauseMenu)
                    optionalPauseMenu.SetOptions(true);
            }
            else
            {
                StopVideo();
                Return();
            }
        }
    }

    public void OnEventHappened(Signal_AnyFire e)
    {
        lastInputTime = Time.unscaledTime;
    }
    private void OnAnyInputPerformed(InputAction.CallbackContext context)
    {
        lastInputTime = Time.unscaledTime;
    }
    private void OnForcePlayInputPerformed(InputAction.CallbackContext context)
    {
        lastInputTime = Time.unscaledTime;
        lastForcePlay = lastInputTime;
    }
}
