using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderDiggingState : SpiderState
{
    public SpiderDiggingState(SpiderManager spiderManager) : base (spiderManager) { }

    public override void StartState()
    {
        Debug.Log("SpiderDiggingState");
    }
    
    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderDecisionState(_spiderManager));
        }
    }

    public override void FixedUpdateState()
    {
        _spiderManager.SpiderController.SetVelocity();

        //Debug.Log(_goalRotation);
        _spiderManager.SpiderController.Rigidbody.MoveRotation(Vector2.SignedAngle(Vector2.up, _spiderManager.SpiderController.Rigidbody.velocity.normalized));
            
        if (_spiderManager.SpiderController.transform.position.magnitude >= Utilities.InnerOrbit)
        {
            _spiderManager.SpiderController.EndDig();
        }
    }
}
