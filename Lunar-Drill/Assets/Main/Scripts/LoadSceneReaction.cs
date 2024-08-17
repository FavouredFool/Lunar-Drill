using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadSceneReaction : MonoBehaviour, IInputSubscriber<Signal_SceneChange>
{
    public enum Mode
    {
        Both, Load, Unload
    }
    public Mode mode;

    public bool keepPosition;
    public Vector3 localPosition;
    public float scale;

    Sequence seq;

    private void OnEnable()
    {
        InputBus.Subscribe(this);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe(this);
    }

    private void Start()
    {
        Vector3 pos = transform.localPosition;
        Vector3 scl = transform.localScale;

        if (mode==Mode.Unload) return;

        transform.localPosition = keepPosition ? transform.localPosition : localPosition;
        transform.localScale = scale * Vector3.one;

        float time = 0.66f;

        seq.Kill();
        seq = DOTween.Sequence();

        seq.Append(transform.DOLocalMove(pos, time).SetEase(Ease.OutSine));
        seq.Join(transform.DOScale(scl, time).SetEase(Ease.OutSine));

        seq.SetUpdate(true);
    }

    public void OnEventHappened(Signal_SceneChange e)
    {
        if (mode == Mode.Load) return;

        Return(e.delay);
    }
    public void Return(float time)
    {
        seq.Kill();
        seq = DOTween.Sequence();

        seq.Append(transform.DOLocalMove(keepPosition ? transform.localPosition : localPosition, time).SetEase(Ease.OutSine));
        seq.Join(transform.DOScale(scale, time).SetEase(Ease.OutSine));

        seq.SetUpdate(true);
    }
}
