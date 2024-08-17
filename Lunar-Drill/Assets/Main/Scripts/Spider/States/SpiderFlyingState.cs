using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderFlyingState : SpiderState
{
    public SpiderFlyingState(SpiderManager spiderManager) : base (spiderManager) { }
    
    float _pufferTime = 0.1f;
    float _endWait = 0.75f;
    float _startTime = float.PositiveInfinity;
    bool _stop = false;
    
    public override void StartState()
    {
        Debug.Log("SpiderFlyingState");
        _startTime = Time.time;
    }

    public IEnumerator FlyEnd()
    {
        yield return new WaitForSeconds(_endWait);
        _spiderManager.SpiderStateManager.SetState(new SpiderDecisionState(_spiderManager));
    }
    
    public override void FixedUpdateState()
    {
        if (_stop) return;
        
        _spiderManager.SpiderController.ApplyGravityToVelocity();
        _spiderManager.SpiderController.Rigidbody.MoveRotation(Vector2.SignedAngle(Vector2.up, -_spiderManager.SpiderController.Rigidbody.velocity.normalized));
            
        if (_spiderManager.SpiderController.transform.position.magnitude <= _spiderManager.SpiderController.SpiderBodyOrbit && Time.time - _startTime > _pufferTime)
        {
            _spiderManager.SpiderController.EndFly();
            _spiderManager.SpiderController.SpriteIterator.ToggleDrill(false);
            _stop = true;
            _spiderManager.SpiderController.IsDrillingFlying = false;
            
            _spiderManager.SpiderController.StartCoroutine(FlyEnd());
        } 
    }
}
