using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    [SerializeField] string[] _introTexts;
    [SerializeField] TMP_Text[] _introTextfields;

    [SerializeField] TMP_Text[] _countdownTextfield;

    [SerializeField] HUDisplay
        _playerHUD,
        _spiderHUD;

    [SerializeField] int _maxPlayerHP, _maxSpiderHP;

    int _playerHP;
    int _spiderHP;

    public void Awake()
    {
        PlayerHP = _maxPlayerHP;
        SpiderHP = _maxSpiderHP;
    }

    public void Start()
    {
        //StartCoroutine(StartSequence());
    }

    public IEnumerator StartSequence()
    {
        Assert.IsTrue(_introTexts.Length == 4);
        Assert.IsTrue(_introTextfields.Length == 4);

        yield return new WaitForSeconds(1.5f);

        _introTextfields[0].text = _introTexts[0];

        yield return new WaitForSeconds(1.5f);

        _introTextfields[1].text = _introTexts[1];

        yield return new WaitForSeconds(1.5f);

        _introTextfields[2].text = _introTexts[2];

        yield return new WaitForSeconds(3f);

        _introTextfields[3].text = _introTexts[3];

        yield return new WaitForSeconds(3f);

        //for(int i = 3; i )

    }

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

    

    public void EndGame(bool playerVictory)
    {
        Debug.Log(playerVictory? "VICTORY!":"GAME OVER!");
    }
}
