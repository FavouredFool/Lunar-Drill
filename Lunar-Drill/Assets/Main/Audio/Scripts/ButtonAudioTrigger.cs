using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Class to attach to a button that should fire events.
public class ButtonAudioTrigger : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    Button _button; // this elements button

    private void Start()
    {
        _button = GetComponent<Button>();
        if(_button!=null)
            _button.onClick.AddListener(() => OnEnter());
    }

    public void OnEnter() // Method that is called when a Button is confirmed. Works only for Buttons
    {
        Debug.Log($"Clicked: {gameObject}");
        AudioController.Fire(new MenuClickAudio($"Clicked: {gameObject}"));
    }

    
    public void OnSelect(BaseEventData eventData) // Method that is called when a button is selected
    {
        AudioController.Fire(new MenuSelectAudio($"Selected: {gameObject}")); // Signal to the audio controller
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelect(null);
    }
}
