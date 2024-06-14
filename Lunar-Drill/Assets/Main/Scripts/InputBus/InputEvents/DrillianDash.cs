using UnityEngine.InputSystem;

public class DrillianDash : IInputSignal
{
    public InputAction.CallbackContext context;

    public DrillianDash(InputAction.CallbackContext context)
    {
        this.context = context;
    }
}
