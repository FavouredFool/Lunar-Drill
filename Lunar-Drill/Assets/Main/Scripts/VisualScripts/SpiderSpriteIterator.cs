using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;
using UnityEngine.VFX;

public class SpiderSpriteIterator : MonoBehaviour
{
    [SerializeField] SpiderController controller;

    [SerializeField] SpiderLaser laserController;

    [SerializeField] Animator animator;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform crest;

    [SerializeField] float fps = 6;
    [SerializeField] float rotTime = 2;

    // VFX
    bool _vfxActive = false;
    [SerializeField] VisualEffect _energyLoss;
    [SerializeField] Texture2D _energyLossPink, _energyLossRed;

    float timer = 0;

    int lastMoveSign = 1;

    bool isLaser => laserController.IsActive;

    float fraction => 1f / fps;
    float angleStep => rotTime / fraction;

    Tween scaleTween;
    float initialScale;

    [SerializeField] Disc[] barDiscs;
    bool energyBarVisible => (isShield || controller.IsShieldCritical) && !controller.IsVulnerable && !controller.IsInvincible;
    float energyBarAlpha;

    private void Awake()
    {
        initialScale = spriteRenderer.transform.localScale.x;
    }
    private void Update()
    {
        lastMoveSign = controller.MoveSign;
        animator.SetInteger("moveDirection", lastMoveSign);

        crest.gameObject.SetActive(!isControlled);

        if (!isControlled)
        {
            timer += Time.deltaTime;

            spriteRenderer.sprite = _idleSprite;

            if (timer >= fraction && lastMoveSign != 0 && !isLaser)
            {
                timer = 0;

                crest.Rotate(Vector3.forward, angleStep * lastMoveSign);
            }
        }

        if (isLaser)
            crest.rotation = Quaternion.LookRotation(Vector3.forward, transform.position.normalized);

        // Shield vfx

        if (isShield)
        {
            if (controller.OverheatT <= 0.8f)
            {
                _energyLoss.SetTexture("Main Texture", _energyLossPink);
            }
            else
            {
                _energyLoss.SetTexture("Main Texture", _energyLossRed);
            }
        }

        if (isShield && !_vfxActive)
        {
            _energyLoss.SendEvent("Discharge");
            _energyLoss.SetBool("Alive", true);
            _vfxActive = !_vfxActive;
        }
        else if (!isShield && _vfxActive)
        {
            _energyLoss.Stop();
            _energyLoss.SetBool("Alive", false);
            _vfxActive = !_vfxActive;
        }

        energyBarAlpha += (energyBarVisible ? 2 : -2) * Time.deltaTime;
        energyBarAlpha = Mathf.Clamp01(energyBarAlpha);
        UpdateEnergyBar();
    }
    void UpdateEnergyBar()
    {
        foreach (Disc d in barDiscs)
        {
            Color c = d.Color;
            c.a = energyBarAlpha;
            d.Color = c;
        }
    }

    bool isControlled => Time.time < controlTime;
    float controlTime = 0;
    public void Control(Sprite sprite, float time)
    {
        controlTime = Time.time + time;
        timer = fraction;

        spriteRenderer.sprite = sprite;
    }

    [SerializeField] Sprite _hitSprite, _stunSprite, _idleSprite;
    public void Hit()
    {
        Control(_hitSprite, controller.InvinvibilityTime);

        animator.SetBool("stunned", false);

        scaleTween.Kill();
        scaleTween = spriteRenderer.transform.DOScale(initialScale * 1.33f, TimeManager.hitFrameTime * 0.33f).SetUpdate(true).SetEase(Ease.OutBack).OnComplete(() =>
          {
              scaleTween = spriteRenderer.transform.DOScale(initialScale, 0.1f).SetUpdate(true).SetEase(Ease.OutSine);
          });

        energyBarAlpha = 0;
        UpdateEnergyBar();
    }

    public void CancelStun()
    {
        controlTime = 0;
        animator.SetBool("stunned", false);
    }

    public void Stun(float time)
    {
        Control(_stunSprite, time);

        animator.SetBool("stunned", true);

        //scaleTween.Kill();
        //scaleTween = spriteRenderer.transform.DOScale(initialScale * 1.33f, time).SetEase(Ease.OutBack);
        //scaleTween.OnComplete(() => spriteRenderer.transform.localScale = initialScale * Vector3.one);
        //DOVirtual.DelayedCall(pulseTime * loops + 0.05f, () => spriteRenderer.transform.localScale = initialScale * Vector3.one);
    }

    bool isShield => Time.time < shieldTime;
    float shieldTime = 0;
    public void ShieldHit(float time)
    {
        shieldTime = Time.time + time;
    }
}
