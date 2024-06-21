using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text _countdownNumber;

    [Header("Agents")]
    public SpiderController _spiderController;
    public DrillianController _drillianController;
    public LunaController _lunaController;
    public OreSpawner _oreSpawner;

    [Header("Elements")]
    public Transform _camera;

    [Header("Managers")]
    [SerializeField] TimeManager _timeManager;
    [SerializeField] NewUndertaker _undertaker;

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

    public float Timer { get; set; }

    public void Awake()
    {
        SetHealth(_maxPlayerHP, true);
        SetHealth(_maxSpiderHP, false);
        _undertaker.Close();

        StartCoroutine(StartRoutine());

    }

    private void Update()
    {
//#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            EndGame(_spiderController.gameObject, true, false);
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            EndGame(_drillianController.gameObject, false, false);
//#endif
    }

    public IEnumerator StartRoutine()
    {
        //_oreSpawner.enabled = false;
        //_lunaController.enabled = false;
        //_drillianController.enabled = false;
        //_spiderController.enabled = false;
        _timeManager.Freeze();

        _camera.localPosition = Vector3.down * 10f;
        _playerHUD.transform.localScale = Vector3.zero;
        _spiderHUD.transform.localScale = Vector3.zero;

        _playerHUD.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetDelay(3f).SetUpdate(true);
        _spiderHUD.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetDelay(3f).SetUpdate(true);
        _camera.DOLocalMoveY(0, 3).SetEase(Ease.InOutSine).SetUpdate(true);

        _countdownNumber.gameObject.SetActive(true);
        _countdownNumber.text = "";

        yield return new WaitForSecondsRealtime(0.5f);

        _countdownNumber.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        yield return new WaitForSecondsRealtime(1f);

        _countdownNumber.text = "2";
        yield return new WaitForSecondsRealtime(1f);

        _countdownNumber.text = "1";
        yield return new WaitForSecondsRealtime(1f);

        _countdownNumber.text = "Go!";

        yield return new WaitForSecondsRealtime(1f);

        _countdownNumber.gameObject.SetActive(false);

        _timeManager.Unfreeze();

        _oreSpawner.enabled = true;
        _lunaController.enabled = true;
        _drillianController.enabled = true;
        _spiderController.enabled = true;

        Timer = Time.time;
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

    public void EndGame(GameObject obj, bool playerVictory, bool addLeaderboard = true)
    {
        Debug.Log(playerVictory ? "VICTORY!" : "GAME OVER!");

        if (playerVictory && addLeaderboard)
            FindObjectOfType<LeaderboardManager>().AddEntry(Time.time - FindObjectOfType<GameManager>().Timer, NameManager.LunaTeamName, NameManager.DrillianTeamName);

        _undertaker.Open(obj, playerVictory);
    }
}