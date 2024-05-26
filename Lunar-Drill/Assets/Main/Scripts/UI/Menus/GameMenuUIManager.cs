using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuUIManager : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Options Functionality")]
    [SerializeField] private GameObject _optionsMenuContainer;
    [SerializeField] private GameObject _optionsBackgroundPanel;
    [SerializeField] private GameObject _optionsMenuFirstSelect;

    [SerializeField] private GameObject _laser;
    [SerializeField] private Vector3 _laserFirstPosition, _laserSecondPosition;
    [SerializeField] private float _laserMinHeight, _laserMaxHeight;
    [SerializeField] float _laserInTime = 0.05f;
    [SerializeField] float _laserExpandTime = 0.1f;

    // --- Public Fields ------------------------
    public bool IsOpen = false;

    //--- Private Fields ------------------------

    private RectTransform _laserRect;
    private Image _backgroundPanelImage;

    //--- Unity Methods ------------------------

    private void Awake()
    {
        _laserRect = _laser.GetComponent<RectTransform>();
        _backgroundPanelImage = _optionsBackgroundPanel.GetComponent<Image>();
    }

    //--- Public Methods ------------------------

    /* Switches the scene. */
    public void SwitchScene(string sceneName)
    {
        Rumble.rumblePaused = false;
        Rumble.rumbleDisabled = false;
        Rumble.main?.ClearAndStopAllRumble();
        SceneManager.LoadScene(sceneName);
    }


    /* Opens and closes options. */
    public void HandleToggleInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        ToggleOptions();
    }

    /* Toggles options in th emain menu. */
    public void ToggleOptions()
    {
        if (IsOpen) //Close it
        {
            _optionsMenuContainer.SetActive(false); // Options off
            _laserRect.DOSizeDelta(new Vector2(_laserRect.sizeDelta.x, _laserMinHeight), _laserExpandTime).SetUpdate(true); // Scale down
            _backgroundPanelImage.DOFade(0, _laserExpandTime).SetDelay(_laserInTime + 0.05f).OnComplete(() => _optionsBackgroundPanel.SetActive(false)).SetUpdate(true); // Background fade out
            _laser.transform.DOLocalMove(_laserFirstPosition, _laserInTime).SetDelay(_laserExpandTime).SetUpdate(true); // Laser out

            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(null, new BaseEventData(eventSystem)); // Select nothing 

            Time.timeScale = 1;
            Rumble.rumblePaused = false;
            Rumble.main?.StopAllRumble();
        }
        else //Open it
        {
            _laser.transform.DOLocalMove(_laserSecondPosition, _laserInTime).SetUpdate(true); // Laser in

            _optionsBackgroundPanel.SetActive(true); // Background fading in
            _backgroundPanelImage.DOFade(.9f, _laserExpandTime).SetDelay(_laserInTime + 0.05f).SetUpdate(true);

            _laserRect.DOSizeDelta(new Vector2(_laserRect.sizeDelta.x, _laserMaxHeight), _laserExpandTime).SetDelay(_laserInTime + 0.05f).OnComplete(() => _optionsMenuContainer.SetActive(true)).SetUpdate(true); // Scaling up, showing options


            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(_optionsMenuFirstSelect, new BaseEventData(eventSystem));  // Select for controller support

            Rumble.rumblePaused = true;
            Rumble.main?.StopAllRumble();
        }
        IsOpen = !IsOpen;
    }

    //--- Private Methods ------------------------

}
