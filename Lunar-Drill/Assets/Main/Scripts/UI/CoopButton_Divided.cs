using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CoopButton_Divided : CoopButton
{
    [Header("Divided Events")]
    public Button.ButtonClickedEvent _OnNorthPerformedEvents;
    public Button.ButtonClickedEvent _OnEastPerformedEvents;
    public Button.ButtonClickedEvent _OnSouthPerformedEvents;
    public Button.ButtonClickedEvent _OnWestPerformedEvents;


    InputType _lastInput;

    public override bool ProcessInput(ChosenCharacter character, InputType inp, InputActionPhase phase)
    {
        if (!base.ProcessInput(character, inp, phase)) return false;

        _lastInput = inp;

        return true;
    }
    public override void TriggerEvents()
    {
        if (_lastInput == InputType.None) return;
        if (_lastInput.HasFlag(InputType.North))
            _OnNorthPerformedEvents.Invoke();
        else if (_lastInput.HasFlag(InputType.East))
            _OnEastPerformedEvents.Invoke();
        else if (_lastInput.HasFlag(InputType.South))
            _OnSouthPerformedEvents.Invoke();
        else if (_lastInput.HasFlag(InputType.West))
            _OnWestPerformedEvents.Invoke();

        base.TriggerEvents();
    }
}
