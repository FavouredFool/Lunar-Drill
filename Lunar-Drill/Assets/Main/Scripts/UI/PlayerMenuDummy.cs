using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMenuDummy : MonoBehaviour, IInputSubscriber<DrillianMoveDirection>, IInputSubscriber<LunaMoveGoal>
{
    RectTransform rect;
    public ChosenCharacter character;
    public float range;
    public float speed;

    Vector2 targetPosition;
    Vector2 originalPosition;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalPosition = rect.anchoredPosition;
        targetPosition = originalPosition;

        InputBus.Subscribe<DrillianMoveDirection>(this);
        InputBus.Subscribe<LunaMoveGoal>(this);
    }
    private void Update()
    {
        Vector2 pos = rect.anchoredPosition;
        pos = Vector2.Lerp(pos, targetPosition, speed * Time.unscaledDeltaTime);
        rect.anchoredPosition = pos;
    }

    public void UpdateTargetPosition(InputAction.CallbackContext c)
    {

        targetPosition = originalPosition;

        if (c.phase == InputActionPhase.Canceled)
            return;

        targetPosition += c.ReadValue<Vector2>()*range;
    }

    public void OnEventHappened(DrillianMoveDirection e)
    {
        if (character == ChosenCharacter.luna) return;

        UpdateTargetPosition(e.context);

    }
    public void OnEventHappened(LunaMoveGoal e)
    {
        if (character == ChosenCharacter.drillian) return;

        UpdateTargetPosition(e.context);
    }
}
