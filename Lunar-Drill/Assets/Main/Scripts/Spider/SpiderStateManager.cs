using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderStateManager
{
    public SpiderState State { get; private set; }
    
    public void SetState(SpiderState state)
    {
        if (State != null) 
            State.EndState();
        State = state;
        State.StartState();
    }
    
    public void UpdateState()
    {
        State.UpdateState();
    }

    public void FixedUpdateState()
    {
        State.FixedUpdateState();
    }
}
