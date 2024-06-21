using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform),typeof(CanvasGroup))]
public class OptionsEntry : MonoBehaviour
{
    public RectTransform rect;
    public CanvasGroup canvasGroup;
    int _lastIndex;

    Sequence _positionSequence;

    private void Awake()
    {
        if(_toggleColorImage) toggleColor = _toggleColorImage.color;
    }
    public void RefreshPosition(int _currentIndex, int _totalAmount)
    {
        if (_currentIndex == _totalAmount - 1) _currentIndex = -1;

        int diff = _currentIndex - _lastIndex;
        bool blink = Mathf.Abs(diff) > 1;

        float
            targetPos = (float)_currentIndex * 200f,
            targetAph = GetAlpha(_currentIndex, _totalAmount);

        float shiftTime = OptionsMenu.shiftTime;

        _positionSequence.Kill();
        _positionSequence = DOTween.Sequence();
        if (!blink)
        {
            Debug.Log(rect +" "+canvasGroup);
            _positionSequence.Append(rect.DOAnchorPosY(targetPos,shiftTime).SetEase(Ease.OutQuad));
            _positionSequence.Join(canvasGroup.DOFade(targetAph, shiftTime).SetEase(Ease.OutQuad));
        }
        else
        {
            float tempPos = ((float)_lastIndex + Mathf.Clamp(-diff, -1, 1)) * 200;

            _positionSequence.Append(rect.DOAnchorPosY(tempPos, shiftTime).SetEase(Ease.OutQuad).OnComplete(() 
                => {
                    rect.anchoredPosition = new Vector2(0, targetPos);
                    rect.localScale = Vector3.one;
                } ));
            _positionSequence.Join(canvasGroup.DOFade(0, shiftTime).SetEase(Ease.OutQuad));

            _positionSequence.Append(canvasGroup.DOFade(targetAph, shiftTime / 3f).SetEase(Ease.OutSine));
        }
        _positionSequence.SetUpdate(true);

        _lastIndex = _currentIndex;
    }
    public void SetPosition(int _currentIndex, int _totalAmount)
    {
        if (_currentIndex == _totalAmount - 1) _currentIndex = -1;

        float
            targetPos = (float)_currentIndex * 200f,
            targetAph = GetAlpha(_currentIndex, _totalAmount);

        rect.anchoredPosition = new Vector2(0, targetPos);
        canvasGroup.alpha = targetAph;

        _lastIndex = _currentIndex;
    }

    public float GetAlpha(int _currentIndex, int _totalAmount)
    {
        float targetAph;

        if (_currentIndex == 0)
            targetAph = 1f;
        else
        {
            _currentIndex= Mathf.Abs(_currentIndex);
            targetAph = 1 * (1 - (float)_currentIndex * (1f / (float)_totalAmount));
        }

        return targetAph;
    }

    public enum OptionType
    {
        None, MasterVolume, MusicVolume, EffectsVolume, Fullscreen, Vibration, Leave
    }
    public OptionType optionType;
    public Slider _slider;
    public Toggle _toggle;
    public Image _toggleColorImage;
    Color toggleColor;
    public bool isSlider => _slider != null;

    public void ToggleEntry()
    {
        switch (optionType)
        {
            case OptionType.MasterVolume:
            case OptionType.MusicVolume:
            case OptionType.EffectsVolume:
            case OptionType.Fullscreen:
            case OptionType.Vibration:
                _toggle.isOn = !_toggle.isOn;
                if (_toggleColorImage) _toggleColorImage.color = _toggle.isOn ? toggleColor : Color.gray;
                break;
            case OptionType.Leave:
                SceneChanger.instance?.Quit();
                break;
        }
    }
    public void SlideEntry(bool up)
    {
        switch (optionType)
        {
            case OptionType.MasterVolume:
            case OptionType.MusicVolume:
            case OptionType.EffectsVolume:
                _slider.value = Mathf.Clamp(_slider.value + 0.1f * (up ? 1 : -1), 0, 1);
                break;
        }
    }
}
