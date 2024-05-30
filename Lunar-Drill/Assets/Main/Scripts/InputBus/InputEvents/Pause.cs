using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : IInputSignal
{
    public InputAction.CallbackContext context;

    public Pause(InputAction.CallbackContext context)
    {
        this.context = context;
    }
}
