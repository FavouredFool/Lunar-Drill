using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpiderVulnerableState : SpiderState
{
    Tween _regenerateVulnerableTween;
    
    public SpiderVulnerableState(SpiderManager spiderManager) : base (spiderManager) { }
    
    public override void StartState()
    {
        Debug.Log("SpiderVulnerableState");

        _spiderManager.SpiderController.IsVulnerable = true;

        _spiderManager.SpiderController.SpriteIterator.Stun(float.MaxValue); //Change this to overheat time

        _spiderManager.SpiderController.RegenerateT = 0;
        _regenerateVulnerableTween = DOTween.To(() => _spiderManager.SpiderController.RegenerateT, e => _spiderManager.SpiderController.RegenerateT = e, 1, _spiderManager.SpiderController.RegenerateTime);
        _regenerateVulnerableTween.OnComplete(() =>
        {
            _spiderManager.SpiderController.OverheatT = 0;
            _spiderManager.SpiderController.IsVulnerable = false;
            _spiderManager.SpiderController.SpriteIterator.CancelStun();
        });
        
        // Destroy all Mines
        _gameManager.MineSpawner.DestroyAllMines();
    }

    public override void EndState()
    {
        if (_regenerateVulnerableTween != null && _regenerateVulnerableTween.IsActive())
        {
            _regenerateVulnerableTween.Kill();
        }
    }
    
    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderDecisionState(_spiderManager));
        }
    }
}
