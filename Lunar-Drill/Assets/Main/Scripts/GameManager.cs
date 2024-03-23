using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _introTextImages;
    [SerializeField] List<GameObject> _countdownTextImages;

    [SerializeField] TimeManager _timeManager;
    [SerializeField] Undertaker _undertaker;

    [SerializeField]
    HUDisplay
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

        Assert.IsTrue(_introTextImages.Count == 4);

        yield return new WaitForSecondsRealtime(2f);

        _introTextImages[0].SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        _introTextImages[1].SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        _introTextImages[2].SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        _introTextImages[3].SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        _introTextImages[3].SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (j == i)
                {
                    _countdownTextImages[j].SetActive(true);
                }
                else
                {
                    _countdownTextImages[j].SetActive(false);
                }
            }

            yield return new WaitForSecondsRealtime(1f);
        }

        _countdownTextImages[2].SetActive(false);
        _countdownTextImages[3].SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f);

        _introTextImages[0].SetActive(false);
        _introTextImages[1].SetActive(false);
        _introTextImages[2].SetActive(false);
        _countdownTextImages[3].SetActive(false);

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
            _playerHP = Mathf.Min(_maxPlayerHP, _playerHP);
            _playerHUD.RefreshHearts(_playerHP);
        }
        else
        {
            _spiderHP++;
            _maxSpiderHP = Mathf.Min(_maxSpiderHP, _spiderHP);
            _spiderHUD.RefreshHearts(_spiderHP);
        }
    }

    public void EndGame(GameObject obj, bool playerVictory)
    {
        Debug.Log(playerVictory ? "VICTORY!" : "GAME OVER!");
        Time.timeScale = 0;
        _undertaker.Activate(obj, playerVictory);
    }
}