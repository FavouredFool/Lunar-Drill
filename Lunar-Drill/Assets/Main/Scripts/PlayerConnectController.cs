using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class PlayerConnectController : MonoBehaviour
{
    // Enum to save which character is chosen by this Player connect Controller.
    public enum ChosenCharacter
    {
        drillian,
        luna,
        singleplayer
    }


    private bool _ready = false; // Bool to save whether or not the player is ready to play!

    public UnityEvent<bool> ReadyStateChanged = new(); // Event to Signal that the Ready State has changed.
    public UnityEvent<ChosenCharacter> ChosenCharacterChanged = new(); // Event to Signal that the Ready State has changed.

    public ChosenCharacter Character { get => _character; }
    public bool Ready
    {
        get => _ready;
        set
        {
            _ready = value;
            ReadyStateChanged.Invoke(value);
        }
    }


    //--- Private Fields ------------------------
    PlayerInput _input; // Input. Needed to assign the User to the Character
    
    ChosenCharacter _character = ChosenCharacter.singleplayer; // The Character Chosen by the User attributed to this Gameobject.


    // --- Public Functions. 
    public void Start()
    {
        Debug.Log("connected");
        DontDestroyOnLoad(gameObject);
        _input = GetComponent<PlayerInput>();
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    // When the Scene is loaded then we assign the player input to the character
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>Destroy(gameObject);
        SwitchActionMapToCharacter();
    }

    public void OnSwap(InputAction.CallbackContext context) // Allow the player(s) to swap their characters. Does nothing when ready.
    {
        if ((!context.started) || Ready)
            return;

        if(_character == ChosenCharacter.drillian)
        {
            ChosenCharacterChanged.Invoke(ChosenCharacter.luna);
            _character = ChosenCharacter.luna;
            
        }
        else if(_character == ChosenCharacter.luna)
        {
            ChosenCharacterChanged.Invoke(ChosenCharacter.drillian);
            _character = ChosenCharacter.drillian;
        }
    }

    public void OnReady(InputAction.CallbackContext context) // Allow the player(s) to get ready. (And rewerse the readyness)
    {
        if (!context.started)
            return;
        Ready = !Ready;
    }


    // Luna Input Events
    
    public void OnMoveGoal(InputAction.CallbackContext context)
    {
        Debug.Log("luna");
        InputBus.Fire(new LunaMoveGoal(context));
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        InputBus.Fire(new LunaShoot(context));
        Debug.Log("luna");

    }



    // Drillian Input Events
    public void OnMoveDirection(InputAction.CallbackContext context)
    {
        Debug.Log("drillian");
        InputBus.Fire(new DrillianMoveDirection(context));
    }



    //Method to activate the correct action map
    public void SwitchActionMapToCharacter()
    {
        
        GameObject characterGO;
        if (_character == ChosenCharacter.luna)
        {

            // If the player is Luna then search the gameobject, disable its input and change this objects input map accordingly
            characterGO = FindAnyObjectByType<LunaController>().gameObject;
            if(characterGO != null)
            {
                PlayerInput goPlayerInput = characterGO.GetComponent<PlayerInput>();
                if (goPlayerInput != null)
                {
                    goPlayerInput.enabled = false;
                    Debug.Log("Disabled INput");
                }
                _input.SwitchCurrentActionMap("Luna");
            }

        }
        else if(_character == ChosenCharacter.drillian)
        {   
            // If the player is Drillian then search the gameobject, disable its input and change this objects input map accordingly
            characterGO = FindAnyObjectByType<DrillianController>().gameObject;
            if(characterGO != null)
            {
                PlayerInput goPlayerInput = characterGO.GetComponent<PlayerInput>();
                if (goPlayerInput != null)
                {
                    goPlayerInput.enabled = false;
                    Debug.Log("Disabled INput");

                }
            }
        }
        else
        {
            // If the player is neither Luna nor Drillian then switch to Singleplayermode.
            
            _input.SwitchCurrentActionMap("Singleplayer");
            // and disable the inputs of the objects accordingly
            characterGO = FindAnyObjectByType<DrillianController>().gameObject;
            bool foundDrillian = false;
            if (characterGO != null)
            {
                foundDrillian = true;
                PlayerInput goPlayerInput = characterGO.GetComponent<PlayerInput>();
                if (goPlayerInput != null)
                {
                    goPlayerInput.enabled = false;
                    
                }
                
                
            }
            else
            {
                _input.SwitchCurrentActionMap("Connect");
            }
            characterGO = FindAnyObjectByType<LunaController>().gameObject;
            if (characterGO != null && foundDrillian)
            {
                PlayerInput goPlayerInput = characterGO.GetComponent<PlayerInput>();
                if (goPlayerInput != null)
                {
                    goPlayerInput.enabled = false;
                }
                else
                {
                    _input.SwitchCurrentActionMap("Connect");
                }
            }
        }
        
    }

    // Method to switch to "coop mode"
    public void CoopReady()
    {
        ChosenCharacterChanged.Invoke(ChosenCharacter.luna);
        _character = ChosenCharacter.luna;
    }

    

}
