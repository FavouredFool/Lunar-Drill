using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpiderWaitState : SpiderState
{
    public SpiderWaitState(SpiderManager spiderManager) : base (spiderManager) { }

    float _duration;
    float _startTime;
    
    public override void StartState()
    {
        Debug.Log("SpiderWaitState");
        
        // Place in scriptable object?
        _duration = Random.Range(1.5f, 3.5f);
        _startTime = Time.time;
    }
    
    public override void UpdateState()
    {
        if (Time.time - _startTime >= _duration)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderDecisionState(_spiderManager));
        }
    }
}
