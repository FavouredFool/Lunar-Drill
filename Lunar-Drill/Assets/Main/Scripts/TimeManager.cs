using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeManager : MonoBehaviour
{
    public static float hitFrameTime = 0.5f;
    public static TimeManager main;
    [SerializeField] OptionsMenuUtilities optionsMenu;
    [SerializeField] GameManager gameManager;
    [SerializeField] Undertaker undertaker;

    [SerializeField] float currentTimeScale;

    bool isPaused => optionsMenu._isOpen||gameManager.inCutscene||undertaker.isActive;

    Tween timeScaleTween;

    private void Awake()
    {
        main = this;
        currentTimeScale = 1;
    }

    private void FixedUpdate()
    {
        RefreshTimeScale();
    }

    public void RefreshTimeScale()
    {
        Time.timeScale = isPaused ? 0 : currentTimeScale;
    }

    public void HitFrame()
    {
        timeScaleTween.Kill();
        currentTimeScale = 0;
        RefreshTimeScale();
        timeScaleTween =
            DOTween.To(() => currentTimeScale, value => { currentTimeScale = value; RefreshTimeScale(); }, 1, hitFrameTime)
            .SetEase(Ease.InExpo)
            .SetUpdate(true)
            .OnUpdate(() => RefreshTimeScale());
    }
}
