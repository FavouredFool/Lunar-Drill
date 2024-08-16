using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderDiggingState : SpiderState
{
    public SpiderDiggingState(SpiderManager spiderManager) : base (spiderManager) { }

    float _pufferTime = 0.1f;
    float _startTime = float.PositiveInfinity;
    bool _stop = true;

    float _initialWait = 0.5f;

    
    public override void StartState()
    {
        Debug.Log("SpiderDiggingState");
        _spiderManager.SpiderController.GoalRotation = -_spiderManager.SpiderController.GoalRotation;
        
        _spiderManager.SpiderController.SpriteIterator.ToggleDrill(true);
        _spiderManager.SpiderController.StartCoroutine(DrillStart());
    }

    public IEnumerator DrillStart()
    {
        yield return new WaitForSeconds(_initialWait);
        _startTime = Time.time;
        _stop = false;
    }
    
    public override void FixedUpdateState()
    {
        if (_stop) return;
        
        _spiderManager.SpiderController.UpdateDigRotation();
        
        _spiderManager.SpiderController.SetVelocity();
        _spiderManager.SpiderController.Rigidbody.MoveRotation(Vector2.SignedAngle(Vector2.up, -_spiderManager.SpiderController.Rigidbody.velocity.normalized));
            
        if (_spiderManager.SpiderController.transform.position.magnitude > _spiderManager.SpiderController.SpiderBodyOrbit && Time.time - _startTime > _pufferTime)
        {
            _spiderManager.SpiderController.EndDig();
            
            
            _spiderManager.SpiderStateManager.SetState(new SpiderFlyingState(_spiderManager));
        } 
    }
}
