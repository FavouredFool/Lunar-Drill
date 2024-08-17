using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class ModeSelection : MonoBehaviour
{
    public bool isOpen { get; private set; }

    [SerializeField]
    Transform body;
    [SerializeField]
    PlayerInput
    _modeInput;
    [SerializeField]
    CoopButton
        _buttonSingleplayer,
        _buttonMultiplayer;

    public void Set(bool open)
    {
        isOpen = open;
        body.localScale = open ? Vector3.one : Vector3.zero;

        _buttonSingleplayer.blocked = open;
        _buttonMultiplayer.blocked = open;

        _modeInput.gameObject.SetActive(open);
    }
    public void Open(float delay=0)
    {
        isOpen = true;

        DOTween.Kill(body);
        body.DOScale(1, 0.3f)
            .SetEase(Ease.OutSine)
            .SetDelay(delay);

        _buttonSingleplayer.blocked = false;
        _buttonMultiplayer.blocked = false;

        _modeInput.gameObject.SetActive(true);
    }
    public void Close(float delay=0)
    {
        isOpen = false;

        DOTween.Kill(body);
        body.DOScale(0, 0.3f)
            .SetEase(Ease.InSine)
            .SetDelay(delay);

        _buttonSingleplayer.blocked = true;
        _buttonMultiplayer.blocked = true;

        _modeInput.gameObject.SetActive(false);
    }

    public void OnBothInputWest(InputAction.CallbackContext context) => InputBus.Fire(new InputWest(ChosenCharacter.both, context));
    public void OnBothInputEast(InputAction.CallbackContext context) => InputBus.Fire(new InputEast(ChosenCharacter.both, context));
}
