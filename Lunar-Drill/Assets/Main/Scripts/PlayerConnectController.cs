using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

public class PlayerConnectController : MonoBehaviour
{
    // Enum to save which character is chosen by this Player connect Controller.
    enum ChosenCharacter
    {
        drillian,
        luna
    }


    //--- Private Fields ------------------------
    PlayerInput _input; // Input. Needed to assign the User to the Character
    
    [SerializeField] ChosenCharacter _character; // The Character Chosen by the User attributed to this Gameobject.


    public void Start()
    {
        Debug.Log("connected");
        DontDestroyOnLoad(gameObject);
        _input = GetComponent<PlayerInput>();
        _character = ChosenCharacter.drillian;
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    // When the Scene is loaded then we assign the player input to the character
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("test");
        SwitchActionMapToCharacter();
    }

    public void OnSwap()
    {
        if(_character == ChosenCharacter.drillian)
        {
            _character = ChosenCharacter.luna;
            
        }
        else
        {
            _character = ChosenCharacter.drillian;
        }
        Debug.Log("Swap");
        Debug.Log($"Player {_input.user} Swapped to: {_character}");
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
            characterGO = FindAnyObjectByType<LunaController>().gameObject;
            if(characterGO != null)
            {
                _input.SwitchCurrentActionMap("Luna");
            }

        }
        else
        {
            characterGO = FindAnyObjectByType<DrillianController>().gameObject;
            if(characterGO != null)
            {

                _input.SwitchCurrentActionMap("Drillian");
            }
        }

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

    

}
