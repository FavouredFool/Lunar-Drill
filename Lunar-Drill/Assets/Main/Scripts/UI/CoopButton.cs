using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine;
using DG.Tweening;

public class CoopButton : MonoBehaviour, IInputSubscriber<InputNorth>, IInputSubscriber<InputEast>, IInputSubscriber<InputSouth>, IInputSubscriber<InputWest>
{
    public Button.ButtonClickedEvent _OnInputPerformedEvents;

    [Header("Components")]
    [SerializeField] Image _fullBody;
    [SerializeField] Image _halfBodyDrillian, _halfBodyLuna;

    [SerializeField] Image _fullBodyBar;
    [SerializeField] Image _halfBodyDrillianBar, _halfBodyLunaBar;

    [SerializeField] Image[] _inputPrompts;

    [SerializeField] Sprite _fullAnySprite, _fullDrillianSprite, _fullLunaSprite;


    [Header("Settings")]
    public ChosenCharacter _inputCharacter;
    public enum InputDirection
    {
        Any, North, East, South, West
    }
    public InputDirection _inputRequired;
    public bool _revertRequirement;
    public float _requiredPressTime;

    [Header("Internal")]
    Tween _scaleTween;
    Tween 
        _fullBodyTween, 
        _halfBodyDrillianTween, _halfBodyLunaTween;
    public int
        _lunarWeight,
        _drillianWeight;
    public float
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

    private void OnValidate()
    {
        Refresh(_inputCharacter, _inputRequired,_revertRequirement);
    }
    private void OnEnable()
    {
        InputBus.Subscribe<InputNorth>(this);
        InputBus.Subscribe<InputEast>(this);
        InputBus.Subscribe<InputSouth>(this);
        InputBus.Subscribe<InputWest>(this);

        Refresh(_inputCharacter, _inputRequired, _revertRequirement);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe<InputNorth>(this);
        InputBus.Unsubscribe<InputEast>(this);
        InputBus.Unsubscribe<InputSouth>(this);
        InputBus.Unsubscribe<InputWest>(this);
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
    public void Refresh(ChosenCharacter character, InputDirection prompt, bool revert)
    {
        _lunarTime = 0;
        _drillianTime = 0;

        _lunarWeight = 0;
        _drillianWeight = 0;

        SetRequired(prompt,revert);
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
    public void SetRequired(InputDirection dir, bool revert)
    {
        _revertRequirement = revert;
        _inputRequired = dir;
        UpdatePrompt(InputDirection.North, InputActionPhase.Canceled);
        UpdatePrompt(InputDirection.East, InputActionPhase.Canceled);
        UpdatePrompt(InputDirection.South, InputActionPhase.Canceled);
        UpdatePrompt(InputDirection.West, InputActionPhase.Canceled);
    }

    //Visuals
    public void UpdatePrompt(InputDirection dir, InputActionPhase phase)
    {
        Image prompt = _inputPrompts[(int)dir - 1];

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

        if (_inputRequired != InputDirection.Any && (_revertRequirement? _inputRequired == dir : _inputRequired != dir))
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
    public void ProcessInput(ChosenCharacter character, InputDirection direction, InputActionPhase phase)
    {
        if (character == ChosenCharacter.any) 
            character = ChosenCharacter.both;

        if (phase != InputActionPhase.Started && phase != InputActionPhase.Canceled) return;

        Debug.Log("Input " + character.ToString() + " " + direction.ToString());

        if (_inputCharacter != ChosenCharacter.both && _inputCharacter!= ChosenCharacter.any &&
            character != ChosenCharacter.both && character != _inputCharacter) return;
        //Input is Registered!

        UpdatePrompt(direction, phase);

        if (_inputRequired != InputDirection.Any && direction != InputDirection.Any && 
            (_revertRequirement? _inputRequired == direction : _inputRequired != direction)) return;
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
    }
    public void TriggerEvents()
    {
        _OnInputPerformedEvents.Invoke();

        _scaleTween.Kill();
        _scaleTween = transform.DOPunchScale(Vector3.one*0.2f, 0.33f).SetEase(Ease.OutSine);

        _lunarWeight = 0;
        _drillianWeight = 0;

        _lunarTime = 0;
        _drillianTime = 0;
    }

    public void OnEventHappened(InputNorth e) => ProcessInput(e.character, InputDirection.North,e.context.phase);
    public void OnEventHappened(InputEast e) => ProcessInput(e.character, InputDirection.East, e.context.phase);
    public void OnEventHappened(InputSouth e) => ProcessInput(e.character, InputDirection.South, e.context.phase);
    public void OnEventHappened(InputWest e) => ProcessInput(e.character, InputDirection.West, e.context.phase);
}
