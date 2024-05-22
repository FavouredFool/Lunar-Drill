using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ConnectManager : MonoBehaviour
{
    bool oldUsed = false;
    [SerializeField] SelectScreen UI;
    [SerializeField] private SelectMenuUIManager _uiManager;

    public List<PlayerConnectController> connectedPlayers = new();
    List<PlayerInput> playerInputs;

    public int NumberConnectedPlayers
    {
        get
        {
            return connectedPlayers.Count;
        }
    }
    public bool isSingleplayer => NumberConnectedPlayers == 1;
    public bool isMultiplayer => NumberConnectedPlayers > 1;

    public bool MultiplayerLunaUp { get; set; } = true;
    public bool IsControlled { get; set; } = false;

    public const float AgreeTime = .5f;

    public float ReadyTime { get; private set; } = 0;
    public float SwapTime { get; private set; } = 0;

    public bool AllPlayerReady(out bool p1, out bool p2)
    {
        p1 = false;
        p2 = false;
        bool Down = true;
        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            PlayerConnectController c = connectedPlayers[i];
            bool isDown = c.ReadyDown;
            if (i == 0) p1 = isDown;
            else p2 = isDown;

            if (!isDown) Down = false;
        }
        return Down;
    }
    public bool AllPlayerSwap(out bool p1, out bool p2)
    {
        p1 = false;
        p2 = false;
        bool Down = true;
        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            PlayerConnectController c = connectedPlayers[i];
            bool isDown = c.SwapDown;
            if (i == 0) p1 = isDown;
            else p2 = isDown;

            if (!isDown) Down = false;
        }
        return Down;
    }

    public void Awake()
    {
        playerInputs = new();
        if (oldUsed) UI.SetEmpty();
    }

    private void Update()
    {
        if (IsControlled) return;

        bool p1, p2;

        //Play
        if (AllPlayerReady(out p1, out p2)) ReadyTime += Time.deltaTime;
        else ReadyTime = 0;
        if (oldUsed) UI.RefreshReady(p1, p2, ReadyTime);
        if (!oldUsed) _uiManager.RefreshReady(p1, p2, ReadyTime);

        if (AgreeTime <= ReadyTime)
        {
            Play();
            return;
        }

        //Swap
        if (isMultiplayer && AllPlayerSwap(out p1, out p2)) SwapTime += Time.deltaTime;
        else SwapTime = 0;
        if (oldUsed) UI.RefreshSwap(p1, p2, SwapTime);
        if (!oldUsed) _uiManager.RefreshSwap(p1, p2, SwapTime);

        if (isMultiplayer && AgreeTime <= SwapTime)
            Swap();

        //Tiggle
        if (connectedPlayers.Count > 0 && connectedPlayers[0].Tiggle)
        {
            connectedPlayers[0].Tiggle = false;
            if (oldUsed) UI.Tiggle(true);
        }
        if (connectedPlayers.Count > 1 && connectedPlayers[1].Tiggle)
        {
            connectedPlayers[1].Tiggle = false;
            if (oldUsed) UI.Tiggle(false);
        }
    }

    public void Swap()
    {
        if (!isMultiplayer) return;

        PlayerConnectController
            p1 = connectedPlayers[0],
            p2 = connectedPlayers[1];

        MultiplayerLunaUp = !MultiplayerLunaUp;

        connectedPlayers[0].SetCharacter(MultiplayerLunaUp ? ChosenCharacter.luna : ChosenCharacter.drillian);
        connectedPlayers[1].SetCharacter(MultiplayerLunaUp ? ChosenCharacter.drillian : ChosenCharacter.luna);

        if (oldUsed) UI.Swap();
        if (oldUsed) UI.SetMulti(MultiplayerLunaUp);

        SwapTime = 0;

        Debug.Log("SWAP");

        IsControlled = true;
        DOVirtual.DelayedCall(0.43f, () =>
        {
            IsControlled = false;
        });

    }
    public void Play()
    {
        if (NumberConnectedPlayers == 0) return;

        if (oldUsed) UI.Play();
        if (!oldUsed) _uiManager.Play();

        ReadyTime = 0;

        Debug.Log("PLAY");

        IsControlled = true;
        //DOVirtual.DelayedCall(0.1f, () => SceneManager.LoadScene("MainScene"));
    }

    public void PlayerJoined(PlayerInput input)
    {
        playerInputs.Add(input);
        PlayerConnectController playerConnectInfo = input.gameObject.GetComponent<PlayerConnectController>();

        if (playerConnectInfo != null)
        {
            connectedPlayers.Add(playerConnectInfo);
            MultiplayerLunaUp = true;

            if (connectedPlayers.Count == 1)
            {
                connectedPlayers[0].SetCharacter(ChosenCharacter.singleplayer);
                if (oldUsed) UI.SetSolo();
                if (!oldUsed) _uiManager.SetSolo();
            }
            if (connectedPlayers.Count == 2)
            {
                connectedPlayers[0].SetCharacter(ChosenCharacter.luna);
                connectedPlayers[1].SetCharacter(ChosenCharacter.drillian);

                if (oldUsed) UI.SetMulti(MultiplayerLunaUp);
                if (!oldUsed) _uiManager.SetMulti();
            }
        }
    }
}
