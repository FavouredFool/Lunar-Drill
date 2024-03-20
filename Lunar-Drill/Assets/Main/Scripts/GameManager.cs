using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] HUDisplay
        _playerHUD,
        _spiderHUD;

    public int PlayerHP { get => _playerHP; 
        set 
        { 
            _playerHP = value; 
            _playerHUD.RefreshHearts(_playerHP);
            if (_playerHP == 0) EndGame(false);
        } 
    }
    public int SpiderHP { get => _spiderHP; 
        set 
        { 
            _spiderHP = value; 
            _spiderHUD.RefreshHearts(_spiderHP);
            if (_spiderHP == 0) EndGame(true);
        }
    }

    [SerializeField] int _playerHP, _spiderHP;

    public void EndGame(bool playerVictory)
    {
        Debug.Log(playerVictory? "VICTORY!":"GAME OVER!");
    }
}
