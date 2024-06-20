using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class NewUndertaker : MonoBehaviour, IInputSubscriber<Signal_SceneChange>
{
    public static bool isOpen;

    [SerializeField] Image Top, Low;
    [SerializeField] TMP_Text TopText, LowText;
    [SerializeField] RectTransform TopSlash, LowSlash;

    [SerializeField]
    CoopButton
        ReturnButton,
        RetryButton,
        LeaderboardButton;

    [SerializeField]
        Image blackfade;

    Sequence seq;

    private void Start()
    {
        InputBus.Subscribe(this);
    }
    // Ends the Scene
    public void OnEventHappened(Signal_SceneChange e)
    {
        AudioController.Fire(new EndSceneStateChange(EndSceneStateChange.State.EndScreenInactive));
        blackfade.DOFade(1,e.delay).SetEase(Ease.InSine).SetUpdate(true);
    }

    public void Open(GameObject focus, bool isPlayer)
    {
        // Mute audio
        AudioController.Fire(new EndSceneStateChange(EndSceneStateChange.State.EndScreenActive));

        isOpen = true;
        transform.position = focus.transform.position;
        gameObject.SetActive(true);

        seq.Kill();
        seq = DOTween.Sequence();

        Top.rectTransform.anchoredPosition = new Vector2(0, 450);
        Low.rectTransform.anchoredPosition = new Vector2(0, -450);
        TopText.rectTransform.anchoredPosition = new Vector2(1920, 225);
        LowText.rectTransform.anchoredPosition = new Vector2(-1920, -225);
        TopSlash.anchoredPosition = new Vector2(220, 695);
        LowSlash.anchoredPosition = new Vector2(-220, -695);

        LeaderboardButton.transform.parent.localScale = Vector3.zero;
        ReturnButton.transform.parent.localScale = Vector3.zero;
        RetryButton.transform.parent.localScale = Vector3.zero;

        LeaderboardButton.blocked = !isPlayer;
        ReturnButton.blocked = isPlayer;
        RetryButton.blocked = isPlayer;


        seq.Append(Top.rectTransform.DOAnchorPosY(0, 0.33f).SetEase(Ease.OutQuint));
        seq.Join(Low.rectTransform.DOAnchorPosY(0, 0.33f).SetEase(Ease.OutQuint));

        seq.Join(TopText.rectTransform.DOAnchorPosX(0, 0.25f).SetEase(Ease.OutElastic).SetDelay(0.15f).OnStart(() =>
        {
            if (isPlayer) AudioController.Fire(new EndSceneLunar(""));
            else AudioController.Fire(new EndSceneGame(""));
            Rumble.main?.RumbleBoth(4, 2, 0.33f);
        }));

        seq.Join(TopSlash.DOAnchorPos(new Vector3(-75, 0), 0.15f).SetEase(Ease.OutBack).SetDelay(0.45f).OnStart(() =>
        {
            AudioController.Fire(new EndSceneShing(""));
        }));
        seq.Join(LowSlash.DOAnchorPos(new Vector3(75, 0), 0.15f).SetEase(Ease.OutBack).SetDelay(0.1f).OnStart(() =>
        {
            AudioController.Fire(new EndSceneShing(""));
        }));

        seq.Join(LowText.rectTransform.DOAnchorPosX(0, 0.25f).SetEase(Ease.OutElastic).SetDelay(0.45f).OnStart(() =>
        {
            if (isPlayer) AudioController.Fire(new EndSceneDrill(""));
            else AudioController.Fire(new EndSceneOver(""));
            Rumble.main?.RumbleBoth(6, 2, 0.33f);
        }));

        if(isPlayer)
            seq.Join(LeaderboardButton.transform.parent.DOScale(2.5f,0.25f).SetEase(Ease.OutBack).SetDelay(0.1f));
        else
        {
            seq.Join(RetryButton.transform.parent.DOScale(2.5f, 0.25f).SetEase(Ease.OutBack).SetDelay(0.1f));
            seq.Join(ReturnButton.transform.parent.DOScale(2.5f, 0.25f).SetEase(Ease.OutBack).SetDelay(0.1f));
        }

        seq.SetUpdate(true);
    }
}
