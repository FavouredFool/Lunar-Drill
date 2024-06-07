using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerMenuController : MonoBehaviour
{
    public PlayerInput Input => GetComponent<PlayerInput>();

    #region InputEvents

    public void OnTest()
    {
        Debug.Log("Test");
    }
    public void OnInputPause(InputAction.CallbackContext context) => InputBus.Fire(new Pause(context));

    public void OnInputNorth(InputAction.CallbackContext context) => InputBus.Fire(new InputNorth(ChosenCharacter.both, context));
    public void OnInputEast(InputAction.CallbackContext context) => InputBus.Fire(new InputEast(ChosenCharacter.both, context));
    public void OnInputSouth(InputAction.CallbackContext context) => InputBus.Fire(new InputSouth(ChosenCharacter.both, context));
    public void OnInputWest(InputAction.CallbackContext context) => InputBus.Fire(new InputWest(ChosenCharacter.both, context));

    #endregion
}
