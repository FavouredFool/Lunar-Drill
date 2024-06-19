using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeManager : MonoBehaviour
{
    public static float hitFrameTime = 0.5f;
    public static TimeManager main;
    [SerializeField] GameManager gameManager;

    [SerializeField] float currentTimeScale;

    bool freeze;
    bool isPaused => freeze || OptionsMenu.isOpen || NewUndertaker.isOpen;

    Tween timeScaleTween;

    private void Awake()
    {
        main = this;
        currentTimeScale = 1;
    }

    private void Update()
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

    public void Freeze()
    {
        freeze = true;
        currentTimeScale = 0;
        Time.timeScale = 0;
        timeScaleTween.Kill();
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
