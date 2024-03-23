using UnityEngine;
using UnityEngine.EventSystems;

public class MouseSelectOnHoverOver : MonoBehaviour, IPointerEnterHandler
{
    //--- Exposed Fields ------------------------

    [SerializeField] GameObject _me;

    //--- Private Fields ------------------------

    //--- Unity Methods ------------------------
  
    public void OnPointerEnter(PointerEventData eventData)
    {
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(_me, new BaseEventData(eventSystem));
    }

    //--- Public Methods ------------------------

    //--- Private Methods ------------------------
}
