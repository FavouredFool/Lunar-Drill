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

    public TMP_Text
        P1_Text, P2_Text;
    public RectTransform
        P1_ConPrompt, P2_ConPrompt;
    public CoopButton_Divided
        Both_Reroll,P1_Reroll, P2_Reroll;
    public TMP_Text Reroll_Text;

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
        NameManager.instance.RandomizeBoth(0);

        Both_Reroll.transform.parent.localScale = Vector3.zero;
        P1_Reroll.transform.parent.localScale = Vector3.zero;
        P2_Reroll.transform.parent.localScale = Vector3.zero;
        Reroll_Text.transform.localScale = Vector3.zero;
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

                P1_Text.text = "P1\nSolo";
                P1_Text.transform.DOScale(1f,0.33f).SetEase(Ease.OutBack);
                P1_ConPrompt.DOScale(0f,0.15f).SetEase(Ease.InBack);
                Both_Reroll.blocked = false;
                Both_Reroll.transform.parent.DOScale(1, 0.33f).SetEase(Ease.OutSine);
                Reroll_Text.transform.DOScale(1, 0.33f).SetEase(Ease.OutBack);
            }
            if (connectedPlayers.Count == 2)
            {
                connectedPlayers[0].SetCharacter(ChosenCharacter.luna);
                connectedPlayers[1].SetCharacter(ChosenCharacter.drillian);

                P1_Text.text = "P1\nLuna";
                
                P2_Text.text = "P2\nDrillian";
                P2_Text.transform.DOScale(1f, 0.33f).SetEase(Ease.OutBack);
                P2_ConPrompt.DOScale(0f, 0.15f).SetEase(Ease.InBack);

                Both_Reroll.blocked = true;
                Both_Reroll.transform.parent.DOScale(0, 0.33f).SetEase(Ease.OutSine);
                P1_Reroll.blocked = false;
                P1_Reroll.transform.parent.DOScale(1, 0.33f).SetEase(Ease.OutSine);
                P2_Reroll.blocked = false;
                P2_Reroll.transform.parent.DOScale(1, 0.33f).SetEase(Ease.OutSine);
            }
        }
    }

}
