using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpiderWaitState : SpiderState
{
    public SpiderWaitState(SpiderManager spiderManager, Stack<SpiderState> stateStack = null, float waitTime = float.NaN) : base(spiderManager)
    {
        _stateStack = stateStack;
        
        if (float.IsNaN(waitTime))
        {
            _duration = Random.Range(1.5f, 3f);
        }
        else
        {
            _duration = waitTime;
        }
    }

    float _duration;
    float _startTime;
    Stack<SpiderState> _stateStack;
    
    public override void StartState()
    {
        Debug.Log("SpiderWaitState");
        
        // Place in scriptable object?
        
        _startTime = Time.time;
    }
    
    public override void UpdateState()
    {
        if (Time.time - _startTime >= _duration)
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
