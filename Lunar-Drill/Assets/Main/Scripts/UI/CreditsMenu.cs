using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField] RectTransform rect;

    [SerializeField] CoopButton[] menuButtons;
    [SerializeField] CoopButton backButton;

    Tween seq;

    public void Open()
    {
        Debug.Log("Open Credits");
        seq.Kill();
        seq = rect.DOAnchorPos(new Vector2(1045, 1950), 1).SetEase(Ease.InOutSine);

        foreach (CoopButton cb in menuButtons)
            cb.blocked = true;
        backButton.blocked = false;
    }
    public void Close()
    {
        seq.Kill();
        seq = rect.DOAnchorPos(new Vector2(0, 0), 1).SetEase(Ease.InOutSine);

        foreach (CoopButton cb in menuButtons)
            cb.blocked = false;
        backButton.blocked = true;
    }
}
