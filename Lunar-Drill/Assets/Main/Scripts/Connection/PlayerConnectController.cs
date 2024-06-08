using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(PlayerInput))]
public class PlayerConnectController : MonoBehaviour
{
    public static PlayerConnectController
        Drillian, Luna;
    public PlayerInput Input => GetComponent<PlayerInput>();
    public ChosenCharacter Character { get; private set; } = ChosenCharacter.both;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void SetCharacter(ChosenCharacter c)
    {
        Character = c;
        Input.SwitchCurrentActionMap(GetActionMapName());
        //Connect rumble at some point
    }
    public void SetMenuMode(bool on)
    {
        if (on) Input.SwitchCurrentActionMap("Menus");
        else Input.SwitchCurrentActionMap(GetActionMapName());
    }
    public string GetActionMapName()
    {
        string actionMap;
        switch (Character)
        {
            case ChosenCharacter.drillian:
                actionMap = "Drillian";
                Drillian = this;
                break;
            case ChosenCharacter.luna:
                actionMap = "Luna";
                Luna = this;
                break;
            default:
                actionMap = "Singleplayer";
                Drillian = this;
                Luna = this;
                break;
        }
        return actionMap;
    }
    private void OnDestroy()
    {
        Luna = null;
        Drillian = null;
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

    public void OnBothInputNorth(InputAction.CallbackContext context) => InputBus.Fire(new InputNorth(ChosenCharacter.both, context, OptionsMenu.isOpen));
    public void OnBothInputEast(InputAction.CallbackContext context) => InputBus.Fire(new InputEast(ChosenCharacter.both, context, OptionsMenu.isOpen));
    public void OnBothInputSouth(InputAction.CallbackContext context) => InputBus.Fire(new InputSouth(ChosenCharacter.both, context, OptionsMenu.isOpen));
    public void OnBothInputWest(InputAction.CallbackContext context) => InputBus.Fire(new InputWest(ChosenCharacter.both, context, OptionsMenu.isOpen));

    public void OnLunaInputNorth(InputAction.CallbackContext context) => InputBus.Fire(new InputNorth(ChosenCharacter.luna, context, OptionsMenu.isOpen));
    public void OnLunaInputEast(InputAction.CallbackContext context) => InputBus.Fire(new InputEast(ChosenCharacter.luna, context, OptionsMenu.isOpen));
    public void OnLunaInputSouth(InputAction.CallbackContext context) => InputBus.Fire(new InputSouth(ChosenCharacter.luna, context, OptionsMenu.isOpen));
    public void OnLunaInputWest(InputAction.CallbackContext context) => InputBus.Fire(new InputWest(ChosenCharacter.luna, context, OptionsMenu.isOpen));

    public void OnDrillianInputNorth(InputAction.CallbackContext context) => InputBus.Fire(new InputNorth(ChosenCharacter.drillian, context, OptionsMenu.isOpen));
    public void OnDrillianInputEast(InputAction.CallbackContext context) => InputBus.Fire(new InputEast(ChosenCharacter.drillian, context, OptionsMenu.isOpen));
    public void OnDrillianInputSouth(InputAction.CallbackContext context) => InputBus.Fire(new InputSouth(ChosenCharacter.drillian, context, OptionsMenu.isOpen));
    public void OnDrillianInputWest(InputAction.CallbackContext context) => InputBus.Fire(new InputWest(ChosenCharacter.drillian, context, OptionsMenu.isOpen));

    #endregion
}

public enum ChosenCharacter
{
    drillian,
    luna,
    both,
    any
}