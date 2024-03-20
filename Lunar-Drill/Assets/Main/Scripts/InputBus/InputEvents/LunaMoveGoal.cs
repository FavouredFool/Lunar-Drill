using UnityEngine.InputSystem;

public class LunaMoveGoal : IInputSignal
{
    public InputAction.CallbackContext context;

    public LunaMoveGoal(InputAction.CallbackContext context)
    {
        this.context = context;
    }
}
