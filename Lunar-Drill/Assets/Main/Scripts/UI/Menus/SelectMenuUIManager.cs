using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMenuUIManager : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Animated Laser")]
    [SerializeField] private GameObject _laser;
    [SerializeField] private float _normalLaserHeight = 202;
    [SerializeField] private float _laserAwakeTime = .7f;
    [SerializeField] private Vector3 _laserP1PromptPos;
    [SerializeField] private Vector3 _laserMiddlePos;
    [SerializeField] private float _laserSoloTime = .7f;
    [SerializeField] private float _laserStartTime = .7f;
    [SerializeField] private float _startLaserHeight = 1360;

    [Header("Select Menu")]
    [SerializeField] private GameObject _p1ConnectPrompt;
    [SerializeField] private GameObject _p2ConnectPrompt;
    [SerializeField] private GameObject _soloPrompt;
    [SerializeField] private GameObject _soloPromptBackground;
    [SerializeField] private GameObject _multiPromptPlay;
    [SerializeField] private GameObject _multiPromptSwap;
    [SerializeField] private GameObject _multiP1;
    [SerializeField] private GameObject _multiP2;
    [SerializeField] private GameObject _multiPromptBackground;

    // --- Public Fields ------------------------

    //--- Private Fields ------------------------
    private RectTransform _laserRect;
    private CanvasGroup _p1ConnectPromptCanvasGroup;
    private CanvasGroup _p2ConnectPromptCanvasGroup;
    private CanvasGroup _soloPromptCanvasGroup;
    private CanvasGroup _soloPromptBackgroundCanvasGroup;
    private CanvasGroup _multiPromptPlayCanvasGroup;
    private CanvasGroup _multiPromptSwapCanvasGroup;
    private CanvasGroup _multiP1CanvasGroup;
    private CanvasGroup _multiP2CanvasGroup;
    private CanvasGroup _multiPromptBackgroundCanvasGroup;

    //--- Unity Methods ------------------------

    private void Awake()
    {
        _laserRect = _laser.GetComponent<RectTransform>();
        _p1ConnectPromptCanvasGroup = _p1ConnectPrompt.GetComponent<CanvasGroup>();
        _p2ConnectPromptCanvasGroup = _p2ConnectPrompt.GetComponent<CanvasGroup>();
        _soloPromptCanvasGroup = _soloPrompt.GetComponent<CanvasGroup>();
        _soloPromptBackgroundCanvasGroup = _soloPromptBackground.GetComponent<CanvasGroup>();
        _multiPromptPlayCanvasGroup = _multiPromptPlay.GetComponent<CanvasGroup>();
        _multiPromptSwapCanvasGroup = _multiPromptSwap.GetComponent<CanvasGroup>();
        _multiP1CanvasGroup = _multiP1.GetComponent<CanvasGroup>();
        _multiP2CanvasGroup = _multiP2.GetComponent<CanvasGroup>();
        _multiPromptBackgroundCanvasGroup = _multiPromptBackground.GetComponent<CanvasGroup>();

        _laserRect.DOSizeDelta(new Vector2(_laserRect.sizeDelta.x, _normalLaserHeight), _laserAwakeTime).SetDelay(.2f).SetUpdate(true); // Scale down
        _laser.transform.DOLocalMove(_laserP1PromptPos, _laserAwakeTime).SetDelay(.2f).SetUpdate(true); // Move down
        _p1ConnectPromptCanvasGroup.DOFade(1, _laserAwakeTime).SetDelay(.2f + _laserAwakeTime / 2).SetUpdate(true); // Fade P1 connect prompt in
    }

    //--- Public Methods ------------------------

    public void SetSolo()
    {
        _laser.transform.DOLocalMove(_laserMiddlePos, _laserSoloTime).SetUpdate(true); // Move to middle
        _p2ConnectPromptCanvasGroup.DOFade(1, _laserSoloTime).SetUpdate(true); // Fade P2 connect prompt in
        _soloPromptCanvasGroup.DOFade(1, _laserSoloTime).SetUpdate(true); // Fade solo prompt in
        _soloPromptBackgroundCanvasGroup.DOFade(1, _laserSoloTime).SetUpdate(true);
        _p1ConnectPromptCanvasGroup.DOFade(0, _laserSoloTime).SetUpdate(true); // Fade P1 connect prompt out
    }

    public void SetMulti()
    {
        _p2ConnectPromptCanvasGroup.DOFade(0, _laserSoloTime).SetUpdate(true); // Fade P2 connect prompt out
        _soloPromptCanvasGroup.DOFade(0, _laserSoloTime).SetUpdate(true); // Fade solo prompt out
        _soloPromptBackgroundCanvasGroup.DOFade(0, _laserSoloTime).SetUpdate(true);
        _multiPromptPlayCanvasGroup.DOFade(1, _laserSoloTime).SetUpdate(true); // Fade multi prompt in
        _multiPromptSwapCanvasGroup.DOFade(1, _laserSoloTime).SetUpdate(true);
        _multiP1CanvasGroup.DOFade(1, _laserSoloTime).SetUpdate(true);
        _multiP2CanvasGroup.DOFade(1, _laserSoloTime).SetUpdate(true);
        _multiPromptBackgroundCanvasGroup.DOFade(1, _laserSoloTime).SetUpdate(true);
    }

    public void Play()
    {
        _laserRect.DOSizeDelta(new Vector2(_laserRect.sizeDelta.x, _startLaserHeight), _laserStartTime).SetDelay(.2f).OnComplete(() => SceneManager.LoadScene("MainScene")).SetUpdate(true); // Scale up till white
    }

    public void RefreshReady(bool p1, bool p2, float time)
    {
        //P1Ready.SetActive(p1);
        //P2Ready.SetActive(p2);

        //ReadyIndicator.color =
        //    Color.Lerp(Color.white, lilac, time / ConnectManager.AgreeTime);
        //ReadySoloIndicator.color = ReadyIndicator.color;
    }

    public void RefreshSwap(bool p1, bool p2, float time)
    {
        //P1Swap.SetActive(p1);
        //P2Swap.SetActive(p2);

        //SwapIndicator.color =
        //    Color.Lerp(Color.white, lilac, time / ConnectManager.AgreeTime);
    }
    //--- Private Methods ------------------------
}
