using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConnectManager : MonoBehaviour
{
    public List<PlayerConnectController> connectedPlayers = new();
    [SerializeField] private SelectScreenHandling _selectScreenHandling; // reference to the script that handles the Select screen.
    [SerializeField] PlayerInfoDisplay _p1Info;
    [SerializeField] PlayerInfoDisplay _p2Info;


    List<PlayerInput> playerInputs;

    public int NumberConnectedPlayers {
        get
        {
            return connectedPlayers.Count;
        }

    }

    public bool CoopGameReady { // A bool to see if there are two players of different chosen characters. -> Local Coop is available.
        get
        {
            if (connectedPlayers.Count<2)
            {
                return false;
            }
            foreach (var player in connectedPlayers)
            {
                if (!player.Ready) 
                { 
                    return false;
                }
            }
            return connectedPlayers[0].Character != connectedPlayers[1].Character;
        }
    }

    public bool SinglePlayerGameReady // A Bool to see if there is at least one player.
    {
        get 
        {
            return connectedPlayers.Count==1 && connectedPlayers[0].Ready; 
        }
    }

    public void Awake()
    {
        playerInputs = new();
    }

    public void PlayerJoined(PlayerInput input)
    {
        playerInputs.Add(input);

        // When a player joins add them to the list that saves their information.
        PlayerConnectController playerConnectInfo = input.gameObject.GetComponent<PlayerConnectController>();

        if (playerConnectInfo != null)
        {
            connectedPlayers.Add(playerConnectInfo);
            if (connectedPlayers.Count == 1)
            {
                _p1Info.gameObject.SetActive(true);
                playerConnectInfo.ChosenCharacterChanged.AddListener((value) => _p1Info.ChosenCharacterChanged(value));
                playerConnectInfo.ReadyStateChanged.AddListener((value) => _p1Info.ReadyStateChanged(value));
            }
            if (connectedPlayers.Count == 2)
            {
                _p2Info.gameObject.SetActive(true);
                playerConnectInfo.ChosenCharacterChanged.AddListener((value) => _p2Info.ChosenCharacterChanged(value));
                playerConnectInfo.ReadyStateChanged.AddListener((value) => _p2Info.ReadyStateChanged(value));
            }
        }
        // If there are more than one player unready all of them and set them to coop mode.
        if (connectedPlayers.Count > 1)
        {
            foreach (PlayerConnectController pcc in connectedPlayers)
            {
                Debug.Log($"Pre: {pcc.Character}");
                pcc.Ready = false;
                pcc.CoopReady();
                Debug.Log($"post: {pcc.Character}");
            }
        }
        _selectScreenHandling.PlayerConnected(playerConnectInfo);
        
        

    }
}
