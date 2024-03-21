using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNumberDisplay : MonoBehaviour
{
    [SerializeField] private SelectScreenHandling _selectScreen;    
    

    [SerializeField] string[] _playersMessage = new string[3]; // Array to save a message for zero to two players
    TMP_Text _text;

    private void Awake()
    {
        _selectScreen.PlayerNumberChanged.AddListener((int n)=>DisplayNumberOfPlayers(n));
        _text = GetComponent<TMP_Text>();
        DisplayNumberOfPlayers(0);
    }


    private void DisplayNumberOfPlayers(int playerNumber)
    {
        _text.SetText(_playersMessage[playerNumber]);
    }
}
