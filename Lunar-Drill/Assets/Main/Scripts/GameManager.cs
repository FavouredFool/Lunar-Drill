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
    [SerializeField] Undertaker _undertaker;

    [SerializeField] HUDisplay
        _playerHUD,
        _spiderHUD;

    [SerializeField] int _maxPlayerHP, _maxSpiderHP;

    int _playerHP;
    int _spiderHP;

    public int PlayerHP => _playerHP;
    public int SpiderHP => _spiderHP;
    public int PlayerMaxHP => _maxPlayerHP;
    public int SpiderMaxHP => _maxSpiderHP;

    public bool inCutscene { get; private set; } = false;

    public void Awake()
    {
        SetHealth(_maxPlayerHP, true);
        SetHealth(_maxSpiderHP, false);
    }

    public void Start()
    {
#if !UNITY_EDITOR
        StartCoroutine(StartSequence());
#endif
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

        yield return new WaitForSecondsRealtime(1f);

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

    public void SetHealth(int amount, bool isPlayer)
    {
        if (isPlayer)
        {
            _playerHP = amount;
            _playerHUD.RefreshHearts(_playerHP);
        }
        else
        {
            _spiderHP = amount;
            _spiderHUD.RefreshHearts(_spiderHP);
        }
    }
    public void Hit(GameObject obj, bool isPlayer)
    {
        if (isPlayer)
        {
            _playerHP--;
            _playerHP = Mathf.Max(0, _playerHP);
            _playerHUD.RefreshHearts(_playerHP);
            if (_playerHP == 0)
            {
                EndGame(obj, false);
                return;
            }
        }
        else
        {
            _spiderHP--;
            _spiderHP = Mathf.Max(0, _spiderHP);
            _spiderHUD.RefreshHearts(_spiderHP);
            if (_spiderHP == 0)
            {
                EndGame(obj, true);
                return;
            }
        }

        TimeManager.main.HitFrame(); 
        //Add Screenshake?
    }
    public void Heal(GameObject obj, bool isPlayer)
    {
        if (isPlayer)
        {
            _playerHP++;
            _playerHP = Mathf.Min(_maxPlayerHP,_playerHP);
            _playerHUD.RefreshHearts(_playerHP);
        }
        else
        {
            _spiderHP++;
            _maxSpiderHP = Mathf.Min(_maxSpiderHP, _spiderHP);
            _spiderHUD.RefreshHearts(_spiderHP);
        }
    }

    public void EndGame(GameObject obj,bool playerVictory)
    {
        Debug.Log(playerVictory? "VICTORY!":"GAME OVER!");
        Time.timeScale = 0;
        _undertaker.Activate(obj, playerVictory);
    }
}