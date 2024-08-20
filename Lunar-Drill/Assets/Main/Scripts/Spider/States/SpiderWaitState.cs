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
    Stack<SpiderState> _stateStack;
    
    public override void StartState()
    {
        Debug.Log("SpiderWaitState");
        
        DOVirtual.DelayedCall(_duration, () =>
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderDecisionState(_spiderManager, _stateStack));
        }, false);
    }
    
    public override void FixedUpdateState()
    {
        _spiderManager.SpiderController.CalculateOrbitRotation();
        _spiderManager.SpiderController.SetSpiderPosition();
        _spiderManager.SpiderController.SetSpiderRotation();
    }
}
