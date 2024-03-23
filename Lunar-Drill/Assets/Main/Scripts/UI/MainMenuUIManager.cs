using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] private GameObject _firstSelected; // Object that should be first selected

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

    /* Sets "Start" Button as selected UI Element to allow menu controlling with controllers. */
    public void SetSelectedUIElement()
    {
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(_firstSelected, new BaseEventData(eventSystem));
    }

    //--- Private Methods ------------------------
}
