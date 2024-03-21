using UnityEngine.InputSystem;

public class LunaShoot : IInputSignal
{
    public InputAction.CallbackContext context;

    public LunaShoot(InputAction.CallbackContext context)
    {
        this.context = context;
    }
}
