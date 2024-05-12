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

    // --- Public Fields ------------------------
    public bool IsOpen = false;

    //--- Private Fields ------------------------

    //--- Unity Methods ------------------------

    //--- Public Methods ------------------------

    /* Switches the scene to main game scene. */
    public void StartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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
