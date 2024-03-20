using UnityEngine.InputSystem;

public class DrillianMoveDirection : IInputSignal
{
    public InputAction.CallbackContext context;

    public DrillianMoveDirection(InputAction.CallbackContext context)
    {
        this.context = context;
    }
}
