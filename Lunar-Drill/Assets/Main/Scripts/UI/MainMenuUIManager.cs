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

    private GameObject _lastSelected; // Object that was last selected, will automatically be selected if nothing is selected anymore
    private Vector3 _lastMousePosition;


    //--- Unity Methods ------------------------

    private void Awake()
    {
        _lastMousePosition = Input.mousePosition;
    }

    private void Update()
    {
        // Cheaty way to always have something selected
        var eventSystem = EventSystem.current;
        if (eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(_lastSelected, new BaseEventData(eventSystem));
        }

        _lastSelected = eventSystem.currentSelectedGameObject;


        if (_lastMousePosition != Input.mousePosition)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            var pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            EventSystem.current.RaycastAll(pointerEventData, results);
            foreach (var r in results)
            {
                if (r.gameObject && r.gameObject.TryGetComponent(out Selectable sel))
                {
                    if (sel.interactable)
                    {
                        EventSystem.current.SetSelectedGameObject(r.gameObject);
                        break;
                    }
                }
            }

            _lastMousePosition = Input.mousePosition;
        }


    }

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
