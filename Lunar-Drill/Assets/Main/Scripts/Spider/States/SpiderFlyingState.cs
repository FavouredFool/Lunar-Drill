using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpiderFlyingState : SpiderState
{
    public SpiderFlyingState(SpiderManager spiderManager, Stack<SpiderState> stateStack) : base(spiderManager)
    {
        _stateStack = stateStack;
    }
    
    float _pufferTime = 0.1f;
    float _endWait = 0.25f;
    float _startTime = float.PositiveInfinity;
    bool _stop = false;
    Stack<SpiderState> _stateStack;
    
    public override void StartState()
    {
        Debug.Log("SpiderFlyingState");
        _startTime = Time.time;
    }

    public override void UpdateState()
    {
        if (_stop) return;
        
        if (_spiderManager.SpiderController.transform.position.magnitude <= _spiderManager.SpiderController.SpiderBodyOrbit && Time.time - _startTime > _pufferTime)
        {
            _spiderManager.SpiderController.EndFly();
            _spiderManager.SpiderController.SpriteIterator.ToggleDrill(false);
            _spiderManager.SpiderController.CollidersSetActive(true);
            _stop = true;
            _spiderManager.SpiderController.IsDrillingFlying = false;
            
            _spiderManager.SpiderStateManager.SetState(new SpiderWaitState(_spiderManager, _stateStack, 0.25f));
        } 
    }
    
    public override void FixedUpdateState()
    {
        if (_stop) return;
        
        _spiderManager.SpiderController.ApplyGravityToVelocity();
        _spiderManager.SpiderController.Rigidbody.MoveRotation(Vector2.SignedAngle(Vector2.up, -_spiderManager.SpiderController.Rigidbody.velocity.normalized));
    }
}
