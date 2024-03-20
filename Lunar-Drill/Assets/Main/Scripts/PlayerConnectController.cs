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
        AssignPlayerToCharacter();
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

    public void OnMoveGoal(InputAction.CallbackContext context)
    {
        Debug.Log($"Luna {context.ToString()}");
    }
    public void OnMoveDirection(InputAction.CallbackContext context)
    {
        Debug.Log($"Drillean {context.ToString()}");
    }


    //Function to assign the Correct Input to the Character. Connect Controller 
    public void AssignPlayerToCharacter()
    {
        // Find correct gameobject for the character
        GameObject characterGO;
        if (_character == ChosenCharacter.drillian)
        {
            characterGO = FindAnyObjectByType<DrillianController>().gameObject;

        }
        else
        {
            characterGO = FindAnyObjectByType<LunaController>().gameObject;
        }
        // assign this objects input to the gamobject
        PlayerInput characterInput = characterGO.GetComponent<PlayerInput>();
        Debug.Log($"Assigning {_input.user.pairedDevices[0]} to {characterGO}");
        if(characterInput.user == null)
        {
            
        }
        else
        {
            characterInput.user.UnpairDevices();
            InputUser.PerformPairingWithDevice(_input.user.pairedDevices[0]);
        }
        
    }
    
}
