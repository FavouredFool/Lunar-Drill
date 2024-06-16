using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

//DontDestroyOnLoad via Persistent Prefab
public class PreparationInterface : MonoBehaviour
{
    public static PreparationInterface instance;
    [SerializeField] RectTransform _teamName;

    [SerializeField] RectTransform _lunaTrans, _drillianTrans;
    [SerializeField] RectTransform _lowerBar;
    public CoopButton _continueButton;

    [SerializeField] RectTransform _P1,_P2;
    [SerializeField] TMP_Text
        P1_Text, P2_Text;
    [SerializeField] RectTransform
        P1_ConPrompt, P2_ConPrompt;
    [SerializeField] RectTransform
    Luna_MovePrompt, Drillian_MovePrompt;
    [SerializeField] TMP_Text NextUp_Text;

    Sequence _seq;

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        instance = this;
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
                _seq.Join(_teamName.DOAnchorPosY(0, t).SetEase(Ease.InOutSine));
                _seq.Join(_teamName.DOScale(0, t).SetEase(Ease.OutBack));

                _seq.Join(_lunaTrans.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(_lunaTrans.DOAnchorPos(new Vector2(0, 250), t).SetEase(Ease.InOutSine));

                _seq.Join(_drillianTrans.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(_drillianTrans.DOAnchorPos(new Vector2(0, 250), t).SetEase(Ease.InOutSine));

                _seq.Join(_P1.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(_P2.DOScale(0, t).SetEase(Ease.OutSine));

                _seq.Join(Luna_MovePrompt.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(Drillian_MovePrompt.DOScale(0, t).SetEase(Ease.OutSine));

                _seq.Join(NextUp_Text.transform.parent.DOScale(0, t).SetEase(Ease.OutSine));

                _continueButton.blocked = true;

                break;
            case SceneIdentity.PlayerConnect:

                _seq.Append(_lowerBar.DOAnchorPosY(100, t).SetEase(Ease.OutSine));
                _seq.Join(_teamName.DOAnchorPosY(0, t).SetEase(Ease.InOutSine));
                _seq.Join(_teamName.DOScale(1, t).SetEase(Ease.OutBack));

                _seq.Join(_lunaTrans.DOScale(1f, t).SetEase(Ease.OutSine));
                _seq.Join(_lunaTrans.DOAnchorPos(new Vector2(35, 60), t).SetEase(Ease.InOutSine));

                _seq.Join(_drillianTrans.DOScale(1f, t).SetEase(Ease.OutSine));
                _seq.Join(_drillianTrans.DOAnchorPos(new Vector2(-35, 60), t).SetEase(Ease.InOutSine));

                _seq.Join(_P1.DOScale(1, t).SetEase(Ease.OutSine));
                _seq.Join(_P2.DOScale(1, t).SetEase(Ease.OutSine));

                _seq.Join(Luna_MovePrompt.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(Drillian_MovePrompt.DOScale(0, t).SetEase(Ease.OutSine));

                _seq.Join(NextUp_Text.transform.parent.DOScale(0, t).SetEase(Ease.OutSine));

                _continueButton.blocked = false;

                break;
            case SceneIdentity.PlayerSelect:

                _seq.Append(_lowerBar.DOAnchorPosY(100, t).SetEase(Ease.OutSine));
                _seq.Join(_teamName.DOAnchorPosY(450, t).SetEase(Ease.InOutSine));
                _seq.Join(_teamName.DOScale(0.5f, t).SetEase(Ease.OutBack));

                _seq.Join(_lunaTrans.DOScale(1.5f, t).SetEase(Ease.OutSine));
                _seq.Join(_lunaTrans.DOAnchorPos(new Vector2(45, -33), t).SetEase(Ease.InOutSine));

                _seq.Join(_drillianTrans.DOScale(1.5f, t).SetEase(Ease.OutSine));
                _seq.Join(_drillianTrans.DOAnchorPos(new Vector2(-75, -50), t).SetEase(Ease.InOutSine));

                _seq.Join(_P1.DOScale(1, t).SetEase(Ease.OutSine));
                _seq.Join(_P2.DOScale(1, t).SetEase(Ease.OutSine));

                _seq.Join(Luna_MovePrompt.DOScale(1, t).SetEase(Ease.OutSine));
                _seq.Join(Drillian_MovePrompt.DOScale(1, t).SetEase(Ease.OutSine));

                _seq.Join(NextUp_Text.transform.parent.DOScale(1, t).SetEase(Ease.OutSine));
                NextUp_Text.text = "Tutorial";

                _continueButton.blocked = false;

                break;
            case SceneIdentity.GameTutorial:

                _seq.Append(_lowerBar.DOAnchorPosY(100, t).SetEase(Ease.OutSine));
                _seq.Join(_teamName.DOAnchorPosY(650, t).SetEase(Ease.InOutSine));
                _seq.Join(_teamName.DOScale(0.5f, t).SetEase(Ease.OutBack));

                _seq.Join(_lunaTrans.DOScale(1.2f, t).SetEase(Ease.InOutSine));
                _seq.Join(_lunaTrans.DOAnchorPos(new Vector2(50, 25), t).SetEase(Ease.InOutSine));

                _seq.Join(_drillianTrans.DOScale(1.2f, t).SetEase(Ease.InOutSine));
                _seq.Join(_drillianTrans.DOAnchorPos(new Vector2(-50, 25), t).SetEase(Ease.InOutSine));

                _seq.Join(_P1.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(_P2.DOScale(0, t).SetEase(Ease.OutSine));

                _seq.Join(Luna_MovePrompt.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(Drillian_MovePrompt.DOScale(0, t).SetEase(Ease.OutSine));

                _seq.Join(NextUp_Text.transform.parent.DOScale(1, t).SetEase(Ease.OutSine));
                NextUp_Text.text = "Gameplay!";

                _continueButton.blocked = false;

                break;
            case SceneIdentity.GameMain:

                _seq.Append(_lowerBar.DOAnchorPosY(-100, t).SetEase(Ease.OutSine));
                _seq.Join(_teamName.DOAnchorPosY(650, t).SetEase(Ease.InOutSine));
                _seq.Join(_teamName.DOScale(0.5f, t).SetEase(Ease.OutBack));

                _seq.Join(_lunaTrans.DOScale(0.8f, t).SetEase(Ease.OutSine));
                _seq.Join(_lunaTrans.DOAnchorPos(new Vector2(0, 250), t).SetEase(Ease.InOutSine));

                _seq.Join(_drillianTrans.DOScale(0.8f, t).SetEase(Ease.OutSine));
                _seq.Join(_drillianTrans.DOAnchorPos(new Vector2(0, 250), t).SetEase(Ease.InOutSine));

                _seq.Join(_P1.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(_P2.DOScale(0, t).SetEase(Ease.OutSine));

                _seq.Join(Luna_MovePrompt.DOScale(0, t).SetEase(Ease.OutSine));
                _seq.Join(Drillian_MovePrompt.DOScale(0, t).SetEase(Ease.OutSine));

                _seq.Join(NextUp_Text.transform.parent.DOScale(0, t).SetEase(Ease.OutSine));

                _continueButton.blocked = true;

                break;
        }

        return t;
    }

    public void SetPlayerInfo(int playersConnected)
    {
        switch (playersConnected)
        {
            case 2:
                P1_Text.text = "P1\nLuna";
                P1_Text.transform.DOScale(1f, 0.33f).SetEase(Ease.OutBack);
                P1_ConPrompt.DOScale(0f, 0.15f).SetEase(Ease.InBack);

                P2_Text.text = "P2\nDrillian";
                P2_Text.transform.DOScale(1f, 0.33f).SetEase(Ease.OutBack);
                P2_ConPrompt.DOScale(0f, 0.15f).SetEase(Ease.InBack);

                Luna_MovePrompt.DOScale(1f, 0.33f).SetEase(Ease.OutSine);
                Drillian_MovePrompt.DOScale(1f, 0.33f).SetEase(Ease.OutSine);

                NextUp_Text.transform.parent.DOScale(1, 0.33f).SetEase(Ease.OutSine);
                NextUp_Text.text = "Swap Screen";

                break;
            case 1:
                P1_Text.text = "P1\nSolo";
                P1_Text.transform.DOScale(1f, 0.33f).SetEase(Ease.OutBack);
                P1_ConPrompt.DOScale(0f, 0.15f).SetEase(Ease.InBack);

                P2_Text.text = "P2\nConnect...";
                P2_Text.transform.DOScale(0.5f, 0.33f).SetEase(Ease.OutBack);
                P2_ConPrompt.DOScale(1f, 0.15f).SetEase(Ease.InBack);

                Luna_MovePrompt.DOScale(1f, 0.33f).SetEase(Ease.OutSine);
                Drillian_MovePrompt.DOScale(1f, 0.33f).SetEase(Ease.OutSine);

                NextUp_Text.transform.parent.DOScale(1, 0.33f).SetEase(Ease.OutSine);
                NextUp_Text.text = "Tutorial";

                break;
            case 0:
            default:
                P1_Text.text = "P1\n...Connect";
                P1_Text.transform.DOScale(0.5f, 0.33f).SetEase(Ease.OutBack);
                P1_ConPrompt.DOScale(1f, 0.15f).SetEase(Ease.InBack);

                P2_Text.text = "P2\nConnect...";
                P2_Text.transform.DOScale(0.5f, 0.33f).SetEase(Ease.OutBack);
                P2_ConPrompt.DOScale(1f, 0.15f).SetEase(Ease.InBack);

                Luna_MovePrompt.DOScale(0f, 0.33f).SetEase(Ease.OutSine);
                Drillian_MovePrompt.DOScale(0f, 0.33f).SetEase(Ease.OutSine);

                NextUp_Text.transform.parent.DOScale(0, 0.33f).SetEase(Ease.OutSine);

                break;
        }
    }
    bool swappedPlayerInfo;
    public void SwapPlayerInfo()
    {
        swappedPlayerInfo = !swappedPlayerInfo;

        P1_Text.text = (swappedPlayerInfo ? "P2" : "P1") + "\nLuna";
        DOTween.Kill(P1_Text.transform);
        P1_Text.transform.localScale = Vector3.one;
        P1_Text.transform.DOPunchScale(Vector3.one*0.1f, 0.33f).SetEase(Ease.OutQuad);


        P2_Text.text = (swappedPlayerInfo ? "P1" : "P2") + "\nDrillian";
        DOTween.Kill(P2_Text.transform);
        P2_Text.transform.localScale = Vector3.one;
        P2_Text.transform.DOPunchScale(Vector3.one * 0.1f, 0.33f).SetEase(Ease.OutQuad);
    }

}
