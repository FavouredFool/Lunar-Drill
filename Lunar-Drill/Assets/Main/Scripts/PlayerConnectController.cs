using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class PlayerConnectController : MonoBehaviour
{


    private bool
        _readyDown = false,
        _swapDown = false;

    public bool Tiggle { get; set; } = false;

    public UnityEvent<bool> ReadyStateChanged = new();
    public UnityEvent<bool> SwapStateChanged = new();
    public UnityEvent<ChosenCharacter> ChosenCharacterChanged = new();

    public ChosenCharacter Character { get => _character; }
    public bool ReadyDown
    {
        get => _readyDown;
        set
        {
            _readyDown = value;
            ReadyStateChanged.Invoke(value);
        }
    }
    public bool SwapDown
    {
        get => _swapDown;
        set
        {
            _swapDown = value;
            SwapStateChanged.Invoke(value);
        }
    }


    //--- Private Fields ------------------------
    public PlayerInput _input { get; set; } // Input. Needed to assign the User to the Character
    
    [SerializeField] ChosenCharacter _character = ChosenCharacter.singleplayer; // The Character Chosen by the User attributed to this Gameobject.


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
        
        SceneManager.sceneLoaded += LateDestroy;
        SwitchActionMapToCharacter();
    }

    private void LateDestroy(Scene scene, LoadSceneMode mode)
    {
        Destroy(gameObject);
        SceneManager.sceneLoaded -= LateDestroy;
    }

    public void OnSwap(InputAction.CallbackContext context) // Allow the player(s) to swap their characters. Does nothing when ready.
    {
        if (context.performed)
            SwapDown = true;
        else if (context.canceled)
            SwapDown = false;

        Tiggle = context.performed;
    }

    public void OnReady(InputAction.CallbackContext context) // Allow the player(s) to get ready. (And rewerse the readyness)
    {
        if (context.performed)
            ReadyDown = true;
        else if (context.canceled)
            ReadyDown = false;

        Tiggle = context.performed;
    }
    public void SetCharacter(ChosenCharacter c)
    {
        ChosenCharacterChanged.Invoke(c);
        _character = c;
    }


    // Luna Input Events

    public void OnMoveGoal(InputAction.CallbackContext context)
    {
        InputBus.Fire(new LunaMoveGoal(context));
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        InputBus.Fire(new LunaShoot(context));
    }



    // Drillian Input Events
    public void OnMoveDirection(InputAction.CallbackContext context)
    {
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
                Rumble.main?.AddGamepad(_input,ChosenCharacter.luna);
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
                _input.SwitchCurrentActionMap("Drillian");
                Rumble.main?.AddGamepad(_input, ChosenCharacter.luna);
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
                Rumble.main?.AddGamepad(_input, ChosenCharacter.singleplayer);
            }
        }
        
    }
}

public enum ChosenCharacter
{
    drillian,
    luna,
    singleplayer
}