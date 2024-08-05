using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
public class PreparationManager : MonoBehaviour
{
    [SerializeField]Transform
        _modeSelectPart,
        _nameSelectPart;
    [SerializeField]CoopButton
        _buttonSingleplayer,
        _buttonMultiplayer,
        _buttonRerollAll,
        _buttonRerollLuna,
        _buttonRerollDrill,
        _buttonBack,
        _buttonNext;
    [SerializeField] PlayerInput
        _modeInput;

    private void Start()
    {
        _modeSelectPart.localScale = Vector3.zero;
        _nameSelectPart.localScale = Vector3.zero;
        RefreshMenu(true);
    }
    public void SelectMode(bool coop)
    {
        RefreshMenu(false);

        _modeInput.gameObject.SetActive(false);

        NameManager.instance.RandomizeBoth(0);

        InputBus.Fire(new PlayerModeChanged(coop));
    }
    public void Back()
    {
        RefreshMenu(true);

        InputBus.Fire(new PlayerModeReset());

        _modeInput.gameObject.SetActive(true);
    }
    public void RefreshMenu(bool modeSelect)
    {
        DOTween.Kill(_modeSelectPart);
        _modeSelectPart.DOScale(modeSelect ? 1 : 0, 0.3f)
            .SetEase(modeSelect ? Ease.OutSine : Ease.InSine)
            .SetDelay(modeSelect ? 0.3f : 0);

        DOTween.Kill(_nameSelectPart);
        _nameSelectPart.DOScale(!modeSelect ? 1 : 0, 0.3f)
            .SetEase(!modeSelect ? Ease.OutSine : Ease.InSine)
            .SetDelay(!modeSelect ? 0.3f : 0f);

        _buttonBack.blocked = modeSelect;
        _buttonNext.blocked = modeSelect;
        _buttonRerollAll.blocked = modeSelect;
        _buttonRerollLuna.blocked = modeSelect;
        _buttonRerollDrill.blocked = modeSelect;

        _buttonSingleplayer.blocked = !modeSelect;
        _buttonMultiplayer.blocked = !modeSelect;
    }
    public void Continue()
    {
        DOTween.Kill(_nameSelectPart);
        _nameSelectPart.DOScale(0, 0.3f).SetEase(Ease.InSine);
    }

    public void OnBothInputWest(InputAction.CallbackContext context) => InputBus.Fire(new InputWest(ChosenCharacter.both, context));
    public void OnBothInputEast(InputAction.CallbackContext context) => InputBus.Fire(new InputEast(ChosenCharacter.both, context));
}
