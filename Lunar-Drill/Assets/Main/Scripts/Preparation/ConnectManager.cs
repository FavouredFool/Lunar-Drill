using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class ConnectManager : MonoBehaviour, IInputSubscriber<PlayerModeReset>, IInputSubscriber<PlayerModeChanged>
{
    public static ConnectManager instance;
    public static bool isOpen { get; private set; }
    public PlayerInputManager _playerInputManager;

    public static List<PlayerInput> playerInputs = new();
    public static List<PlayerConnectController> connectedPlayers = new();

    public static bool isCoop;
    public static int TargetConnectedPlayers => isCoop ? 2 : 1;
    public static bool Valid => connectedPlayers.Count == TargetConnectedPlayers;


    [SerializeField]
    Transform body;
        [SerializeField]
    GameObject
        P1Screen, P2Screen,
        P1Con, P2Con,
        SoloP1Conf,P1Conf, P2Conf;

    private void OnEnable()
    {
        InputBus.Subscribe<PlayerModeChanged>(this);
        InputBus.Subscribe<PlayerModeReset>(this);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe<PlayerModeChanged>(this);
        InputBus.Unsubscribe<PlayerModeReset>(this);
    }

    public void Set(bool open)
    {
        isOpen = open;
        body.localScale = open ? Vector3.one : Vector3.zero;

        if(open) _playerInputManager.EnableJoining();
        else _playerInputManager.DisableJoining();

        _playerInputManager.gameObject.SetActive(open);
        SetMenuMode(isOpen);

        RefreshMenu();
    }
    public async void Open(float delay=0)
    {
        if (isOpen) return;
        isOpen = true;

        ResetConnections();

        DOTween.Kill(body);
        body.DOScale(1, 0.3f)
            .SetEase(Ease.OutSine)
            .SetDelay(delay);

        _playerInputManager.gameObject.SetActive(false);

        await Task.Delay(10);

        _playerInputManager.EnableJoining();
        _playerInputManager.gameObject.SetActive(true);

        SetMenuMode(isOpen);
        RefreshMenu();
    }
    public void Close(float delay=0)
    {
        if (!isOpen) return;
        isOpen = false;

        DOTween.Kill(body);
        body.DOScale(0, 0.3f)
            .SetEase(Ease.InSine)
            .SetDelay(delay);

        _playerInputManager.DisableJoining();
        _playerInputManager.gameObject.SetActive(false);

        SetMenuMode(isOpen);
        RefreshMenu();
    }
    public bool ToggleValid(float delay=0)
    {
        if (!Valid)
            Open(delay);
        else
        {
            Close(delay);
            PlayerConnectController.Enable();
            InputBus.Fire(new PlayerModeConfirmed());
        }

        SetMenuMode(isOpen);

        return Valid;
    }
    public void RefreshMenu()
    {
        P1Screen.SetActive(true);
        P1Screen.SetActive(isCoop);

        int connected = connectedPlayers.Count;

        P1Con.SetActive(connected <= 0);
        P2Con.SetActive(connected <= 1);
        SoloP1Conf.SetActive(connected > 0 && !isCoop);
        P1Conf.SetActive(connected > 0 && isCoop);
        P2Conf.SetActive(connected > 1);
    }


    public void ResetConnections()
    {
        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            Destroy(connectedPlayers[i].gameObject);
        }
        playerInputs.Clear();
        connectedPlayers.Clear();

        RefreshMenu();
    }
    public void PlayerJoined(PlayerInput input)
    {
        PlayerConnectController playerConnectInfo = input.gameObject.GetComponent<PlayerConnectController>();

        playerInputs.Add(input);
        connectedPlayers.Add(playerConnectInfo);

        RefreshMenu();

        if (connectedPlayers.Count == 1)
        {
            connectedPlayers[0].SetCharacter(ChosenCharacter.both);

            Rumble.instance?.RumbleLuna(1, 0.5f, 0.33f);
        }
        if (connectedPlayers.Count == 2)
        {
            connectedPlayers[0].SetCharacter(ChosenCharacter.luna);
            connectedPlayers[1].SetCharacter(ChosenCharacter.drillian);

            Rumble.instance?.RumbleDrillian(1, 0.5f, 0.33f);
        }

        PlayerConnectController.isSolo = connectedPlayers.Count == 1;

        ToggleValid();
    }
    public void PlayerLeft()
    {
        ResetConnections();
        ToggleValid();
    }


    public void SetMenuMode(bool on)
    {
        if (PlayerConnectController.Drillian)
            PlayerConnectController.Drillian.SetMenuMode(on);
        if (PlayerConnectController.Luna)
            PlayerConnectController.Luna.SetMenuMode(on);
    }

    public void OnEventHappened(PlayerModeChanged e)
    {
        isCoop = e.Coop;
    }
    public void OnEventHappened(PlayerModeReset e) 
        => ResetConnections();
}
