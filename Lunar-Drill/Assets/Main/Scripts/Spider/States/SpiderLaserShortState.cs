using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLaserShortState : SpiderState
{
    public SpiderLaserShortState(SpiderManager spiderManager, Stack<SpiderState> stateStack = null) : base(
        spiderManager)
    {
        _stateStack = stateStack;
    }
    
    Stack<SpiderState> _stateStack;
    
    
    public override void StartState()
    {
        Debug.Log("SpiderLaserShortState");
        
        _spiderManager.SpiderController.StartCoroutine(_spiderManager.SpiderController.SpiderLaser.ShootLaser());
        _spiderManager.SpiderController.StartCoroutine(RandomLaserShortMovement());
    }
    
    public override void EndState()
    {
        _spiderManager.SpiderController.SpiderLaser.StopLaser();
    }
    
    public IEnumerator RandomLaserShortMovement()
    {
        _spiderManager.SpiderController.SetMovementGoalRotation(120, 179);
        yield return _spiderManager.SpiderController.WaitUntilArrivedAtGoalRotation();

        if (_spiderManager.SpiderController.IsVulnerable)
        {
            yield break;
        }
        
        _spiderManager.SpiderController.SetMovementGoalRotation(45, 90);
        yield return _spiderManager.SpiderController.WaitUntilArrivedAtGoalRotation();
        
        if (_spiderManager.SpiderController.IsVulnerable)
        {
            yield break;
        }

        _spiderManager.SpiderController.SpiderLaser.StopLaser();

        _spiderManager.SpiderStateManager.SetState(new SpiderWaitState(_spiderManager, _stateStack, 1f));
    }
    
    public override void FixedUpdateState()
    {
        _spiderManager.SpiderController.CalculateOrbitRotation();
        _spiderManager.SpiderController.SetSpiderPosition();
        _spiderManager.SpiderController.SetSpiderRotation();
    }
}
