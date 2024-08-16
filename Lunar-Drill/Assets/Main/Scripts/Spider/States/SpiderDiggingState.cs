using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        
        _spiderManager.SpiderController.IsDrillingFlying = true;
        _spiderManager.SpiderController.SpriteIterator.ToggleDrill(true);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(_initialWait);
        
        // Dotween jump (not on the actual transform of the spider but on a child transform for the visual)
        Transform body = _spiderManager.SpiderController.BodyToTween;
        Sequence jumpSequence = DOTween.Sequence();
        jumpSequence.Append(body.DOLocalMoveY(0.5f, _initialWait * 0.6f).SetEase(Ease.OutCubic));
        jumpSequence.AppendInterval(_initialWait * 0.2f);
        jumpSequence.Append(body.DOLocalMoveY(0, _initialWait * 0.2f).SetEase(Ease.InCubic));

        sequence.Join(jumpSequence);
        
        sequence.OnComplete(() =>
        {
            _startTime = Time.time;
            _stop = false;
        });
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
