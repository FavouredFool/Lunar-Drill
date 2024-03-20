using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHeart : MonoBehaviour
{
    [SerializeField] Image spriteRenderer;

    bool _on;

    Tween scaleTween;

    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }

    public void Toggle(bool on)
    {
        if (_on == on) return;

        scaleTween.Kill();

        if (on)
        {
            scaleTween = transform.DOScale(1,0.33f).SetEase(Ease.OutBack).SetUpdate(true);
        }
        else
        {
            scaleTween = transform.DOScale(1, 0.2f).SetEase(Ease.InBounce).SetUpdate(true);
        }
    }
}
