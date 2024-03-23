using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHeart : MonoBehaviour
{
    [SerializeField] Image spriteRenderer;

    bool _on = false;

    Tween scaleTween;

    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }

    public void Toggle(bool on)
    {
        if (_on == on) return;

        _on = on;

        scaleTween.Kill();

        if (on)
        {
            scaleTween = transform.DOScale(1,0.33f).SetEase(Ease.OutBack).SetUpdate(true);
        }
        else
        {
            scaleTween = transform.DOScale(1.5f, 0.4f).SetEase(Ease.OutBounce).SetUpdate(true)
                .OnComplete(() =>
                {
                    scaleTween = transform.DOScale(0, 0.2f).SetEase(Ease.InBack).SetUpdate(true);
                });
        }
    }
}
