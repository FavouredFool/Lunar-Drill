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
        
        _spiderManager.SpiderController.SetMovementGoalRotation(45, 90);
        yield return _spiderManager.SpiderController.WaitUntilArrivedAtGoalRotation();

        _spiderManager.SpiderController.SpiderLaser.StopLaser();
        yield return new WaitForSeconds(Random.Range(1f, 2.5f));

        _spiderManager.SpiderStateManager.SetState(new SpiderDecisionState(_spiderManager, _stateStack));
    }
    
    public override void FixedUpdateState()
    {
        _spiderManager.SpiderController.CalculateOrbitRotation();
        _spiderManager.SpiderController.SetSpiderPosition();
        _spiderManager.SpiderController.SetSpiderRotation();
    }
}
