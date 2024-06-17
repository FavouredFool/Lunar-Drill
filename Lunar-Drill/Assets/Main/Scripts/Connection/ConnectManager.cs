using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;

public class ConnectManager : MonoBehaviour
{
    private List<PlayerInput> playerInputs= new();
    public List<PlayerConnectController> connectedPlayers = new();

    
    public CoopButton_Divided
        Both_Reroll,P1_Reroll, P2_Reroll;
    public TMP_Text Reroll_Text;
    bool nameSeeded;

    public int NumberConnectedPlayers
    {
        get
        {
            return connectedPlayers.Count;
        }
    }
    public bool isSingleplayer => NumberConnectedPlayers == 1;
    public bool isMultiplayer => NumberConnectedPlayers > 1;

    private void Start()
    {
        PreparationInterface.instance.SetPlayerInfo(0);

        Both_Reroll.transform.parent.localScale = Vector3.zero;
        P1_Reroll.transform.parent.localScale = Vector3.zero;
        P2_Reroll.transform.parent.localScale = Vector3.zero;
        Reroll_Text.transform.localScale = Vector3.zero;

        nameSeeded = false;
    }
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

                PreparationInterface.instance.SetPlayerInfo(1);

                Both_Reroll.blocked = false;
                Both_Reroll.transform.parent.DOScale(1, 0.33f).SetEase(Ease.OutSine);
                Reroll_Text.transform.DOScale(1, 0.33f).SetEase(Ease.OutBack);
            }
            if (connectedPlayers.Count == 2)
            {
                connectedPlayers[0].SetCharacter(ChosenCharacter.luna);
                connectedPlayers[1].SetCharacter(ChosenCharacter.drillian);

                PreparationInterface.instance.SetPlayerInfo(2);

                Both_Reroll.blocked = true;
                Both_Reroll.transform.parent.DOScale(0, 0.33f).SetEase(Ease.OutSine);
                P1_Reroll.blocked = false;
                P1_Reroll.transform.parent.DOScale(1, 0.33f).SetEase(Ease.OutSine);
                P2_Reroll.blocked = false;
                P2_Reroll.transform.parent.DOScale(1, 0.33f).SetEase(Ease.OutSine);
            }

            PlayerConnectController.isSolo = isSingleplayer;
            if (!nameSeeded)
            {
                NameManager.instance.RandomizeBoth(0);
                nameSeeded = true;
            }
        }
    }

}
