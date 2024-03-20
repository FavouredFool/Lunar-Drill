using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConnectManager : MonoBehaviour
{
    List<PlayerInput> playerInputs;

    public void Awake()
    {
        playerInputs = new();
    }

    public void PlayerJoined(PlayerInput input)
    {
        playerInputs.Add(input);
    }
}
