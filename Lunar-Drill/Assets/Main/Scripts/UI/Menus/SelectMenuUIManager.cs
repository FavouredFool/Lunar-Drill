using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectMenuUIManager : MonoBehaviour
{
    //--- Exposed Fields ------------------------
    [Header("Colors")]
    [SerializeField] private Color _cyan;
    [SerializeField] private Color _yellow;

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
    [SerializeField] private GameObject _multiP1PlayPrompt;
    [SerializeField] private GameObject _multiP2PlayPrompt;
    [SerializeField] private GameObject _multiP1SwapPrompt;
    [SerializeField] private GameObject _multiP2SwapPrompt;
    [SerializeField] private GameObject _multiP1Luna;
    [SerializeField] private GameObject _multiP1Drillian;
    [SerializeField] private GameObject _multiP2Luna;
    [SerializeField] private GameObject _multiP2Drillian;
    [SerializeField] private GameObject _multiPromptBackground;
    [SerializeField] private Image _multiPromptBackgroundCyan;
    [SerializeField] private Image _multiPromptBackgroundYellow;

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
        _multiPromptPlay.SetActive(false); // Turn off elements that would be in the way
        _multiPromptSwap.SetActive(false);
    }

    public void RefreshMulti(bool p1Play, bool p1Swap, bool p2Play, bool p2Swap)
    {
        _multiP1PlayPrompt.SetActive(p1Play);
        _multiP1SwapPrompt.SetActive(p1Swap);
        _multiP2PlayPrompt.SetActive(p2Play);
        _multiP2SwapPrompt.SetActive(p2Swap);
    }

    public void Swap(bool p1Luna)
    {
        if (p1Luna)
        {
            _multiPromptBackgroundCyan.color = _cyan;
            _multiPromptBackgroundYellow.color = _yellow;
            _multiP1Luna.SetActive(true);
            _multiP1Drillian.SetActive(false);
            _multiP2Luna.SetActive(false);
            _multiP2Drillian.SetActive(true);
        }
        else
        {
            _multiPromptBackgroundCyan.color = _yellow;
            _multiPromptBackgroundYellow.color = _cyan;
            _multiP1Luna.SetActive(false);
            _multiP1Drillian.SetActive(true);
            _multiP2Luna.SetActive(true);
            _multiP2Drillian.SetActive(false);
        }
    }

    //--- Private Methods ------------------------
}
