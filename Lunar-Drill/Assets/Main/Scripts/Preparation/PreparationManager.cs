using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PreparationManager : MonoBehaviour, IInputSubscriber<PlayerModeConfirmed>
{
    [SerializeField] ModeSelection _modeSelection;
    [SerializeField] NameSelection _nameSelection;
    [SerializeField] ConnectManager _connection;
    [SerializeField] Transform _characters;

    private void OnEnable()
    {
        InputBus.Subscribe(this);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe(this);
    }

    private void Start()
    {
        _connection.Set(false);
        _modeSelection.Set(false);
        _nameSelection.Set(false);
        _characters.localScale = Vector3.zero;

        _modeSelection.Open();
    }

    public void SelectMode(bool coop)
    {
        _modeSelection.Close();

        if (_connection.Valid)
        {
            _nameSelection.Open(0.3f);
            _characters.DOScale(1,0.33f).SetDelay(0.3f).SetEase(Ease.OutSine);
            NameManager.instance?.RandomizeBoth(0);
        }
        else
            _connection.Open(0.3f);

        InputBus.Fire(new PlayerModeChanged(coop));
    }
    public void ConfirmConnection()
    {
        _connection.Close();

        _nameSelection.Open(0.3f);
        _characters.DOScale(1, 0.33f).SetDelay(0.3f).SetEase(Ease.OutSine);

        NameManager.instance?.RandomizeBoth(0);
    }

    public void Back()
    {
        _nameSelection.Close();
        _characters.DOScale(0, 0.25f).SetEase(Ease.InSine);
        _connection.Set(false);

        InputBus.Fire(new PlayerModeReset());

        _modeSelection.Open(0.3f);

    }
    public void Continue()
    {
        _nameSelection.Close();
    }

    public void OnEventHappened(PlayerModeConfirmed e) => ConfirmConnection();
}
