using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionQuitButton : MonoBehaviour, IInputSubscriber<Signal_SceneChange>
{
    [SerializeField]TMP_Text _header, _sub;

    private void OnEnable()
    {
        InputBus.Subscribe(this);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe(this);
    }
    public void OnEventHappened(Signal_SceneChange e)
    {
        string ht, st;
        switch (e.scene)
        {
            case SceneIdentity.PlayerConnect:
            case SceneIdentity.PlayerSelect:
            case SceneIdentity.GameTutorial:
                ht = "Return";
                st = "Quit to main menu";
                break;
            case SceneIdentity.GameMain:
                ht = "Abandon";
                st = "Quit to main menu";
                break;
            case SceneIdentity.Stats:
                ht = "Return";
                st = "Back to main menu";
                break;
            case SceneIdentity.MainMenu:
            default:
                ht = "Quit";
                st = "Close the game";
                break;
        }

        _header.text = ht;
        _sub.text = st;
    }
}
