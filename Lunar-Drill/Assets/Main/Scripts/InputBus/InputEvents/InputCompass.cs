using UnityEngine.InputSystem;

public class InputNorth : IInputSignal
{
    public bool inOverlayMenu;
    public ChosenCharacter character;
    public InputAction.CallbackContext context;

    public InputNorth(ChosenCharacter character, InputAction.CallbackContext context, bool inOverlayMenu=false)
    {
        this.inOverlayMenu = inOverlayMenu;
        this.character = character;
        this.context = context;
    }
}
public class InputEast : IInputSignal
{
    public bool inOverlayMenu;
    public ChosenCharacter character;
    public InputAction.CallbackContext context;

    public InputEast(ChosenCharacter character, InputAction.CallbackContext context, bool inOverlayMenu = false)
    {
        this.inOverlayMenu = inOverlayMenu;
        this.character = character;
        this.context = context;
    }
}
public class InputSouth : IInputSignal
{
    public bool inOverlayMenu;
    public ChosenCharacter character;
    public InputAction.CallbackContext context;

    public InputSouth(ChosenCharacter character, InputAction.CallbackContext context, bool inOverlayMenu = false)
    {
        this.inOverlayMenu = inOverlayMenu;
        this.character = character;
        this.context = context;
    }
}
public class InputWest : IInputSignal
{
    public bool inOverlayMenu;
    public ChosenCharacter character;
    public InputAction.CallbackContext context;

    public InputWest(ChosenCharacter character, InputAction.CallbackContext context, bool inOverlayMenu = false)
    {
        this.inOverlayMenu = inOverlayMenu;
        this.character = character;
        this.context = context;
    }
}
