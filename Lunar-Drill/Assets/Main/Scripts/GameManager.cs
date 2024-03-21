using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text[] _introTextfields;

    [SerializeField] TMP_Text _countdownTextfield;

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
        StartCoroutine(StartSequence());
    }

    public IEnumerator StartSequence()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();

        Time.timeScale = 0;

        Assert.IsTrue(_introTextfields.Length == 4);

        yield return new WaitForSecondsRealtime(2f);

        _introTextfields[0].gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        _introTextfields[1].gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        _introTextfields[2].gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        _introTextfields[3].gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        _countdownTextfield.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            _countdownTextfield.text = i.ToString();

            yield return new WaitForSecondsRealtime(1f);
        }

        _countdownTextfield.text = "GO!";

        yield return new WaitForSecondsRealtime(0.5f);

        _introTextfields[0].gameObject.SetActive(false);
        _introTextfields[1].gameObject.SetActive(false);
        _introTextfields[2].gameObject.SetActive(false);
        _introTextfields[3].gameObject.SetActive(false);
        _countdownTextfield.gameObject.SetActive(false);

        // les go
        Debug.Log("GO");

        Time.timeScale = 1;
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
