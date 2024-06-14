
using UnityEngine.InputSystem;


public class MenuMoveNorth : IInputSignal
{
    public InputAction.CallbackContext context;

    public MenuMoveNorth(InputAction.CallbackContext context)
    {
        this.context = context;
    }
}
public class MenuMoveEast : IInputSignal
{
    public InputAction.CallbackContext context;

    public MenuMoveEast(InputAction.CallbackContext context)
    {
        this.context = context;
    }
}
public class MenuMoveSouth : IInputSignal
{
    public InputAction.CallbackContext context;

    public MenuMoveSouth(InputAction.CallbackContext context)
    {
        this.context = context;
    }
}
public class MenuMoveWest : IInputSignal
{
    public InputAction.CallbackContext context;

    public MenuMoveWest(InputAction.CallbackContext context)
    {
        this.context = context;
    }
}