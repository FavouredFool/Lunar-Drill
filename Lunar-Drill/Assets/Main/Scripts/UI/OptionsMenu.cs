using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Linq;

public class OptionsMenu : MonoBehaviour, IInputSubscriber<Signal_SceneChange>, IInputSubscriber<MenuMoveNorth>, IInputSubscriber<MenuMoveSouth>
{
    public static OptionsMenu instance;
    public static bool isOpen;
    bool isControlled;

    [SerializeField] GameObject _body;
    [SerializeField] CoopButton _button;
    [SerializeField] CoopButton _menuToggle,_menuToggleSlide;
    [SerializeField] Image
        _blackground,
        _background;
    [SerializeField] CanvasGroup _content;
    [SerializeField] List<OptionsEntry> _entries;
    [SerializeField] int _entryIndexShift;

    float _lastShiftTime = 0;
    public const float shiftTime = 0.33f;

    int northInput = 0, southInput = 0;
    float Scroll => Mathf.Clamp01(northInput) - Mathf.Clamp01(southInput);

    Sequence bodySequence;

    private Bus _masterBus;
    private Bus _sfxBus;
    private Bus _musicBus;

    private void OnEnable()
    {
        InputBus.Subscribe<MenuMoveNorth>(this);
        InputBus.Subscribe<MenuMoveSouth>(this);
        InputBus.Subscribe<Signal_SceneChange>(this);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe<MenuMoveNorth>(this);
        InputBus.Unsubscribe<MenuMoveSouth>(this);
        InputBus.Unsubscribe<Signal_SceneChange>(this);
    }

    public void SetUp()
    {
        SettingSaver.Load();
        _masterBus = RuntimeManager.GetBus("bus:/");
        _sfxBus = RuntimeManager.GetBus("bus:/SFX");
        _musicBus = RuntimeManager.GetBus("bus:/Music");

        _body.SetActive(false);
        isOpen = false;
        SetMenuMode(false);

        Screen.fullScreen = true;

        PopulateEntryData();
    }
    void Update()
    {
        if (Scroll != 0 && _lastShiftTime + shiftTime+shiftTime*1/3 < Time.unscaledTime)
            ChangeEntry(Scroll>0);
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }
    public void Open()
    {
        if (isControlled) return;

        AudioController.Fire(new MenuPauseAudio(MenuPauseAudio.PauseState.GamePaused));

        isOpen = true;
        _body.SetActive(true);

        isControlled = true;

        _blackground.color = Color.clear;
        _content.alpha = 0;
        _background.rectTransform.sizeDelta = new Vector2(1250, 0);

        bodySequence.Kill();
        bodySequence = DOTween.Sequence();
        bodySequence.Append(_blackground.DOFade(1, 0.33f).SetEase(Ease.InSine));
        bodySequence.Join(_content.DOFade(1, 0.1f).SetDelay(0.23f).SetEase(Ease.InSine));
        bodySequence.Join(_background.rectTransform.DOSizeDelta(new Vector2(1250,1700),0.2f).SetEase(Ease.OutSine));
        bodySequence.SetUpdate(true).OnComplete(() =>
        {
            isControlled = false;
        });

        SetMenuMode(true);

        _entryIndexShift = 0;
        for (int i = 0; i < _entries.Count; i++)
            _entries[i].SetPosition(i,_entries.Count);
    }
    public void Close()
    {
        AudioController.Fire(new MenuPauseAudio(MenuPauseAudio.PauseState.GameRunning));
        if (isControlled) return;

        isControlled = true;
        bodySequence.Kill();
        bodySequence = DOTween.Sequence();
        bodySequence.Append(_blackground.DOFade(0, 0.33f).SetEase(Ease.InSine));
        bodySequence.Join(_content.DOFade(0, 0.1f).SetEase(Ease.InSine));
        bodySequence.Join(_background.rectTransform.DOSizeDelta(new Vector2(1250, 0), 0.2f).SetEase(Ease.InSine));
        bodySequence.SetUpdate(true).OnComplete(() =>
        {
            isControlled = false;
            isOpen = false;
            _body.SetActive(false);

            SetMenuMode(false);
        });

    }

    public void SetMenuMode(bool on)
    {
        if (PlayerConnectController.Drillian)
            PlayerConnectController.Drillian.SetMenuMode(on);
        if (PlayerConnectController.Luna)
            PlayerConnectController.Luna.SetMenuMode(on);
    }

    public void PopulateEntryData()
    {
        for (int i = 0; i < _entries.Count; i++)
        {
            OptionsEntry entry = _entries[i];

            float _currentFloat = 1;
            switch (entry.optionType)
            {
                case OptionsEntry.OptionType.MasterVolume:
                    _masterBus.getVolume(out _currentFloat);
                    entry._slider.value = _currentFloat;
                    entry._slider.onValueChanged.AddListener(ChangeMasterVolume);
                    entry._toggle.isOn = !FmodUtil.getMuteMaster();
                    entry._toggle.onValueChanged.AddListener(ChangeMasterMute);
                    break;
                case OptionsEntry.OptionType.MusicVolume:
                    _masterBus.getVolume(out _currentFloat);
                    entry._slider.value = _currentFloat;
                    entry._slider.onValueChanged.AddListener(ChangeMusicVolume);
                    entry._toggle.isOn = !FmodUtil.getMuteMusic();
                    entry._toggle.onValueChanged.AddListener(ChangeMusicMute);
                    break;
                case OptionsEntry.OptionType.EffectsVolume:
                    _masterBus.getVolume(out _currentFloat);
                    entry._slider.value = _currentFloat;
                    entry._slider.onValueChanged.AddListener(ChangeEffectsVolume);
                    entry._toggle.isOn = !FmodUtil.getMuteSFX();
                    entry._toggle.onValueChanged.AddListener(ChangeEffectsMute);
                    break;
                case OptionsEntry.OptionType.Fullscreen:
                    entry._toggle.isOn = Screen.fullScreenMode != FullScreenMode.Windowed;
                    entry._toggle.onValueChanged.AddListener(ChangeFullScreen);
                    break;
                case OptionsEntry.OptionType.Vibration:
                    entry._toggle.isOn = !Rumble.rumbleDisabled;
                    entry._toggle.onValueChanged.AddListener(ChangeVibration);
                    break;
            }
        }
    }

    public void ChangeEntry(bool up)
    {
        _lastShiftTime = Time.unscaledTime;

        OptionsEntry shifter;
        if (!up)
        {
            shifter = _entries[_entries.Count - 1];
            _entries.Remove(shifter);
            _entries.Insert(0, shifter);
        }
        else
        {
            shifter = _entries[0];
            _entries.Remove(shifter);
            _entries.Add(shifter);
        }

        _menuToggle.gameObject.SetActive(!_entries[_entryIndexShift].isSlider);
        _menuToggleSlide.gameObject.SetActive(_entries[_entryIndexShift].isSlider);

        RefreshEntryPositions();
    }
    public void ToggleEntry()
    {
        _entries[_entryIndexShift].ToggleEntry();
        SettingSaver.Save();

        Rumble.instance?.RumbleFeedback();
    }
    public void SlideEntry(bool up)
    {
        _entries[_entryIndexShift].SlideEntry(up);
        SettingSaver.Save();
    }

    public void RefreshEntryPositions()
    {
        for (int i = 0; i < _entries.Count; i++)
        {
            OptionsEntry entry = _entries[i];
            entry.RefreshPosition(i,_entries.Count);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        bool allowOpen = true;
        if (SceneChanger.currentScene == SceneIdentity.PlayerConnect || SceneChanger.currentScene == SceneIdentity.Stats)
            allowOpen = false;
        _button.gameObject.SetActive(allowOpen);
    }

    public void OnEventHappened(MenuMoveNorth e)
    {
        if (e.context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
            northInput++;
        else if (e.context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled)
            northInput--;
    }
    public void OnEventHappened(MenuMoveSouth e)
    {
        if (e.context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
            southInput++;
        else if (e.context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled)
            southInput--;
    }
    public void OnEventHappened(Signal_SceneChange e)
    {
        Close();
    }

    #region Modifications

    private void ChangeFullScreen(bool on)
    {
        if (on)
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;
    }
    private void ChangeVibration(bool on)
    {
        Rumble.rumbleDisabled = !on;
        Rumble.instance?.ClearAndStopAllRumble();
        Debug.Log("X");
    }

    private void ChangeMasterMute(bool on)
    {
        FmodUtil.SetMuteMaster(!on);
    }
    private void ChangeMusicMute(bool on)
    {
        FmodUtil.SetMuteMusic(!on);
    }
    private void ChangeEffectsMute(bool on)
    {
        FmodUtil.SetMuteSFX(!on);
    }

    public void ChangeMasterVolume(float value)
    {
        _masterBus.setVolume(value);
        float _currentMasterVolume = 1;
        _masterBus.getVolume(out _currentMasterVolume);
        Debug.Log($"Master Volume: {_currentMasterVolume}");
    }
    public void ChangeMusicVolume(float value)
    {
        _musicBus.setVolume(value);
    }
    public void ChangeEffectsVolume(float value)
    {
        _sfxBus.setVolume(value);
    }

    #endregion
}
