using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndertakerDisable : MonoBehaviour, IInputSubscriber<Signal_Undertaker>
{
    private void Awake()
    {
        InputBus.Subscribe(this);
    }
    private void OnDestroy()
    {
        InputBus.Unsubscribe(this);
    }

    public void OnEventHappened(Signal_Undertaker e)
    {
        gameObject.SetActive(e.targetObject == gameObject);
    }
}

public class Signal_Undertaker : IInputSignal
{
    public bool isPlayer;
    public GameObject targetObject;

    public Signal_Undertaker(bool win, GameObject obj)
    {
        isPlayer = win;
        targetObject = obj;
    }
}