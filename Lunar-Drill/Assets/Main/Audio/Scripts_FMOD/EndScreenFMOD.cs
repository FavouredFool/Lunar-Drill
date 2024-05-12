using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenFMOD : MonoBehaviour,
    IAudioSubscriber<EndSceneLunar>,
    IAudioSubscriber<EndSceneDrill>,
    IAudioSubscriber<EndSceneShing>,
    IAudioSubscriber<EndSceneGame> ,
    IAudioSubscriber<EndSceneOver>
{
    [SerializeField]
    EventReference _endSceneLunar;
    [SerializeField]
    EventReference _endSceneDrill;
    [SerializeField]
    EventReference _endSceneShing;
    [SerializeField]
    EventReference _endSceneGame;
    [SerializeField]
    EventReference _endSceneOver;

    [BankRef]
    public List<string> Banks;


    public void OnAudioEvent(EndSceneLunar audioEvent)
    {
        RuntimeManager.PlayOneShot(_endSceneLunar);
    }

    public void OnAudioEvent(EndSceneDrill audioEvent)
    {
        RuntimeManager.PlayOneShot(_endSceneDrill);

    }

    public void OnAudioEvent(EndSceneShing audioEvent)
    {
        RuntimeManager.PlayOneShot(_endSceneShing);
    }

    public void OnAudioEvent(EndSceneGame audioEvent)
    {
        RuntimeManager.PlayOneShot(_endSceneGame);
    }

    public void OnAudioEvent(EndSceneOver audioEvent)
    {
        RuntimeManager.PlayOneShot(_endSceneOver);
    }

    void Awake()
    {
        AudioController.Subscribe<EndSceneLunar>(this);
        AudioController.Subscribe<EndSceneDrill>(this);
        AudioController.Subscribe<EndSceneShing>(this);
        AudioController.Subscribe<EndSceneGame>(this);
        AudioController.Subscribe<EndSceneOver>(this);
    }

    void OnDestroy()
    {
        AudioController.Unsubscribe<EndSceneLunar>(this);
        AudioController.Unsubscribe<EndSceneDrill>(this);
        AudioController.Unsubscribe<EndSceneShing>(this);
        AudioController.Unsubscribe<EndSceneGame>(this);
        AudioController.Unsubscribe<EndSceneOver>(this);
        
    }
}
