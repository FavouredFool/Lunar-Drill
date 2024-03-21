using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Class that handles the general overarching logic of the character swap screen.
public class SelectScreenHandling : MonoBehaviour
{

    public UnityEvent<int> PlayerNumberChanged = new(); // Event that fires when the number of players changed
    public UnityEvent Starting = new(); // Event that fires when we are going to start the game
    public UnityEvent StoppingGameStart = new(); // Event that fires when the game Start is aborted.

    [SerializeField] private ConnectManager connectManager; // Script that Saves if players are Ready and how many.

    [SerializeField] string _nextScene; // the Scene to be loaded after the player(s) selected their characters

    private Coroutine _countdownCoroutine; // coroutine that starts the countdown. Saved so it can be aborted.



    // Function that adds a Listener to a PlayerConnectController
    // (with the Change Game Start function being executed)
    // this allows to start the game of all players are ready.

    public void PlayerConnected(PlayerConnectController pcc)
    {
        PlayerNumberChanged.Invoke(connectManager.NumberConnectedPlayers);
        pcc.ReadyStateChanged.AddListener(ChangeGameStart);
    }
    
    
    // Function to check whether the game is ready
    // For singleplayer or multiplayer if it is 
    // Start a Countdown that can be aborted.
    // Exectuted when the Ready state of the players changes
    private void ChangeGameStart(bool newState)
    {
        
        if (connectManager.SinglePlayerGameReady || connectManager.CoopGameReady)
        {
            Starting.Invoke();
            _countdownCoroutine = StartCoroutine(StartCountdown());
        } 
        else 
        {
            StoppingGameStart.Invoke();
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
            }
        }
    }


    // Function to start the Countdown to load another Scene
    IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(_nextScene);
    }
}
