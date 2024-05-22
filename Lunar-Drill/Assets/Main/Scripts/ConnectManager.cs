using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ConnectManager : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    bool oldUsed = false;
    [Header("UI")]
    [SerializeField] SelectScreen UI;
    [SerializeField] private SelectMenuUIManager _uiManager;

    // --- Public Fields ------------------------
    public List<PlayerConnectController> connectedPlayers = new();

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

    //--- Private Fields ------------------------
    private List<PlayerInput> playerInputs; // Inputs

    private bool _p1NewActionRequired = false; // Has the player to end their action before making a new one
    private bool _p2NewActionRequired = false;
    private bool _p1Play = false; // Is the player ready to play
    private bool _p1Swap = false; // Is the player ready to swap
    private bool _p2Play = false;
    private bool _p2Swap = false;

    //--- Unity Methods ------------------------

    public void Awake()
    {
        playerInputs = new();
        if (oldUsed) UI.SetEmpty();
    }

    private void Update()
    {
        if (IsControlled) return;

        // P1 has to end their action to make a new one
        if (_p1NewActionRequired)
        {
            // Is not executing any action at the moment means they have had to stop previous action and can know make a new one
            if (!connectedPlayers[0].ReadyDown && !connectedPlayers[0].SwapDown) _p1NewActionRequired = false;
        }

        // P2 has to end their action to make a new one
        if (_p2NewActionRequired)
        {
            // Is not executing any action at the moment means they have had to stop previous action and can know make a new one
            if (!connectedPlayers[1].ReadyDown && !connectedPlayers[1].SwapDown) _p2NewActionRequired = false;
        }



        bool p1, p2;

        //Play
        if (oldUsed)
        {
            if (AllPlayerReady(out p1, out p2)) ReadyTime += Time.deltaTime;
            else ReadyTime = 0;
            if (oldUsed) UI.RefreshReady(p1, p2, ReadyTime);

            if (AgreeTime <= ReadyTime)
            {
                Play();
                return;
            }

            //Swap
            if (isMultiplayer && AllPlayerSwap(out p1, out p2)) SwapTime += Time.deltaTime;
            else SwapTime = 0;
            if (oldUsed) UI.RefreshSwap(p1, p2, SwapTime);

            if (isMultiplayer && AgreeTime <= SwapTime)
                Swap();
        }
        else
        {
            // If in Singleplayer and user presses ready button, start game.
            if (isSingleplayer)
            {
                if (!_p1NewActionRequired && connectedPlayers[0].ReadyDown)
                {
                    Play();
                    return;
                }
            }
            else if (isMultiplayer)
            {
                // --- Interpreting inputs ---
                if (!_p1NewActionRequired && connectedPlayers[0].SwapDown) // P1 changes ready to swap status
                {
                    if (_p1Swap) _p1Swap = false;
                    else
                    {
                        _p1Play = false; // Can't be ready to play if ready to swap
                        _p1Swap = true;
                    }
                    _p1NewActionRequired = true; // Require new action so same action is not performed multiple times
                }
                if (!_p1NewActionRequired && connectedPlayers[0].ReadyDown) // P1 changes ready to play status
                {
                    if (_p1Play) _p1Play = false;
                    else
                    {
                        _p1Swap = false; // Can't be ready to swap if ready to play
                        _p1Play = true;
                    }
                    _p1NewActionRequired = true; // Require new action so same action is not performed multiple times
                }
                if (!_p2NewActionRequired && connectedPlayers[1].SwapDown) // P2 changes ready to swap status
                {
                    if (_p2Swap) _p2Swap = false;
                    else
                    {
                        _p2Play = false; // Can't be ready to play if ready to swap
                        _p2Swap = true;
                    }
                    _p2NewActionRequired = true; // Require new action so same action is not performed multiple times
                }
                if (!_p2NewActionRequired && connectedPlayers[1].ReadyDown) // P2 changes ready to play status
                {
                    if (_p2Play) _p2Play = false;
                    else
                    {
                        _p2Swap = false; // Can't be ready to swap if ready to play
                        _p2Play = true;
                    }
                    _p2NewActionRequired = true; // Require new action so same action is not performed multiple times
                }


                _uiManager.RefreshMulti(_p1Play, _p1Swap, _p2Play, _p2Swap); // Refreshing UI

                // --- Swap if both ready (favor swapping over playing) ---
                if (_p1Swap && _p2Swap)
                {
                    Swap();
                    _p1Swap = false; // Not ready to swap anymore
                    _p2Swap = false;
                    return;
                }
                // --- Play if both ready ---
                if (_p1Play && _p2Play)
                {
                    Play();
                    return;
                }

            }
        }

        // What is this doing? 
        ////Tiggle 
        //if (connectedPlayers.Count > 0 && connectedPlayers[0].Tiggle)
        //{
        //    connectedPlayers[0].Tiggle = false;
        //    if (oldUsed) UI.Tiggle(true);
        //}
        //if (connectedPlayers.Count > 1 && connectedPlayers[1].Tiggle)
        //{
        //    connectedPlayers[1].Tiggle = false;
        //    if (oldUsed) UI.Tiggle(false);
        //}
    }

    //--- Public Methods ------------------------
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
        if (!oldUsed) _uiManager.Swap(MultiplayerLunaUp);

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

                // Make sure game does not get started by same action that joins player
                _p1NewActionRequired = true;
            }
            if (connectedPlayers.Count == 2)
            {
                connectedPlayers[0].SetCharacter(ChosenCharacter.luna);
                connectedPlayers[1].SetCharacter(ChosenCharacter.drillian);

                // Make sure p2 ready to play gets not set by same action that joins player
                _p2NewActionRequired = true;

                if (oldUsed) UI.SetMulti(MultiplayerLunaUp);
                if (!oldUsed) _uiManager.SetMulti();
            }
        }
    }
}
