using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SpiderMovementState : SpiderState
{
    public SpiderMovementState(SpiderManager spiderManager, Stack<SpiderState> stateStack = null, float innerAngle = 80, float outerAngle = 170) :
        base(spiderManager)
    {
        _innerAngle = innerAngle;
        _outerAngle = outerAngle;
        _stateStack = stateStack;
    }

    float _innerAngle;
    float _outerAngle;
    Stack<SpiderState> _stateStack;
    
    public override void StartState()
    {
        Debug.Log("SpiderMovementState");
        _spiderManager.SpiderController.SetMovementGoalRotation(_innerAngle, _outerAngle);
    }
    
    public override void UpdateState()
    {
        if (_spiderManager.SpiderController.ArrivedAtGoalRotation())
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderDecisionState(_spiderManager, _stateStack));
        }
    }

    public override void FixedUpdateState()
    {
        _spiderManager.SpiderController.CalculateOrbitRotation();
        _spiderManager.SpiderController.SetSpiderPosition();
        _spiderManager.SpiderController.SetSpiderRotation();
    }
}
