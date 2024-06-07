using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(PlayerInput))]
public class PlayerConnectController : MonoBehaviour
{
    public PlayerInput Input => GetComponent<PlayerInput>();
    public ChosenCharacter Character { get; private set; } = ChosenCharacter.both;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void SetCharacter(ChosenCharacter c)
    {
        string actionMap;
        switch (c)
        {
            case ChosenCharacter.drillian:
                actionMap = "Drillian";
                break;
            case ChosenCharacter.luna:
                actionMap = "Luna";
                break;
            default:
                actionMap = "Singleplayer";
                break;
        }

        Character = c;
        Input.SwitchCurrentActionMap(actionMap);
        Debug.Log(Input.currentActionMap.name);
        //Connect rumble at some point
    }

    #region InputEvents

    public void OnTest()
    {
        Debug.Log("Test");
    }
    public void OnInputPause(InputAction.CallbackContext context) => InputBus.Fire(new Pause(context));

    public void OnMoveLuna(InputAction.CallbackContext context) => InputBus.Fire(new LunaMoveGoal(context));
    public void OnMoveDrillian(InputAction.CallbackContext context) => InputBus.Fire(new DrillianMoveDirection(context));

    public void OnActiveLuna(InputAction.CallbackContext context) => InputBus.Fire(new LunaShoot(context));
    public void OnActiveDrillian(InputAction.CallbackContext context) => InputBus.Fire(new DrillianDash(context));

    public void OnBothInputNorth(InputAction.CallbackContext context) => InputBus.Fire(new InputNorth(ChosenCharacter.both, context));
    public void OnBothInputEast(InputAction.CallbackContext context) => InputBus.Fire(new InputEast(ChosenCharacter.both, context));
    public void OnBothInputSouth(InputAction.CallbackContext context) => InputBus.Fire(new InputSouth(ChosenCharacter.both, context));
    public void OnBothInputWest(InputAction.CallbackContext context) => InputBus.Fire(new InputWest(ChosenCharacter.both, context));

    public void OnLunaInputNorth(InputAction.CallbackContext context) => InputBus.Fire(new InputNorth(ChosenCharacter.luna, context));
    public void OnLunaInputEast(InputAction.CallbackContext context) => InputBus.Fire(new InputEast(ChosenCharacter.luna, context));
    public void OnLunaInputSouth(InputAction.CallbackContext context) => InputBus.Fire(new InputSouth(ChosenCharacter.luna, context));
    public void OnLunaInputWest(InputAction.CallbackContext context) => InputBus.Fire(new InputWest(ChosenCharacter.luna, context));

    public void OnDrillianInputNorth(InputAction.CallbackContext context) => InputBus.Fire(new InputNorth(ChosenCharacter.drillian, context));
    public void OnDrillianInputEast(InputAction.CallbackContext context) => InputBus.Fire(new InputEast(ChosenCharacter.drillian, context));
    public void OnDrillianInputSouth(InputAction.CallbackContext context) => InputBus.Fire(new InputSouth(ChosenCharacter.drillian, context));
    public void OnDrillianInputWest(InputAction.CallbackContext context) => InputBus.Fire(new InputWest(ChosenCharacter.drillian, context));

    #endregion
}

public enum ChosenCharacter
{
    drillian,
    luna,
    both,
    any
}