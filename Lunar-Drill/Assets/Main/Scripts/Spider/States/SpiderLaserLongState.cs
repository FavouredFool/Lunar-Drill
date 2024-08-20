using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLaserLongState : SpiderState
{
    public SpiderLaserLongState(SpiderManager spiderManager, Stack<SpiderState> stateStack = null) : base(spiderManager)
    {
        _stateStack = stateStack;
    }
    
    Stack<SpiderState> _stateStack;
    
    public override void StartState()
    {
        Debug.Log("SpiderLaserLongState");
        
        _spiderManager.SpiderController.StartCoroutine(_spiderManager.SpiderController.SpiderLaser.ShootLaser());
        _spiderManager.SpiderController.StartCoroutine(RandomLaserLongMovement());
    }
    
    public IEnumerator RandomLaserLongMovement()
    {
        
        _spiderManager.SpiderController.SetMovementGoalRotation(90, 179);
        yield return _spiderManager.SpiderController.WaitUntilArrivedAtGoalRotation();
        
        if (_spiderManager.SpiderController.IsVulnerable)
        {
            yield break;
        }
        
        _spiderManager.SpiderController.SetMovementGoalRotation(35, 60);
        yield return _spiderManager.SpiderController.WaitUntilArrivedAtGoalRotation();

        if (_spiderManager.SpiderController.IsVulnerable)
        {
            yield break;
        }
        
        _spiderManager.SpiderController.SetMovementGoalRotation(75, 120);
        yield return _spiderManager.SpiderController.WaitUntilArrivedAtGoalRotation();
        
        if (_spiderManager.SpiderController.IsVulnerable)
        {
            yield break;
        }
        
        
        _spiderManager.SpiderController.SetMovementGoalRotation(55, 80);
        yield return _spiderManager.SpiderController.WaitUntilArrivedAtGoalRotation();
        
        if (_spiderManager.SpiderController.IsVulnerable)
        {
            yield break;
        }
        
        _spiderManager.SpiderController.SpiderLaser.StopLaser();

        _spiderManager.SpiderStateManager.SetState(new SpiderWaitState(_spiderManager, _stateStack, 1f));
    }

    public override void EndState()
    {
        _spiderManager.SpiderController.SpiderLaser.StopLaser();
    }
    
    public override void FixedUpdateState()
    {
        _spiderManager.SpiderController.CalculateOrbitRotation();
        _spiderManager.SpiderController.SetSpiderPosition();
        _spiderManager.SpiderController.SetSpiderRotation();
    }
}
