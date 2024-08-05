using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

public class ConnectManager : MonoBehaviour, IInputSubscriber<PlayerModeReset>, IInputSubscriber<PlayerModeChanged>
{
    public static ConnectManager instance;
    public static bool isOpen { get; private set; }
    public PlayerInputManager _playerInputManager;

    private List<PlayerInput> playerInputs = new();
    public List<PlayerConnectController> connectedPlayers = new();

    public static bool isSolo;
    public static int TargetConnectedPlayers => isSolo ? 1 : 2;

    [SerializeField]
    GameObject
        body,
        P1Screen, P2Screen,
        P1Con, P2Con,
        P1Conf, P2Conf;

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
    public void SetUp()
    {

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

        RefreshMenu();
        Check(!isSolo);
    }
    public void PlayerLeft()
    {
        ResetConnections();
        Check(!isSolo);
    }

    public void RefreshMenu()
    {
        P1Screen.SetActive(true);
        P1Screen.SetActive(!isSolo);

        int connected = connectedPlayers.Count;

        P1Con.SetActive(connected <= 0);
        P2Con.SetActive(connected <= 1);
        P1Conf.SetActive(connected > 0);
        P2Conf.SetActive(connected > 1);
    }

    public void Check(bool coop)
    {
        isSolo = !coop;

        if (connectedPlayers.Count != TargetConnectedPlayers)
            Open();
        else
        {
            Close();
            PlayerConnectController.Enable();
            InputBus.Fire(new PlayerModeConfirmed());
        }

        SetMenuMode(isOpen);
    }

    public async void Open()
    {
        if (isOpen) return;
        isOpen = true;

        ResetConnections();

        _playerInputManager.gameObject.SetActive(false);
        body.gameObject.SetActive(true);

        await Task.Delay(10);

        _playerInputManager.EnableJoining();
        _playerInputManager.gameObject.SetActive(true);
    }
    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        body.gameObject.SetActive(false);

    }

    public void SetMenuMode(bool on)
    {
        if (PlayerConnectController.Drillian)
            PlayerConnectController.Drillian.SetMenuMode(on);
        if (PlayerConnectController.Luna)
            PlayerConnectController.Luna.SetMenuMode(on);
    }

    public void OnEventHappened(PlayerModeChanged e) 
        => Check(e.Coop);
    public void OnEventHappened(PlayerModeReset e) 
        => ResetConnections();
}
