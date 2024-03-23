using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionFix : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    //--- Private Fields ------------------------

    private GameObject _lastSelected; // Object that was last selected, will automatically be selected if nothing is selected anymore
    private Vector3 _lastMousePosition;

    private bool _cameFromBackButton = false;

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

        if (!_cameFromBackButton)
        {
            if (_lastMousePosition != Input.mousePosition)
            {

                // THis part is from https://forum.unity.com/threads/option-to-select-element-on-mouseover.1214904/ by HunterAhlquist
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
                            _lastSelected = EventSystem.current.currentSelectedGameObject;
                            break;
                        }
                    }
                }

                _lastMousePosition = Input.mousePosition;
            }
        }
    }

    //--- Public Methods ------------------------

    public void CameFromBackButton()
    {
        _cameFromBackButton = true;
        StartCoroutine(OnCameFromBackButton());
    }


    //--- Private Methods ------------------------

    private IEnumerator OnCameFromBackButton()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        _cameFromBackButton = false;
    }
}
