using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SpiderMovementState : SpiderState
{
    public SpiderMovementState(SpiderManager spiderManager) : base (spiderManager) { }
    
    public override void StartState()
    {
        Debug.Log("SpiderMovementState");
        _spiderManager.SpiderController.SetMovementGoalRotation(80, 170);
    }
    
    public override void UpdateState()
    {
        if (_spiderManager.SpiderController.ArrivedAtGoalRotation())
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderDecisionState(_spiderManager));
        }
    }

    public override void FixedUpdateState()
    {
        _spiderManager.SpiderController.CalculateOrbitRotation();
        _spiderManager.SpiderController.SetSpiderPosition();
        _spiderManager.SpiderController.SetSpiderRotation();
    }
}
