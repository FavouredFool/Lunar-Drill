using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoDisplay : MonoBehaviour
{
    [SerializeField] Transform _LunaPosition; // Position to which the Object will Move if Luna is selected.
    [SerializeField] Transform _DrillianPosition; // Position to which the Object will Move if Luna is selected.
    [SerializeField] string _playerName; //Name of the Player
    
    PlayerConnectController.ChosenCharacter _chosenCharacter; // Character that is displayed by this Info Display
    bool _characterReady = false;
    TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        UpdateDisplay();
    }
    private void UpdateDisplay()
    {
        string Ready = _characterReady ? " Ready!" : "?";
        _text.text = $"{_playerName}{Ready}";
        if(_chosenCharacter == PlayerConnectController.ChosenCharacter.drillian)
        {
            transform.position =  _DrillianPosition.position;
            
        }
        else if(_chosenCharacter == PlayerConnectController.ChosenCharacter.luna)
        {
            transform.position = _LunaPosition.position;

        }
        _text.color = Color.white;
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
