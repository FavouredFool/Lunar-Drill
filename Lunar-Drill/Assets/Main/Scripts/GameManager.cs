using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text[] _introTextfields;

    [SerializeField] TMP_Text _countdownTextfield;

    [SerializeField] TimeManager _timeManager;

    [SerializeField] HUDisplay
        _playerHUD,
        _spiderHUD;

    [SerializeField] int _maxPlayerHP, _maxSpiderHP;

    int _playerHP;
    int _spiderHP;

    public bool inCutscene { get; private set; } = false;

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

        inCutscene = true;

        Assert.IsTrue(_introTextfields.Length == 4);

        yield return new WaitForSecondsRealtime(2f);

        _introTextfields[0].gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        _introTextfields[1].gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        _introTextfields[2].gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        _introTextfields[3].gameObject.SetActive(true);

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

        inCutscene = false;
        Time.timeScale = 1;
    }

    public int PlayerHP { get => _playerHP; 
        set 
        {
            _playerHP = value; 
            _playerHUD.RefreshHearts(_playerHP);
            if (_playerHP == 0) EndGame(false);
            else TimeManager.main.HitFrame(); //Add Screenshake
        }
    }
    public int SpiderHP { get => _spiderHP; 
        set 
        { 
            _spiderHP = value; 
            _spiderHUD.RefreshHearts(_spiderHP);
            if (_spiderHP == 0) EndGame(true);
            else TimeManager.main.HitFrame(); //Add Screenshake
        }
    }

    

    public void EndGame(bool playerVictory)
    {
        Debug.Log(playerVictory? "VICTORY!":"GAME OVER!");
    }
}