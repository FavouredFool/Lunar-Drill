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
    public static bool isSolo = true;
    public static bool Enabled=true;
    public PlayerInput Input => GetComponent<PlayerInput>();
    public ChosenCharacter Character = ChosenCharacter.both;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void SetCharacter(ChosenCharacter c)
    {
        Character = c;
        Input.SwitchCurrentActionMap(GetActionMapName());
        Rumble.instance?.SetGamepad(Input, c);
    }
    public void SetMenuMode(bool on)
    {
        if (!Input.isActiveAndEnabled)
        {
            Debug.LogWarning("Tried to switch to Menu Input map while playerinput component is disabled.");
            return;
        }

        if (on) Input.SwitchCurrentActionMap("Menus");
        else
        {
            string map = GetActionMapName();
            Input.SwitchCurrentActionMap(map);
        }

        Debug.Log("SWITCHED TO ACTION MAP: " + Input.currentActionMap);
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

    public static bool Swap()
    {
        if (isSolo || !Drillian || !Luna) return false;

        PlayerConnectController Drill = Drillian;
        Drillian = Luna;
        Luna = Drill;

        Drillian.SetCharacter(ChosenCharacter.drillian);
        Luna.SetCharacter(ChosenCharacter.luna);

        return true;
    }
    public static void Delete()
    {
        bool bothSame = Luna && Drillian && Luna.gameObject == Drillian.gameObject;
        if (Luna)
            Destroy(Luna.gameObject);
        if (!bothSame && Drillian)
            Destroy(Drillian.gameObject);
    }
    public static void Disable()
    {
        Enabled = false;
    }
    public static void Enable()
    {
        Enabled = true;
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
    public void OnInputPause(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new Pause(context)); }

    public void OnMoveLuna(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new LunaMoveGoal(context)); }
    public void OnMoveDrillian(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new DrillianMoveDirection(context)); }

    public void OnActiveLuna(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new LunaShoot(context)); }
    public void OnActiveDrillian(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new DrillianDash(context)); }

    public void OnBothInputNorth(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputNorth(ChosenCharacter.both, context)); }
    public void OnBothInputEast(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputEast(ChosenCharacter.both, context)); }
    public void OnBothInputSouth(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputSouth(ChosenCharacter.both, context));}
    public void OnBothInputWest(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputWest(ChosenCharacter.both, context));}

    public void OnLunaInputNorth(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputNorth(ChosenCharacter.luna, context));}
    public void OnLunaInputEast(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputEast(ChosenCharacter.luna, context));}
    public void OnLunaInputSouth(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputSouth(ChosenCharacter.luna, context));}
    public void OnLunaInputWest(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputWest(ChosenCharacter.luna, context));}

    public void OnDrillianInputNorth(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputNorth(ChosenCharacter.drillian, context));}
    public void OnDrillianInputEast(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputEast(ChosenCharacter.drillian, context));}
    public void OnDrillianInputSouth(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputSouth(ChosenCharacter.drillian, context));}
    public void OnDrillianInputWest(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new InputWest(ChosenCharacter.drillian, context));}

    public void OnMenuMoveNorth(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new MenuMoveNorth(context));}
    public void OnMenuMoveEast(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new MenuMoveEast(context));}
    public void OnMenuMoveSouth(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new MenuMoveSouth(context));}
    public void OnMenuMoveWest(InputAction.CallbackContext context) { if (Enabled) InputBus.Fire(new MenuMoveWest(context));}

    #endregion
}

public enum ChosenCharacter
{
    drillian,
    luna,
    both,
    any
}