using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Main Menu")]
    [SerializeField] private List<GameObject> _mainMenuButtons;

    [Header("Options Functionality")]
    [SerializeField] private RectTransform _mainMenuContainer;
    [SerializeField] private RectTransform _optionsMenuContainer;

    [SerializeField] private Vector3 _mainMenuFirstPosition, _mainMenuSecondPosition;
    [SerializeField] private Vector3 _optionsMenuFirstPosition, _optionsMenuSecondPosition;

    [SerializeField] private GameObject _mainMenuFirstSelect;
    [SerializeField] private GameObject _optionsMenuFirstSelect;

    [SerializeField] [Range(0, 3)] float _pullTime;

    [Header("Blend to Game")]
    [SerializeField] private GameObject _backgroundBlend;
    [SerializeField] private GameObject _laser;
    [SerializeField] private GameObject _laserEnd;
    [SerializeField] private GameObject _luna;

    [SerializeField] private Vector3 _laserSecondPosition;
    [SerializeField] private Vector3 _laserEndSecondPosition;
    [SerializeField] private Vector3 _lunaSecondPosition;

    [SerializeField] private float _blendTime = 1f;

    // --- Public Fields ------------------------
    public bool IsOpen = false;

    //--- Private Fields ------------------------

    private Image _backgroundBlendImage;

    //--- Unity Methods ------------------------

    private void Awake()
    {
        _backgroundBlendImage = _backgroundBlend.GetComponent<Image>();
    }

    //--- Public Methods ------------------------

    /* Switches the scene to main game scene and plays little transition animation. */
    public void StartGame(string sceneName)
    {
        _backgroundBlend.SetActive(true);
        _backgroundBlendImage.DOFade(1, _blendTime - 0.2f).SetUpdate(true); // Fade background to black

        _laser.transform.DOLocalMove(_laserSecondPosition, _blendTime).SetUpdate(true); // Laser 
        _laserEnd.transform.DOLocalMove(_laserEndSecondPosition, _blendTime).SetUpdate(true); // Laser end
        _luna.transform.DOLocalMove(_lunaSecondPosition, _blendTime).OnComplete(() => SceneManager.LoadScene(sceneName)).SetUpdate(true); // Luna and switching to game scene
    }

    /* Quits the application. */
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }


    /* Sets Main Menu Button given by index in list as selected UI Element to allow menu controlling with controllers. */
    public void SelectUIElement(int idx)
    {
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(_mainMenuButtons[idx], new BaseEventData(eventSystem));
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
            _optionsMenuContainer.transform.DOLocalMove(_optionsMenuFirstPosition, _pullTime).SetUpdate(true);
            _mainMenuContainer.transform.DOLocalMove(_mainMenuFirstPosition, _pullTime).SetUpdate(true);

            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(_mainMenuFirstSelect, new BaseEventData(eventSystem)); // Select for controller support

            Time.timeScale = 1; // TODO: figure out why that needed to be here but =0 is nowhere to be found

            // Save the settings:
            SettingSaver.Save();
        }
        else //Open it
        {
            _optionsMenuContainer.transform.DOLocalMove(_optionsMenuSecondPosition, _pullTime).SetUpdate(true);
            _mainMenuContainer.transform.DOLocalMove(_mainMenuSecondPosition, _pullTime).SetUpdate(true);

            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(_optionsMenuFirstSelect, new BaseEventData(eventSystem));  // Select for controller support
        }
        IsOpen = !IsOpen;
    }

    //--- Private Methods ------------------------
}
