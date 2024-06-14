using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConnectManager : MonoBehaviour
{
    private List<PlayerInput> playerInputs= new();
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

    public void PlayerJoined(PlayerInput input)
    {
        playerInputs.Add(input);
        PlayerConnectController playerConnectInfo = input.gameObject.GetComponent<PlayerConnectController>();

        if (playerConnectInfo != null)
        {
            connectedPlayers.Add(playerConnectInfo);

            if (connectedPlayers.Count == 1)
            {
                connectedPlayers[0].SetCharacter(ChosenCharacter.both);

            }
            if (connectedPlayers.Count == 2)
            {
                connectedPlayers[0].SetCharacter(ChosenCharacter.luna);
                connectedPlayers[1].SetCharacter(ChosenCharacter.drillian);
            }
        }
    }

}
