using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Linq;

public class OptionsMenu : MonoBehaviour
{
    public static bool isOpen;
    bool isControlled;

    [SerializeField] GameObject _body;
    [SerializeField] Image
        _blackground,
        _background;
    [SerializeField] List<OptionsEntry> _entries;
    [SerializeField] int _entryIndexShift;
    float _lastShiftTime = 0;
    public const float shiftTime = 0.33f;

    Sequence bodySequence;

    private Bus _masterBus;
    private VCA _sfxVCA;
    private VCA _musicVCA;

    private void Start()
    {
        SettingSaver.Load();
        _masterBus = RuntimeManager.GetBus("bus:/");
        _sfxVCA = RuntimeManager.GetVCA("vca:/SFX");
        _musicVCA = RuntimeManager.GetVCA("vca:/Music");

        _body.SetActive(false);
        isOpen = false;
        SetMenuMode(false);

        PopulateEntryData();
    }
    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }
    public void Open()
    {
        if (isControlled) return;

        isOpen = true;
        _body.SetActive(true);

        isControlled = true;

        _blackground.color = Color.clear;
        _background.rectTransform.sizeDelta = new Vector2(1250, 0);

        bodySequence.Kill();
        bodySequence = DOTween.Sequence();
        bodySequence.Append(_blackground.DOFade(1, 0.33f).SetEase(Ease.InSine));
        bodySequence.Join(_background.rectTransform.DOSizeDelta(new Vector2(1250,1700),0.2f).SetEase(Ease.OutSine));
        bodySequence.OnComplete(() =>
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
        if (isControlled) return;

        isControlled = true;
        bodySequence.Kill();
        bodySequence = DOTween.Sequence();
        bodySequence.Append(_blackground.DOFade(0, 0.33f).SetEase(Ease.InSine));
        bodySequence.Join(_background.rectTransform.DOSizeDelta(new Vector2(1250, 0), 0.2f).SetEase(Ease.InSine));
        bodySequence.OnComplete(() =>
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
                    break;
                case OptionsEntry.OptionType.MusicVolume:
                    _masterBus.getVolume(out _currentFloat);
                    entry._slider.value = _currentFloat;
                    entry._slider.onValueChanged.AddListener(ChangeMusicVolume);
                    break;
                case OptionsEntry.OptionType.EffectsVolume:
                    _masterBus.getVolume(out _currentFloat);
                    entry._slider.value = _currentFloat;
                    entry._slider.onValueChanged.AddListener(ChangeEffectsVolume);
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
        if (_lastShiftTime + shiftTime < Time.unscaledTime)
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

            RefreshEntryPositions();
        }
    }
    public void ModifyEntry(bool up)
    {
        _entries[_entryIndexShift].ModifyEntry(up);
    }

    public void RefreshEntryPositions()
    {
        for (int i = 0; i < _entries.Count; i++)
        {
            OptionsEntry entry = _entries[i];
            entry.RefreshPosition(i,_entries.Count);
        }
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
        Rumble.main?.ClearAndStopAllRumble();
        Debug.Log("X");
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
        _musicVCA.setVolume(value);
    }
    public void ChangeEffectsVolume(float value)
    {
        _sfxVCA.setVolume(value);
    }

    #endregion
}