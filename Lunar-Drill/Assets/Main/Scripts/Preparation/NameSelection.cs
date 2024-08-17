using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class NameSelection : MonoBehaviour, IInputSubscriber<TeamNameChanged>
{
    public bool isOpen { get; private set; }

    [SerializeField]
    Transform body;

    [SerializeField]
    CoopButton
        _buttonRerollAll,
        _buttonRerollLuna,
        _buttonRerollDrill,
        _buttonBack,
        _buttonNext;

    [SerializeField]
    TMP_Text _textL, _textD;

    private void OnEnable()
    {
        InputBus.Subscribe(this);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe(this);
    }

    public void Set(bool open)
    {
        isOpen = open;
        body.localScale = open ? Vector3.one : Vector3.zero;

        _buttonRerollAll.blocked = open;
        _buttonRerollDrill.blocked = open;
        _buttonRerollLuna.blocked = open;
        _buttonBack.blocked = open;
        _buttonNext.blocked = open;
    }
    public void Open(float delay = 0)
    {
        isOpen = true;

        DOTween.Kill(body);
        body.DOScale(1, 0.3f)
            .SetEase(Ease.OutSine)
            .SetDelay(delay);

        bool isSolo = PlayerConnectController.isSolo;

        _buttonRerollAll.transform.parent.gameObject.SetActive(isSolo);
        _buttonRerollDrill.transform.parent.gameObject.SetActive(!isSolo);
        _buttonRerollLuna.transform.parent.gameObject.SetActive(!isSolo);

        _buttonRerollAll.blocked = !isSolo;
        _buttonRerollDrill.blocked = isSolo;
        _buttonRerollLuna.blocked = isSolo;

        _buttonBack.blocked = false;
        _buttonNext.blocked = false;
    }
    public void Close(float delay = 0)
    {
        isOpen = false;

        DOTween.Kill(body);
        body.DOScale(0, 0.3f)
            .SetEase(Ease.InSine)
            .SetDelay(delay);

        _buttonRerollAll.blocked = true;
        _buttonRerollDrill.blocked = true;
        _buttonRerollLuna.blocked = true;

        _buttonBack.blocked = true;
        _buttonNext.blocked = true;
    }

    public void OnEventHappened(TeamNameChanged e)
    {
        _textL.text = e.LunarName;
        _textD.text = e.DrillianName;
    }
}
