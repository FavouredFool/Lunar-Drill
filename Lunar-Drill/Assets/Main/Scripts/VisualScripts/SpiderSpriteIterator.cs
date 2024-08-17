using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;
using Unity.VisualScripting;
using UnityEngine.VFX;

public class SpiderSpriteIterator : MonoBehaviour
{
    [SerializeField] SpiderController controller;

    [SerializeField] SpiderLaser laserController;

    [SerializeField] Animator animator;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer crest,drill;
    [SerializeField] SpriteRenderer _leftLeg, _rightLeg;

    [SerializeField] float fps = 6;
    [SerializeField] float rotTime = 2;

    // VFX
    bool _vfxActive = false;
    [SerializeField] VisualEffect _energyLoss;
    [SerializeField] Texture2D _energyLossPink, _energyLossRed;

    float timer = 0;

    int lastMoveSign = 1;

    bool isLaser => laserController.IsActive;
    bool isDrill = false;

    float fraction => 1f / fps;
    float angleStep => rotTime / fraction;

    Tween scaleTween;
    float initialScale;

    [SerializeField] Disc[] barDiscs;
    bool energyBarVisible => (isShield || controller.IsShieldCritical) && !controller.IsVulnerable && !controller.IsInvincible;
    float energyBarAlpha;
    bool isControlled => Time.time < controlTime;
    float controlTime = 0;
    [SerializeField] Sprite _hitSprite, _stunSprite, _idleSprite;
    [SerializeField] Sprite _crestBase, _drillBase;
    [SerializeField] Sprite[] _drillSprites;
    [SerializeField]float _drillIterationSpeed = 2;
    int _drillIndex = 0;

    private void Awake()
    {
        initialScale = spriteRenderer.transform.localScale.x;
    }
    private void Update()
    {
        lastMoveSign = controller.MoveSign;
        animator.SetInteger("moveDirection", lastMoveSign);

        crest.gameObject.SetActive(!isControlled&&!isDrill);

        if (!isControlled&&!isDrill)
        {
            timer += Time.deltaTime;

            spriteRenderer.sprite = _idleSprite;

            if (timer >= fraction && lastMoveSign != 0 && !isLaser)
            {
                timer = 0;

                crest.transform.Rotate(Vector3.forward, angleStep * lastMoveSign);
            }
        }

        if (isLaser)
            crest.transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position.normalized);

        if (isDrill)
        {
            crest.transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.up);

            timer += Time.deltaTime*_drillIterationSpeed;
            if (timer >= fraction)
            {
                timer = 0;
                _drillIndex = (_drillIndex + 1) % _drillSprites.Length;
                drill.sprite = _drillSprites[_drillIndex];
            }
        }

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

        energyBarAlpha += (energyBarVisible && !isDrill ? 2 : -2) * Time.deltaTime;
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

    public void Control(Sprite sprite, float time)
    {
        controlTime = Time.time + time;
        timer = fraction;

        spriteRenderer.sprite = sprite;
    }


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

    public void ToggleDrill(bool on)
    {
        //IsControlled sollte dabei auch an sein.
        isDrill = on; //Das sollte probably mehr wie isLaser Funktionieren?
        drill.gameObject.SetActive(on);
        _leftLeg.gameObject.SetActive(!on);
        _rightLeg.gameObject.SetActive(!on);
        _drillIndex = 0;

        animator.SetBool("drill", on);
    }
}
