using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class TutorialSpeechBubble : MonoBehaviour
{
    public bool IsOpen { get; private set; }

    public ChosenCharacter character;
    [SerializeField] TMP_Text text;

    Tween twn;

    public void Display(TutorialEntry entry)
    {
        twn.Kill();
        if (IsOpen) twn = transform.DOPunchScale(Vector3.one * 1.1f, 0.25f);
        else twn = transform.DOScale(1,0.33f).SetEase(Ease.OutBack);

        IsOpen = true;

        text.text = entry.message;
    }
    public void Hide()
    {
        twn.Kill();
        if (IsOpen) twn = transform.DOScale(0, 0.33f).SetEase(Ease.InSine);

        IsOpen = false;
    }
}
