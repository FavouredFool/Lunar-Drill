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

    [SerializeField] private GameObject _lunaMulti1, _drillianMulti1;
    [SerializeField] private Vector3 _lunaMultiInPosition1, _drillianMultiInPosition1, _lunaMultiOutPosition1, _drillianMultiOutPosition1;

    [SerializeField] private GameObject _lunaMulti2, _drillianMulti2;
    [SerializeField] private Vector3 _lunaMultiInPosition2, _drillianMultiInPosition2, _lunaMultiOutPosition2, _drillianMultiOutPosition2;

    [SerializeField] private GameObject _lunaSolo, _drillianSolo;
    [SerializeField] private Vector3 _lunaSoloInPosition, _drillianSoloInPosition, _lunaSoloOutPosition, _drillianSoloOutPosition;

    private bool _swapped = false;
    private float _textMoveTime = 0.3f;

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

    private void Start()
    {
        _swapped = false;
    }

    public void SetEmpty()
    {
        foreach (GameObject g in SoloObjects)
            g.SetActive(false);
        foreach (GameObject g in MultiObjects)
            g.SetActive(false);
        foreach (GameObject g in EmptyObjects)
            g.SetActive(true);

        // Luna and Drillian Text
        _lunaSolo.transform.DOLocalMove(_lunaSoloOutPosition, _textMoveTime).SetUpdate(true);
        _drillianSolo.transform.DOLocalMove(_drillianSoloOutPosition, _textMoveTime).SetUpdate(true);
        _lunaMulti1.transform.DOLocalMove(_lunaMultiOutPosition1, _textMoveTime).SetUpdate(true);
        _drillianMulti1.transform.DOLocalMove(_drillianMultiOutPosition1, _textMoveTime).SetUpdate(true);
        _lunaMulti2.transform.DOLocalMove(_lunaMultiOutPosition2, _textMoveTime).SetUpdate(true);
        _drillianMulti2.transform.DOLocalMove(_drillianMultiOutPosition2, _textMoveTime).SetUpdate(true);

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

        // Luna and Drillian Text
        _lunaSolo.transform.DOLocalMove(_lunaSoloInPosition, _textMoveTime).SetUpdate(true);
        _drillianSolo.transform.DOLocalMove(_drillianSoloInPosition, _textMoveTime).SetUpdate(true);
        _lunaMulti1.transform.DOLocalMove(_lunaMultiOutPosition1, _textMoveTime).SetUpdate(true);
        _drillianMulti1.transform.DOLocalMove(_drillianMultiOutPosition1, _textMoveTime).SetUpdate(true);
        _lunaMulti2.transform.DOLocalMove(_lunaMultiOutPosition2, _textMoveTime).SetUpdate(true);
        _drillianMulti2.transform.DOLocalMove(_drillianMultiOutPosition2, _textMoveTime).SetUpdate(true);

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

        // Luna and Drillian Text
        _lunaSolo.transform.DOLocalMove(_lunaSoloOutPosition, _textMoveTime).SetUpdate(true);
        _drillianSolo.transform.DOLocalMove(_drillianSoloOutPosition, _textMoveTime).SetUpdate(true);
        if (!_swapped)
        {
            _lunaMulti1.transform.DOLocalMove(_lunaMultiInPosition1, _textMoveTime).SetUpdate(true);
            _drillianMulti1.transform.DOLocalMove(_drillianMultiInPosition1, _textMoveTime).SetUpdate(true);
            _lunaMulti2.transform.DOLocalMove(_lunaMultiOutPosition2, _textMoveTime).SetUpdate(true);
            _drillianMulti2.transform.DOLocalMove(_drillianMultiOutPosition2, _textMoveTime).SetUpdate(true);
        }
        else
        {
            _lunaMulti1.transform.DOLocalMove(_lunaMultiOutPosition1, _textMoveTime).SetUpdate(true);
            _drillianMulti1.transform.DOLocalMove(_drillianMultiOutPosition1, _textMoveTime).SetUpdate(true);
            _lunaMulti2.transform.DOLocalMove(_lunaMultiInPosition2, _textMoveTime).SetUpdate(true);
            _drillianMulti2.transform.DOLocalMove(_drillianMultiInPosition2, _textMoveTime).SetUpdate(true);
        }


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
        ReadySoloIndicator.color = magenta;
        P1Ready.SetActive(false);
        P2Ready.SetActive(false);
    }
    public void Swap()
    {
        SwapIndicator.color = Color.white;
        P1Swap.SetActive(false);
        P2Swap.SetActive(false);

        _swapped = !_swapped;
    }

    public void Tiggle(bool P1)
    {
        Vector3 force = Vector3.one * Random.Range(0.02f, 0.06f);
        if (P1)
        {
            if (Luna_Up.localScale != Vector3.zero) Luna_Up.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
            if (Luna_Up_Solo.localScale != Vector3.zero) Luna_Up_Solo.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
            if (Drillian_Up.localScale != Vector3.zero) Drillian_Up.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
            if (Drillian_Up_Solo.localScale != Vector3.zero) Drillian_Up_Solo.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
        }
        else
        {
            if (Luna_Down.localScale != Vector3.zero) Luna_Down.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
            if (Drillian_Down.localScale != Vector3.zero) Drillian_Down.DOPunchScale(force, 0.2f, 0).SetEase(Ease.InBack);
        }
    }
}
