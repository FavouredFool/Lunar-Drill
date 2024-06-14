using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

//DontDestroyOnLoad via Persistent Prefab
public class PreparationInterface : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] TMP_Text _teamNameText;

    [SerializeField] RectTransform _lunaTrans, _drillianTrans;
    [SerializeField] RectTransform _lowerBar;
    [SerializeField] CoopButton _continueButton;

    Sequence _seq;

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        SetScene(SceneIdentity.MainMenu,0f);
    }

    public float SetScene(SceneIdentity scene, float t=0.66f)
    {
        //animator.SetInteger("State", state);

        _seq.Kill();
        _seq= DOTween.Sequence();

        switch (scene)
        {
            case SceneIdentity.MainMenu:

                _seq.Append(_lowerBar.DOAnchorPosY(-100, t).SetEase(Ease.OutSine));
                _seq.Join(_teamNameText.rectTransform.DOAnchorPosY(0, t).SetEase(Ease.InOutSine));
                _seq.Join(_teamNameText.rectTransform.DOScale(0, t).SetEase(Ease.OutBack));

                _seq.Join(_lunaTrans.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(_lunaTrans.DOAnchorPos(new Vector2(0, 250), t).SetEase(Ease.InOutSine));

                _seq.Join(_drillianTrans.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(_drillianTrans.DOAnchorPos(new Vector2(0, 250), t).SetEase(Ease.InOutSine));

                _continueButton.blocked = true;

                break;
            case SceneIdentity.PlayerConnect:

                _seq.Append(_lowerBar.DOAnchorPosY(100, t).SetEase(Ease.OutSine));
                _seq.Join(_teamNameText.rectTransform.DOAnchorPosY(0, t).SetEase(Ease.InOutSine));
                _seq.Join(_teamNameText.rectTransform.DOScale(1, t).SetEase(Ease.OutBack));

                _seq.Join(_lunaTrans.DOScale(1f, t).SetEase(Ease.OutSine));
                _seq.Join(_lunaTrans.DOAnchorPos(new Vector2(0, 0), t).SetEase(Ease.InOutSine));

                _seq.Join(_drillianTrans.DOScale(1f, t).SetEase(Ease.OutSine));
                _seq.Join(_drillianTrans.DOAnchorPos(new Vector2(0, 0), t).SetEase(Ease.InOutSine));

                _continueButton.blocked = false;

                break;
            case SceneIdentity.PlayerSelect:

                _seq.Append(_lowerBar.DOAnchorPosY(100, t).SetEase(Ease.OutSine));
                _seq.Join(_teamNameText.rectTransform.DOAnchorPosY(450, t).SetEase(Ease.InOutSine));
                _seq.Join(_teamNameText.rectTransform.DOScale(1, t).SetEase(Ease.OutBack));

                _seq.Join(_lunaTrans.DOScale(1.5f, t).SetEase(Ease.OutSine));
                _seq.Join(_lunaTrans.DOAnchorPos(new Vector2(45, -33), t).SetEase(Ease.InOutSine));

                _seq.Join(_drillianTrans.DOScale(1.5f, t).SetEase(Ease.OutSine));
                _seq.Join(_drillianTrans.DOAnchorPos(new Vector2(-75, -50), t).SetEase(Ease.InOutSine));

                _continueButton.blocked = false;

                break;
            case SceneIdentity.GameTutorial:

                _seq.Append(_lowerBar.DOAnchorPosY(100, t).SetEase(Ease.OutSine));
                _seq.Join(_teamNameText.rectTransform.DOAnchorPosY(450, t).SetEase(Ease.InOutSine));
                _seq.Join(_teamNameText.rectTransform.DOScale(1, t).SetEase(Ease.OutBack));

                _seq.Join(_lunaTrans.DOScale(1.2f, t).SetEase(Ease.InOutSine));
                _seq.Join(_lunaTrans.DOAnchorPos(new Vector2(50, 25), t).SetEase(Ease.InOutSine));

                _seq.Join(_drillianTrans.DOScale(1.2f, t).SetEase(Ease.InOutSine));
                _seq.Join(_drillianTrans.DOAnchorPos(new Vector2(-50, 25), t).SetEase(Ease.InOutSine));

                _continueButton.blocked = false;

                break;
            case SceneIdentity.GameMain:

                _seq.Append(_lowerBar.DOAnchorPosY(-100, t).SetEase(Ease.OutSine));
                _seq.Join(_teamNameText.rectTransform.DOAnchorPosY(650, t).SetEase(Ease.InOutSine));
                _seq.Join(_teamNameText.rectTransform.DOScale(1, t).SetEase(Ease.OutBack));

                _seq.Join(_lunaTrans.DOScale(0.8f, t).SetEase(Ease.OutSine));
                _seq.Join(_lunaTrans.DOAnchorPos(new Vector2(0, 250), t).SetEase(Ease.InOutSine));

                _seq.Join(_drillianTrans.DOScale(0.8f, t).SetEase(Ease.OutSine));
                _seq.Join(_drillianTrans.DOAnchorPos(new Vector2(0, 250), t).SetEase(Ease.InOutSine));

                _continueButton.blocked = true;

                break;
        }

        return t;
    }

    public void SetTeamName(string name)
    {
        _teamNameText.text = name;
    }
    public void SetTeamName(string name1, string name2) 
        => SetTeamName(name1 + "//" + name2);

}
