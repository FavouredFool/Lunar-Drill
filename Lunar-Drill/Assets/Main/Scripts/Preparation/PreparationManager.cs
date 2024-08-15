using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PreparationManager : MonoBehaviour, IInputSubscriber<Signal_SceneChange>
{
    [SerializeField] ModeSelection _modeSelection;
    [SerializeField] NameSelection _nameSelection;
    [SerializeField] ConnectManager _connection;
    [SerializeField] Transform _characters;

    private void OnEnable()
    {
        InputBus.Subscribe<Signal_SceneChange>(this);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe<Signal_SceneChange>(this);
    }

    private void Start()
    {
        _connection.Set(false);
        _modeSelection.Set(false);
        _nameSelection.Set(false);
        _characters.localScale = Vector3.zero;

        _modeSelection.Open();

        PlayerConnectController.Exhaust();
    }

    public void SelectMode(bool coop)
    {
        _modeSelection.Close();

        Debug.Log("CONNECTMANAGER -->> " + ConnectManager.Valid);

        InputBus.Fire(new PlayerModeChanged(coop));

        if (ConnectManager.Valid)
        {
            _nameSelection.Open(0.3f);
            _characters.DOScale(1,0.33f).SetDelay(0.3f).SetEase(Ease.OutSine);
            NameManager.instance?.RandomizeBoth(0);

            InputBus.Fire(new PlayerModeConfirmed());
        }
        else
            _connection.Open(0.3f);


        PlayerConnectController.Exhaust();
    }
    public void ConfirmConnection()
    {
        _connection.Close();

        _nameSelection.Open(0.66f);
        _characters.DOScale(1, 0.33f).SetDelay(0.66f).SetEase(Ease.OutSine);

        NameManager.instance?.RandomizeBoth(0);

        PlayerConnectController.Exhaust();
    }

    public void Back()
    {
        if (_nameSelection.isOpen)
            _nameSelection.Close();
        if (ConnectManager.isOpen)
            _connection.Close();

        _characters.DOScale(0, 0.25f).SetEase(Ease.InSine);

        InputBus.Fire(new PlayerModeReset());

        _modeSelection.Open(0.3f);

        PlayerConnectController.Exhaust();
    }
    public void Continue()
    {
        _nameSelection.Close();
    }


    public void OnEventHappened(Signal_SceneChange e)
    {
        if (e.scene!=SceneIdentity.GameTutorial)
            _characters.DOScale(0, e.delay).SetEase(Ease.InSine);
    }
}
