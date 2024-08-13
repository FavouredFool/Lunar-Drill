using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpiderDamagedState : SpiderState
{
    Tween _regenerateVulnerableTween;
    
    public SpiderDamagedState(SpiderManager spiderManager) : base (spiderManager) { }
    
    public override void StartState()
    {
        Debug.Log("SpiderDamagedState");

        GetDamaged();
        
        _spiderManager.SpiderStateManager.SetState(new SpiderDecisionState(_spiderManager));
    }
    
    void GetDamaged()
    {
        // Camera shake
        CamShake.Instance.ShakeCamera();

        // Sound
        AudioController.Fire(new SpiderHit(""));

        //Damage
        _spiderManager.SpiderController.SpriteIterator.Hit();
        _gameManager.Hit(_spiderManager.SpiderController.gameObject, false);

        _spiderManager.SpiderController.IsVulnerable = false;
        _spiderManager.SpiderController.IsInvincible = true;
        _spiderManager.SpiderController.OverheatT = 0;
        _spiderManager.SpiderController.SpawnHP();
        
        DOVirtual.DelayedCall(4, () => _spiderManager.SpiderController.IsInvincible = false, false);
        
        foreach (SpriteRenderer spriteRenderer in _spiderManager.SpiderController.SpriteRenderers)
        {
            spriteRenderer.DOColor(Color.clear, _spiderManager.SpiderController.InvincibleTime).SetEase(Ease.Flash, 48, 0.75f);
        }

        Rumble.instance?.RumbleBoth(4, 1f, 0.33f);
    }
}
