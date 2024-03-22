using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNumberDisplay : MonoBehaviour
{
    [SerializeField] private SelectScreenHandling _selectScreen;


    [SerializeField] GameObject[] _playerMessagesImages = new GameObject[3];

    private void Awake()
    {
        _selectScreen.PlayerNumberChanged.AddListener((int n)=>DisplayNumberOfPlayers(n));
        DisplayNumberOfPlayers(0);
    }


    private void DisplayNumberOfPlayers(int playerNumber)
    {
        for(int i = 0; i < _playerMessagesImages.Length; i++)
        {
            if (i == playerNumber)
            {
                _playerMessagesImages[i].SetActive(true);
            }
            else
            {
                _playerMessagesImages[i].SetActive(false);
            }
        }
    }
}
