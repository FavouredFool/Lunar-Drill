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
    [SerializeField] private Vector3 _laserP1PromptPos;
    [SerializeField] private Vector3 _laserMiddlePos;
    [SerializeField] private float _startLaserHeight = 1360;

    [Header("Animation Times")]
    [SerializeField] private float _awakeTime = .5f;
    [SerializeField] private float _setSoloTime = .5f;
    [SerializeField] private float _playTime = .5f;
    [SerializeField] private float _setMultiTime = .5f;
    [SerializeField] private float _swapTime = .5f;

    [Header("Solo")]
    [SerializeField] private GameObject _p1ConnectPrompt;
    [SerializeField] private GameObject _p2ConnectPrompt;
    [SerializeField] private GameObject _soloPrompt;
    [SerializeField] private GameObject _soloCharacters;
    [SerializeField] private GameObject _soloLuna;
    [SerializeField] private Vector3 _soloLunaEndPos;
    [SerializeField] private Vector3 _soloDillianEndPos;
    [SerializeField] private GameObject _soloDrillian;
    [SerializeField] private GameObject _soloPromptBackground;
    [SerializeField] private RectTransform _soloPromptBackgroundCyan;
    [SerializeField] private RectTransform _soloPromptBackgroundYellow;

    [Header("Multi General")]
    [SerializeField] private GameObject _multiPromptPlay;
    [SerializeField] private Vector3 _multiPromptPlayStartPos;
    [SerializeField] private Vector3 _multiPromptPlayEndPos;
    [SerializeField] private GameObject _multiPromptSwap;
    [SerializeField] private Vector3 _multiPromptSwapStartPos;
    [SerializeField] private Vector3 _multiPromptSwapEndPos;
    [SerializeField] private GameObject _multiP1;
    [SerializeField] private Vector3 _multiP1EndPos;
    [SerializeField] private GameObject _multiP2;
    [SerializeField] private Vector3 _multiP2EndPos;
    [SerializeField] private GameObject _multiP1PlayPrompt;
    [SerializeField] private GameObject _multiP2PlayPrompt;
    [SerializeField] private GameObject _multiP1SwapPrompt;
    [SerializeField] private GameObject _multiP2SwapPrompt;
    [SerializeField] private GameObject _multiP1Luna;
    [SerializeField] private Vector3 _multiP1LunaStartPos;
    [SerializeField] private Vector2 _multiP1LunaStartSize;
    [SerializeField] private Vector3 _multiP1LunaStartRot;
    [SerializeField] private GameObject _multiP1Drillian;
    [SerializeField] private GameObject _multiP2Luna;
    [SerializeField] private GameObject _multiP2Drillian;
    [SerializeField] private Vector3 _multiP2DrillianStartPos;
    [SerializeField] private Vector2 _multiP2DrillianStartSize;
    [SerializeField] private Vector3 _multiP2DrillianStartRot;
    [SerializeField] private GameObject _multiPromptBackgroundCyan;
    [SerializeField] private GameObject _multiPromptBackgroundYellow;
    [SerializeField] private Vector3 _multiPromptBackgroundYellowEndPos;

    [Header("Multi Swap")]
    [SerializeField] private Vector3 _lunaTopP1;
    [SerializeField] private Vector3 _lunaTopP2;
    [SerializeField] private Vector3 _lunaBottomP1;
    [SerializeField] private Vector3 _lunaBottomP2;
    [SerializeField] private Vector3 _lunaBottomP3;
    [SerializeField] private Vector3 _drillianTopP1;
    [SerializeField] private Vector3 _drillianTopP2;
    [SerializeField] private Vector3 _drillianTopP3;
    [SerializeField] private Vector3 _drillianBottomP1;
    [SerializeField] private Vector3 _drillianBottomP2;
    [SerializeField] private Vector3 _drillianBottomP3;
    // --- Public Fields ------------------------

    //--- Private Fields ------------------------
    private RectTransform _laserRect;
    private CanvasGroup _p1ConnectPromptCanvasGroup;
    private CanvasGroup _p2ConnectPromptCanvasGroup;
    private CanvasGroup _soloPromptCanvasGroup;
    private Image _multiPromptBackgroundCyanImage;
    private Image _multiPromptBackgroundYellowImage;

    //--- Unity Methods ------------------------

    private void Awake()
    {
        _laserRect = _laser.GetComponent<RectTransform>();
        _p1ConnectPromptCanvasGroup = _p1ConnectPrompt.GetComponent<CanvasGroup>();
        _p2ConnectPromptCanvasGroup = _p2ConnectPrompt.GetComponent<CanvasGroup>();
        _soloPromptCanvasGroup = _soloPrompt.GetComponent<CanvasGroup>();
        _multiPromptBackgroundCyanImage = _multiPromptBackgroundCyan.GetComponent<Image>();
        _multiPromptBackgroundYellowImage = _multiPromptBackgroundYellow.GetComponent<Image>();

        _laserRect.DOSizeDelta(new Vector2(_laserRect.sizeDelta.x, _normalLaserHeight), _awakeTime).SetDelay(.2f).SetUpdate(true); // Scale down
        _laser.transform.DOLocalMove(_laserP1PromptPos, _awakeTime).SetDelay(.2f).SetUpdate(true); // Move down
        _p1ConnectPromptCanvasGroup.DOFade(1, _awakeTime).SetDelay(.2f + _awakeTime / 2).SetUpdate(true); // Fade P1 connect prompt in
    }

    //--- Public Methods ------------------------

    /* Sets up and animates UI for when the first player joins the game. */
    public void SetSolo()
    {
        //Laser
        _laser.transform.DOLocalMove(_laserMiddlePos, _setSoloTime).SetUpdate(true); // Move to middle

        // P2 Connect prompt
        _p2ConnectPromptCanvasGroup.DOFade(1, _setSoloTime).SetUpdate(true); // Fade P2 connect prompt in

        // Solo Part
        _p1ConnectPromptCanvasGroup.alpha = 0; // Remove P1 connect prompt
        _soloPromptCanvasGroup.DOFade(1, _setSoloTime).SetUpdate(true); // Fade solo prompt in
        _soloPromptBackground.SetActive(true); // Background
        _soloPromptBackgroundCyan.transform.DOLocalMove(new Vector3(-541, 0, 0), _setSoloTime).SetUpdate(true);
        _soloPromptBackgroundYellow.transform.DOLocalMove(new Vector3(500, 0, 0), _setSoloTime).SetUpdate(true);
        DOVirtual.DelayedCall(_setSoloTime / 2, () =>
          {
              _soloCharacters.SetActive(true); // Solo characters
              _soloLuna.transform.DOLocalMove(_soloLunaEndPos, _setSoloTime / 2).OnComplete(() =>
                   _soloLuna.transform.DOShakePosition(_setSoloTime, strength: new Vector3(15, 15, 0), vibrato: 5, randomness: 90, snapping: false, fadeOut: true)
                ).SetUpdate(true);
              _soloDrillian.transform.DOLocalMove(_soloDillianEndPos, _setSoloTime / 2).OnComplete(() =>
                  _soloDrillian.transform.DOShakePosition(_setSoloTime, strength: new Vector3(15, 15, 0), vibrato: 5, randomness: 90, snapping: false, fadeOut: true)
              ).SetUpdate(true);
          });
    }

    public void SetMulti()
    {
        // P2 Connect prompt
        _p2ConnectPromptCanvasGroup.alpha = 0; // Remove P2 connect prompt

        // Solo elements & Backgorund
        _soloPromptCanvasGroup.alpha = 0; // Remove solo prompt 
        _soloPromptBackgroundCyan.transform.DOLocalMove(Vector3.zero, _setMultiTime)
            .OnComplete(() => { _soloPromptBackground.SetActive(false); _multiPromptBackgroundCyan.SetActive(true); })
            .SetUpdate(true); // Cyan top
        _multiPromptBackgroundYellow.SetActive(true);
        _multiPromptBackgroundYellow.transform.DOLocalMove(_multiPromptBackgroundYellowEndPos, _setMultiTime).SetUpdate(true); // Yellow bottom

        // Characters
        _soloLuna.transform.DOLocalMove(_multiP1LunaStartPos, _setMultiTime / 2).SetUpdate(true);
        _soloLuna.GetComponent<RectTransform>().DOSizeDelta(_multiP1LunaStartSize, _setMultiTime / 2).SetUpdate(true);
        _soloLuna.transform.DORotate(_multiP1LunaStartRot, _setMultiTime / 2).SetUpdate(true);
        DOVirtual.DelayedCall(_setMultiTime / 2, () =>
           {
               _soloLuna.SetActive(false);
               _multiP1Luna.SetActive(true);
               _multiP1Luna.transform.DOShakePosition(_setMultiTime, strength: new Vector3(15, 15, 0), vibrato: 5, randomness: 90, snapping: false, fadeOut: true).SetUpdate(true);
           });
        _soloDrillian.transform.DOLocalMove(_multiP2DrillianStartPos, _setMultiTime / 2).SetUpdate(true);
        _soloDrillian.GetComponent<RectTransform>().DOSizeDelta(_multiP2DrillianStartSize, _setMultiTime / 2).SetUpdate(true);
        _soloDrillian.transform.DORotate(_multiP2DrillianStartRot, _setMultiTime / 2).SetUpdate(true);
        DOVirtual.DelayedCall(_setMultiTime / 2, () =>
        {
            _soloDrillian.SetActive(false);
            _multiP2Drillian.SetActive(true);
            _multiP2Drillian.transform.DOShakePosition(_setMultiTime, strength: new Vector3(15, 15, 0), vibrato: 5, randomness: 90, snapping: false, fadeOut: true).SetUpdate(true);
        });

        // Multi elements
        _multiPromptPlay.SetActive(true);
        _multiPromptPlay.transform.DOLocalMove(_multiPromptPlayEndPos, _setMultiTime).SetUpdate(true);
        _multiPromptSwap.SetActive(true);
        _multiPromptSwap.transform.DOLocalMove(_multiPromptSwapEndPos, _setMultiTime).SetUpdate(true);
        _multiP1.SetActive(true);
        _multiP1.transform.DOLocalMove(_multiP1EndPos, _setMultiTime).SetUpdate(true);
        _multiP2.SetActive(true);
        _multiP2.transform.DOLocalMove(_multiP2EndPos, _setMultiTime).SetUpdate(true);
    }

    public void Play()
    {
        _laserRect.DOSizeDelta(new Vector2(_laserRect.sizeDelta.x, _startLaserHeight), _playTime).OnComplete(() => SceneManager.LoadScene("MainScene")).SetUpdate(true); // Scale up till white
        _multiPromptPlay.transform.DOLocalMove(_multiPromptPlayStartPos, _playTime / 3).SetUpdate(true); // Move prompts out of the way
        _multiPromptSwap.transform.DOLocalMove(_multiPromptSwapStartPos, _playTime / 3).SetUpdate(true);
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
            _multiPromptBackgroundCyanImage.color = _cyan;
            _multiPromptBackgroundYellowImage.color = _yellow;

            _multiP2Luna.transform.DOLocalMove(_lunaBottomP3, _swapTime)
           .OnComplete(() =>
           {
               _multiP1Luna.SetActive(true);
               _multiP1Luna.transform.DOShakePosition(_swapTime, strength: new Vector3(15, 15, 0), vibrato: 5, randomness: 90, snapping: false, fadeOut: true).SetUpdate(true);
               _multiP2Luna.SetActive(false);
               _multiP2Luna.transform.localPosition = _lunaBottomP1;
           })
           .SetUpdate(true);

            _multiP1Drillian.transform.DOLocalMove(_drillianTopP3, _swapTime / 2)
            .OnComplete(() =>
            {
                _multiP1Drillian.SetActive(false);
                _multiP1Drillian.transform.localPosition = _drillianTopP1;
                _multiP2Drillian.SetActive(true);
                _multiP2Drillian.transform.DOLocalMove(_drillianBottomP2, _swapTime / 2).
                OnComplete(() =>
                _multiP2Drillian.transform.DOShakePosition(_swapTime, strength: new Vector3(15, 15, 0), vibrato: 5, randomness: 90, snapping: false, fadeOut: true).SetUpdate(true))
                .SetUpdate(true);
            })
            .SetUpdate(true);
        }
        else
        {
            _multiPromptBackgroundCyanImage.color = _yellow;
            _multiPromptBackgroundYellowImage.color = _cyan;

            _multiP1Luna.transform.DOLocalMove(_lunaTopP2, _swapTime / 2)
            .OnComplete(() =>
            {
                _multiP1Luna.SetActive(false);
                _multiP1Luna.transform.localPosition = _lunaTopP1;
                _multiP2Luna.SetActive(true);
                _multiP2Luna.transform.DOLocalMove(_lunaBottomP2, _swapTime / 2)
                .OnComplete(() => _multiP2Luna.transform.DOShakePosition(_swapTime, strength: new Vector3(15, 15, 0), vibrato: 5, randomness: 90, snapping: false, fadeOut: true).SetUpdate(true))
                .SetUpdate(true);
            })
            .SetUpdate(true);

            _multiP2Drillian.transform.DOLocalMove(_drillianBottomP3, _swapTime / 2)
           .OnComplete(() =>
           {
               _multiP2Drillian.SetActive(false);
               _multiP2Drillian.transform.localPosition = _drillianBottomP1;
               _multiP1Drillian.SetActive(true);
               _multiP1Drillian.transform.DOLocalMove(_drillianTopP2, _swapTime / 2).
               OnComplete(() =>
               _multiP1Drillian.transform.DOShakePosition(_swapTime, strength: new Vector3(15, 15, 0), vibrato: 5, randomness: 90, snapping: false, fadeOut: true).SetUpdate(true))
               .SetUpdate(true);
           })
           .SetUpdate(true);

            //_multiP1Luna.SetActive(false);
            //_multiP1Drillian.SetActive(true);
            //_multiP2Luna.SetActive(true);
            //_multiP2Drillian.SetActive(false);
        }
    }

    //--- Private Methods ------------------------
}
