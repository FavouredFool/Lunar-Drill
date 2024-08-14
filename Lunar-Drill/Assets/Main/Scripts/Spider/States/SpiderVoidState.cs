using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpiderVoidState : SpiderState
{
    public SpiderVoidState(SpiderManager spiderManager) : base (spiderManager) { }
    
    public override void StartState()
    {
        Debug.Log("SpiderVoidState");
    }
}
