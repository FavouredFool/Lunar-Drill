using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SelectScreen : MonoBehaviour
{
    //UI ELEMENTS

    public Color
        cyan,
        yellow,
        magenta,
        lilac;

    public GameObject[] EmptyObjects, SoloObjects, MultiObjects;

    public GameObject P1Ready, P1Swap;
    public GameObject P2Ready, P2Swap;

    public SpriteRenderer
        BG_Up_1, BG_Up_2, BG_Down;

    public SpriteRenderer
        ReadySoloIndicator,
        ReadyIndicator,
        SwapIndicator;

    public Transform
        Luna_Up, Drillian_Up,
        Luna_Up_Solo, Drillian_Up_Solo,
        Luna_Down, Drillian_Down;

    public void SetEmpty()
    {
        foreach (GameObject g in SoloObjects)
            g.SetActive(false);
        foreach (GameObject g in MultiObjects)
            g.SetActive(false);
        foreach (GameObject g in EmptyObjects)
            g.SetActive(true);

        //Background
        DOTween.Kill(BG_Up_1);
        DOTween.Kill(BG_Up_2);
        DOTween.Kill(BG_Down);

        BG_Up_1.DOColor(Color.black, 0.33f);
        BG_Up_2.DOColor(Color.black, 0.33f);
        BG_Down.DOColor(Color.black, 0.33f);

        //Pictures
        DOTween.Kill(Luna_Up);
        DOTween.Kill(Luna_Up_Solo);
        DOTween.Kill(Luna_Down);
        DOTween.Kill(Drillian_Up);
        DOTween.Kill(Drillian_Up_Solo);
        DOTween.Kill(Drillian_Down);

        Luna_Up.DOScale(0f, 0.1f).SetEase(Ease.InSine);
        Luna_Up_Solo.DOScale(0f, 0.1f).SetEase(Ease.InSine);
        Luna_Down.DOScale(0f, 0.1f).SetEase(Ease.InSine);
        Drillian_Up.DOScale(0f, 0.1f).SetEase(Ease.InSine);
        Drillian_Up_Solo.DOScale(0f, 0.1f).SetEase(Ease.InSine);
        Drillian_Down.DOScale(0f, 0.1f).SetEase(Ease.InSine);
    }
    public void SetSolo()
    {
        foreach (GameObject g in MultiObjects)
            g.SetActive(false);
        foreach (GameObject g in EmptyObjects)
            g.SetActive(false);
        foreach (GameObject g in SoloObjects)
            g.SetActive(true);

        //Background
        DOTween.Kill(BG_Up_1);
        DOTween.Kill(BG_Up_2);
        DOTween.Kill(BG_Down);

        BG_Up_1.DOColor(cyan, 0.33f);
        BG_Up_2.DOColor(yellow, 0.33f);
        BG_Down.DOColor(Color.black, 0.33f);

        //Pictures
        DOTween.Kill(Luna_Up);
        DOTween.Kill(Luna_Up_Solo);
        DOTween.Kill(Luna_Down);
        DOTween.Kill(Drillian_Up);
        DOTween.Kill(Drillian_Up_Solo);
        DOTween.Kill(Drillian_Down);

        Luna_Up.DOScale(0f, 0.1f).SetEase(Ease.InSine);
        Luna_Up_Solo.DOScale(0.4f, 0.33f).SetEase(Ease.OutBack).SetDelay(0.1f);
        Luna_Down.DOScale(0f, 0.1f).SetEase(Ease.InSine);
        Drillian_Up.DOScale(0f, 0.1f).SetEase(Ease.InSine);
        Drillian_Up_Solo.DOScale(0.4f, 0.33f).SetEase(Ease.OutBack).SetDelay(0.1f);
        Drillian_Down.DOScale(0f, 0.1f).SetEase(Ease.InSine);
    }
    public void SetMulti(bool Lup_Ddown)
    {
        foreach (GameObject g in SoloObjects)
            g.SetActive(false);
        foreach (GameObject g in EmptyObjects)
            g.SetActive(false);
        foreach (GameObject g in MultiObjects)
            g.SetActive(true);

        //Background
        DOTween.Kill(BG_Up_1);
        DOTween.Kill(BG_Up_2);
        DOTween.Kill(BG_Down);

        BG_Up_1.DOColor(Lup_Ddown ? cyan : yellow, 0.33f);
        BG_Up_2.DOColor(Lup_Ddown ? cyan : yellow, 0.33f);
        BG_Down.DOColor(Lup_Ddown ? yellow : cyan, 0.33f);

        //Pictures
        DOTween.Kill(Luna_Up);
        DOTween.Kill(Luna_Up_Solo);
        DOTween.Kill(Luna_Down);
        DOTween.Kill(Drillian_Up);
        DOTween.Kill(Drillian_Up_Solo);
        DOTween.Kill(Drillian_Down);

        Luna_Up_Solo.DOScale(0f, 0.1f).SetEase(Ease.InSine);
        Drillian_Up_Solo.DOScale(0f, 0.1f).SetEase(Ease.InSine);

        if (Lup_Ddown)
        {
            Luna_Up.DOScale(0.6f, 0.33f).SetEase(Ease.OutBack).SetDelay(0.1f);
            Luna_Down.DOScale(0f, 0.1f).SetEase(Ease.InSine);
            Drillian_Up.DOScale(0f, 0.1f).SetEase(Ease.InSine);
            Drillian_Down.DOScale(0.6f, 0.1f).SetEase(Ease.InSine).SetDelay(0.1f);
        }
        else
        {
            Drillian_Up.DOScale(0.6f, 0.33f).SetEase(Ease.OutBack).SetDelay(0.1f);
            Drillian_Down.DOScale(0f, 0.1f).SetEase(Ease.InSine);
            Luna_Up.DOScale(0f, 0.1f).SetEase(Ease.InSine);
            Luna_Down.DOScale(0.6f, 0.1f).SetEase(Ease.InSine).SetDelay(0.1f);
        }
    }

    public void RefreshReady(bool p1, bool p2, float time)
    {
        P1Ready.SetActive(p1);
        P2Ready.SetActive(p2);

        ReadyIndicator.color = 
            Color.Lerp(Color.white, lilac, time / ConnectManager.AgreeTime);
        ReadySoloIndicator.color = ReadyIndicator.color;
    }
    public void RefreshSwap(bool p1, bool p2, float time)
    {
        P1Swap.SetActive(p1);
        P2Swap.SetActive(p2);

        SwapIndicator.color = 
            Color.Lerp(Color.white, lilac, time / ConnectManager.AgreeTime);
    }

    public void Play()
    {
        ReadyIndicator.color = magenta;
        ReadySoloIndicator.color=magenta;
        P1Ready.SetActive(false);
        P2Ready.SetActive(false);
    }
    public void Swap()
    {
        SwapIndicator.color = Color.white;
        P1Swap.SetActive(false);
        P2Swap.SetActive(false);
    }

    public void Tiggle(bool P1)
    {
        Vector3 force = Vector3.one * Random.Range(0.02f, 0.06f);
        if (P1)
        {
            if (Luna_Up.localScale != Vector3.zero) Luna_Up.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
            if (Luna_Up_Solo.localScale != Vector3.zero) Luna_Up_Solo.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
            if (Drillian_Up.localScale!=Vector3.zero) Drillian_Up.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
            if (Drillian_Up_Solo.localScale != Vector3.zero) Drillian_Up_Solo.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
        }
        else
        {
            if (Luna_Down.localScale != Vector3.zero) Luna_Down.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
            if (Drillian_Down.localScale != Vector3.zero) Drillian_Down.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
        }
    }
}
