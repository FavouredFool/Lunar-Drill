using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHeart : MonoBehaviour
{
    [SerializeField] Image image;

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
            image.color = Color.white;
            scaleTween = transform.DOPunchScale(Vector3.one*0.33f,0.33f).SetEase(Ease.OutBack).SetUpdate(true);
        }
        else
        {
            image.color = Color.Lerp(Color.gray,Color.black,0.5f);
            scaleTween = transform.DOPunchScale(Vector3.one * 0.33f, 0.4f).SetEase(Ease.OutBounce).SetUpdate(true);
        }
    }
}
