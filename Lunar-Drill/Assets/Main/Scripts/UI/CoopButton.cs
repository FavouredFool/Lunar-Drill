using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine;
using DG.Tweening;

public class CoopButton : MonoBehaviour, 
    IInputSubscriber<InputNorth>, IInputSubscriber<InputEast>, IInputSubscriber<InputSouth>, IInputSubscriber<InputWest>, 
    IInputSubscriber<Signal_SceneChange>, IInputSubscriber<Pause>,
    IInputSubscriber<MenuMoveNorth>, IInputSubscriber<MenuMoveEast>, IInputSubscriber<MenuMoveSouth>, IInputSubscriber<MenuMoveWest>
{
    public bool blocked = false;
    public Button.ButtonClickedEvent _OnInputPerformedEvents;

    [Header("Components")]
    [SerializeField] Image _fullBody;
    [SerializeField] Image _halfBodyDrillian, _halfBodyLuna;

    [SerializeField] Image _fullBodyBar;
    [SerializeField] Image _halfBodyDrillianBar, _halfBodyLunaBar;

    [SerializeField] Image _singleInputPrompt;
    [SerializeField] Image[] _inputPrompts;

    [SerializeField] Sprite _fullAnySprite, _fullDrillianSprite, _fullLunaSprite;
    [SerializeField] Sprite _pausePromptSprite;


    [Header("Settings")]
    public ChosenCharacter _inputCharacter;
    [System.Flags] public enum InputType
    {
        None=0,
        Pause=1, 
        North=2, 
        East=4, 
        South=8, 
        West=16
    }
    public InputType _inputRequired;
    public float _requiredPressTime;
    public bool _isOverlayMenu;

    public enum CompassInputType
    {
        Both, Button, Move
    }
    public CompassInputType
        _CompassInputTypeNorth,
        _CompassInputTypeEast,
        _CompassInputTypeSouth,
        _CompassInputTypeWest;


    [Header("Internal")]
    Tween _scaleTween;
    Tween 
        _fullBodyTween, 
        _halfBodyDrillianTween, _halfBodyLunaTween;
     int
        _lunarWeight,
        _drillianWeight;
     float
        _lunarTime,
        _drillianTime;

    bool LunarReady => _lunarTime >= _requiredPressTime && _lunarWeight > 0;
    bool DrillianReady => _drillianTime >= _requiredPressTime && _drillianWeight > 0;

    int RelevantWeight
    {
        get
        {
            switch (_inputCharacter)
            {
                case ChosenCharacter.drillian:
                    return _drillianWeight;
                case ChosenCharacter.luna:
                    return _lunarWeight;
                case ChosenCharacter.both:
                    return Mathf.Min(_lunarWeight, _drillianWeight);
                default:
                    return Mathf.Max(_lunarWeight, _drillianWeight);
            }
        }
    }
    float RelevantTime
    {
        get
        {
            switch (_inputCharacter)
            {
                case ChosenCharacter.drillian:
                    return _drillianTime;
                case ChosenCharacter.luna:
                    return _lunarTime;
                case ChosenCharacter.both:
                    return Mathf.Min(_lunarTime, _drillianTime);
                default:
                    return Mathf.Max(_lunarTime, _drillianTime);
            }
        }
    }
    bool RelevantReady
    {
        get
        {
            switch (_inputCharacter)
            {
                case ChosenCharacter.drillian:
                    return DrillianReady;
                case ChosenCharacter.luna:
                    return LunarReady;
                case ChosenCharacter.both:
                    return LunarReady&&DrillianReady;
                default:
                    return LunarReady || DrillianReady;
            }

        }
    }

    bool HasSingle => _inputRequired.HasFlag(InputType.Pause);
    bool HasCompass =>
        _inputRequired.HasFlag(InputType.North) ||
        _inputRequired.HasFlag(InputType.East) ||
        _inputRequired.HasFlag(InputType.South) ||
        _inputRequired.HasFlag(InputType.West);

    float
        initTime = 0,
        initBlockTime = 0.66f;
    bool initBlocked => Time.unscaledTime - initTime < initBlockTime;

    private void OnValidate()
    {
        Refresh(_inputCharacter, _inputRequired);
    }
    private void OnEnable()
    {
        initTime = Time.unscaledTime;

        InputBus.Subscribe<InputNorth>(this);
        InputBus.Subscribe<InputEast>(this);
        InputBus.Subscribe<InputSouth>(this);
        InputBus.Subscribe<InputWest>(this);

        InputBus.Subscribe<MenuMoveNorth>(this);
        InputBus.Subscribe<MenuMoveEast>(this);
        InputBus.Subscribe<MenuMoveSouth>(this);
        InputBus.Subscribe<MenuMoveWest>(this);

        InputBus.Subscribe<Pause>(this);
        InputBus.Subscribe<Signal_SceneChange>(this);

        Refresh(_inputCharacter, _inputRequired);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe<InputNorth>(this);
        InputBus.Unsubscribe<InputEast>(this);
        InputBus.Unsubscribe<InputSouth>(this);
        InputBus.Unsubscribe<InputWest>(this);

        InputBus.Unsubscribe<MenuMoveNorth>(this);
        InputBus.Unsubscribe<MenuMoveEast>(this);
        InputBus.Unsubscribe<MenuMoveSouth>(this);
        InputBus.Unsubscribe<MenuMoveWest>(this);

        InputBus.Unsubscribe<Pause>(this);
        InputBus.Unsubscribe<Signal_SceneChange>(this);

    }

    private void Update()
    {
        if (_requiredPressTime > 0)
        {
            if (_lunarWeight > 0) _lunarTime += Time.unscaledDeltaTime;
            else _lunarTime -= Time.unscaledDeltaTime * 2f;

            if (_drillianWeight > 0) _drillianTime += Time.unscaledDeltaTime;
            else _drillianTime -= Time.unscaledDeltaTime * 2f;


            _lunarTime = Mathf.Clamp(_lunarTime, 0, _requiredPressTime);
            _drillianTime = Mathf.Clamp(_drillianTime, 0, _requiredPressTime);


            if (_inputCharacter == ChosenCharacter.both)
            {
                _halfBodyDrillianBar.fillAmount = _drillianTime / _requiredPressTime;
                _halfBodyLunaBar.fillAmount = _lunarTime / _requiredPressTime;
            }
            else
            {
                _fullBodyBar.fillAmount = RelevantTime / _requiredPressTime;
            }
        }
        
        if (RelevantReady)
            TriggerEvents();
    }

    //Used to set things up. Should likely not be changed at runtime
    public void Refresh(ChosenCharacter character) => Refresh(character, _inputRequired);
    public void Refresh(InputType prompt) => Refresh(_inputCharacter, prompt);
    public void Refresh(ChosenCharacter character, InputType prompt)
    {
        _lunarTime = 0;
        _drillianTime = 0;

        _lunarWeight = 0;
        _drillianWeight = 0;

        _halfBodyDrillianBar.fillAmount = 0;
        _halfBodyLunaBar.fillAmount = 0;
        _fullBodyBar.fillAmount = 0;

        SetRequired(prompt);
        SetCharacter(character);
    }
    public void SetCharacter(ChosenCharacter ply)
    {
        _inputCharacter = ply;

        _halfBodyDrillian.gameObject.SetActive(ply == ChosenCharacter.both);
        _halfBodyLuna.gameObject.SetActive(ply == ChosenCharacter.both);
        _fullBody.gameObject.SetActive(ply != ChosenCharacter.both);

        _fullBody.sprite = ply==ChosenCharacter.any? _fullAnySprite : (ply == ChosenCharacter.drillian ? _fullDrillianSprite : _fullLunaSprite);

        UpdateBody();
    }
    public void SetRequired(InputType inp)
    {
        _inputRequired = inp;
        UpdatePrompt(InputType.Pause, InputActionPhase.Canceled);
        UpdatePrompt(InputType.North, InputActionPhase.Canceled);
        UpdatePrompt(InputType.East, InputActionPhase.Canceled);
        UpdatePrompt(InputType.South, InputActionPhase.Canceled);
        UpdatePrompt(InputType.West, InputActionPhase.Canceled);
    }

    //Visuals
    public void UpdatePrompt(InputType inp, InputActionPhase phase)
    {
        Image prompt;
        if (inp.HasFlag(InputType.Pause))
        {
            prompt = _singleInputPrompt;
            prompt.sprite = _pausePromptSprite;
        }
        else if (inp.HasFlag(InputType.North))
            prompt = _inputPrompts[0];
        else if (inp.HasFlag(InputType.East))
            prompt = _inputPrompts[1];
        else if (inp.HasFlag(InputType.South))
            prompt = _inputPrompts[2];
        else if (inp.HasFlag(InputType.West))
            prompt = _inputPrompts[3];
        else return;

        prompt.gameObject.SetActive(prompt == _singleInputPrompt ? HasSingle : HasCompass);

        if (!prompt.gameObject.activeSelf) return;

        switch (phase)
        {
            case InputActionPhase.Started:
                prompt.transform.localScale = Vector3.one * 0.95f;
                prompt.color = Color.Lerp(Color.white,Color.black,0.2f);
                break;
            case InputActionPhase.Canceled:
                prompt.transform.localScale = Vector3.one;
                prompt.color = Color.white;
                break;
            default:
                break;
        }

        if (!_inputRequired.HasFlag(inp))
        {
            prompt.color = Color.black;
            prompt.transform.localScale *= 0.75f;
        }
    }
    public void UpdateBody()
    {
        if (_inputCharacter == ChosenCharacter.both)
        {
            _halfBodyDrillianTween.Kill();
            _halfBodyDrillianTween =
                _halfBodyDrillian.transform.DOScale(Vector3.one * (_drillianWeight > 0 ? 0.8f : 1f), 0.1f).SetEase(Ease.InSine);

            _halfBodyLunaTween.Kill();
            _halfBodyLunaTween =
                _halfBodyLuna.transform.DOScale(Vector3.one * (_lunarWeight > 0 ? 0.8f : 1f), 0.1f).SetEase(Ease.InSine);
        }
        else
        {
            _fullBodyTween.Kill();
            _fullBodyTween =
                _fullBody.transform.DOScale(Vector3.one * (RelevantWeight > 0 ? 0.8f : 1f), 0.1f).SetEase(Ease.InSine);
        }
    }

    //Logic
    public virtual bool ProcessInput(ChosenCharacter character, InputType inp, InputAction.CallbackContext context)
        =>ProcessInput(character,inp,context.phase);
    public virtual bool ProcessInput(ChosenCharacter character, InputType inp, InputActionPhase phase)
    {
        if ((blocked || initBlocked) && phase != InputActionPhase.Canceled) return false; //Allow when cancelled

        if (OptionsMenu.isOpen && !_isOverlayMenu) return false;

        if (character == ChosenCharacter.any) 
            character = ChosenCharacter.both;

        if (phase != InputActionPhase.Started && phase != InputActionPhase.Canceled) return false;

        if (_inputCharacter != ChosenCharacter.both && _inputCharacter!= ChosenCharacter.any &&
            character != ChosenCharacter.both && character != _inputCharacter) return false;
        //Input is Registered!

        UpdatePrompt(inp, phase);

        if (!_inputRequired.HasFlag(inp)) return false;
        //Input is Relevant!

        bool on = phase == InputActionPhase.Started;
        switch (character) //Add weight
        {
            case ChosenCharacter.drillian:
                _drillianWeight += on ? 1 : -1;
                _drillianTime += 0.1f;
                break;
            case ChosenCharacter.luna:
                _lunarWeight += on ? 1 : -1;
                _lunarTime += 0.1f;
                break;
            default:
                _drillianWeight += on ? 1 : -1;
                _lunarWeight += on ? 1 : -1;
                _drillianTime += 0.1f;
                _lunarTime += 0.1f;
                break;
        }
        _drillianWeight = Mathf.Max(0, _drillianWeight);
        _lunarWeight = Mathf.Max(0, _lunarWeight);

        UpdateBody();

        return true;
    }
    public virtual void TriggerEvents()
    {
        _OnInputPerformedEvents.Invoke();

        transform.localScale = Vector3.one;
        _scaleTween.Kill();
        _scaleTween = transform.DOPunchScale(Vector3.one * 0.2f, 0.33f).SetEase(Ease.OutSine);

        _lunarWeight = 0;
        _drillianWeight = 0;

        _lunarTime = 0;
        _drillianTime = 0;
    }

    public void OnEventHappened(Signal_SceneChange e)
    {
        initTime = Time.unscaledTime;
        initBlockTime = e.delay*1.1f;
    }

    public void OnEventHappened(Pause e) => ProcessInput(ChosenCharacter.any, InputType.Pause, e.context.phase);

    public void OnEventHappened(InputNorth e)
    {
        if (_CompassInputTypeNorth != CompassInputType.Move) ProcessInput(e.character, InputType.North, e.context.phase);
    }
    public virtual void OnEventHappened(MenuMoveNorth e)
    {
        if (_CompassInputTypeNorth!=CompassInputType.Button) ProcessInput(ChosenCharacter.any, InputType.North, e.context.phase);
    }

    public void OnEventHappened(InputEast e)
    {
        if (_CompassInputTypeEast != CompassInputType.Move) ProcessInput(e.character, InputType.East, e.context.phase);
    }
    public virtual void OnEventHappened(MenuMoveEast e)
    {
        if (_CompassInputTypeEast != CompassInputType.Button) ProcessInput(ChosenCharacter.any, InputType.East, e.context.phase);
    }

    public void OnEventHappened(InputSouth e)
    {
        if (_CompassInputTypeSouth != CompassInputType.Move) ProcessInput(e.character, InputType.South, e.context.phase);
    }
    public virtual void OnEventHappened(MenuMoveSouth e)
    {
        if (_CompassInputTypeSouth != CompassInputType.Button) ProcessInput(ChosenCharacter.any, InputType.South, e.context.phase);
    }

    public void OnEventHappened(InputWest e)
    {
        if (_CompassInputTypeWest != CompassInputType.Move) ProcessInput(e.character, InputType.West, e.context.phase);
    }
    public virtual void OnEventHappened(MenuMoveWest e)
    {
        if(_CompassInputTypeWest != CompassInputType.Button) ProcessInput(ChosenCharacter.any, InputType.West, e.context.phase);
    }
}
