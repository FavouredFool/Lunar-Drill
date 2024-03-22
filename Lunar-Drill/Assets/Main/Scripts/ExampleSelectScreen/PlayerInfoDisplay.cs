using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoDisplay : MonoBehaviour
{
    [SerializeField] Transform _LunaPosition; // Position to which the Object will Move if Luna is selected.
    [SerializeField] Transform _DrillianPosition; // Position to which the Object will Move if Luna is selected.
                                                  // [SerializeField] string _playerName; //Name of the Player

    [SerializeField] GameObject _select;
    [SerializeField] GameObject _ready;

    PlayerConnectController.ChosenCharacter _chosenCharacter; // Character that is displayed by this Info Display
    bool _characterReady = false;

    private void Awake()
    {
        UpdateDisplay();
    }
    private void UpdateDisplay()
    {
        if (_characterReady)
        {
            _ready.SetActive(true);
            _select.SetActive(false);
        }
        else
        {
            _ready.SetActive(false);
            _select.SetActive(true);
        }


        if(_chosenCharacter == PlayerConnectController.ChosenCharacter.drillian)
        {
            transform.position =  _DrillianPosition.position;
            
        }
        else if(_chosenCharacter == PlayerConnectController.ChosenCharacter.luna)
        {
            transform.position = _LunaPosition.position;

        }
    }

    public void ChosenCharacterChanged(PlayerConnectController.ChosenCharacter ch)
    {
        _chosenCharacter = ch;
        UpdateDisplay();
    }

    public void ReadyStateChanged(bool state)
    {
        _characterReady = state;
        UpdateDisplay();
    }


}
