using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerMenuController : MonoBehaviour
{
    public static PlayerMenuController instance;

    private void Awake()
    {
        instance = this;
    }

    #region InputEvents 

    public void OnTest()
    {
        Debug.Log("Test");
    }
    public void OnInputPause(InputAction.CallbackContext context) => InputBus.Fire(new Pause(context));

    public void OnInputNorth(InputAction.CallbackContext context) => InputBus.Fire(new InputNorth(ChosenCharacter.both, context, OptionsMenu.isOpen));
    public void OnInputEast(InputAction.CallbackContext context) => InputBus.Fire(new InputEast(ChosenCharacter.both, context, OptionsMenu.isOpen));
    public void OnInputSouth(InputAction.CallbackContext context) => InputBus.Fire(new InputSouth(ChosenCharacter.both, context, OptionsMenu.isOpen));
    public void OnInputWest(InputAction.CallbackContext context) => InputBus.Fire(new InputWest(ChosenCharacter.both, context, OptionsMenu.isOpen));

    #endregion
}
