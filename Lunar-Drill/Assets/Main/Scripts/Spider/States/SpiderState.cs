using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpiderState
{
    protected readonly SpiderManager _spiderManager;
    protected readonly GameManager _gameManager;
    
    protected SpiderState(SpiderManager spiderManager)
    {
        _spiderManager = spiderManager;
        _gameManager = spiderManager.GameManager;
    }

    public virtual void StartState() { }
    public virtual void EndState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}
