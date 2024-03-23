using DG.Tweening;
using Shapes;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpiderLaser : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Laser")]
    [SerializeField] SpriteRenderer[] _laserSlimVisuals;
    [SerializeField] SpriteRenderer[] _laserVisuals;
    [SerializeField] BoxCollider2D _laserCollider;
    [SerializeField][Range(0.1f, 10f)] float _laserMaxThickness;
    [SerializeField][Range(0.01f, 10f)] float _laserMinThickness;
    [SerializeField][Range(0.01f, 10f)] float _thickeningTimeSeconds;

    [Header("Timings")]
    [SerializeField][Range(0.1f, 10f)] float _preLaserDuration;

    [Header("VFX")]
    [SerializeField] VisualEffect _laserChargeOuter;
    [SerializeField] VisualEffect _laserChargeInner;

    public bool IsActive { get; private set; }=false;

    bool _breakOut = false;

    //--- Private Fields ------------------------

    SpiderController _spider;

    //--- Unity Methods ------------------------

    public void Awake()
    {
        _spider = GetComponent<SpiderController>();
    }


    //--- Public Methods ------------------------

    public IEnumerator ShootLaser()
    {
        if (_spider.IsVulnerable) yield break;

        IsActive = true;
        _breakOut = false;

        //Rumble
        Rumble.main?.AddRumble(ChosenCharacter.luna, new Vector2(0.1f, 0.2f));
        Rumble.main?.AddRumble(ChosenCharacter.drillian, new Vector2(0.1f, 0.2f));

        // VFX
        _laserChargeOuter.SetFloat("Charge Time", _preLaserDuration);
        _laserChargeOuter.SetBool("Alive", true);
        _laserChargeOuter.SendEvent("Charge");
        _laserChargeInner.SetFloat("Charge Time", _preLaserDuration);
        _laserChargeInner.SetBool("Alive", true);
        _laserChargeInner.SendEvent("Charge");

        _laserVisuals.ForEach(e => e.enabled = false);
        _laserSlimVisuals.ForEach(e => e.enabled = true);

        foreach (SpriteRenderer spriteRenderer in _laserSlimVisuals)
        {
            DOTween.To(() => spriteRenderer.color, x => spriteRenderer.color = x, Color.clear, _preLaserDuration).SetEase(Ease.InFlash, 12, 0);
        }

        float waitStart = Time.time;

        while ((Time.time - waitStart < _preLaserDuration) || _breakOut)
        {
            // If vulnerable, break out earlier
            if (_spider.IsVulnerable)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        _laserSlimVisuals.ForEach(e => e.enabled = false);
        _laserVisuals.ForEach(e => e.enabled = true);
        _laserVisuals.ForEach(e => e.size = new Vector2(_laserMinThickness, e.size.y));

        Sequence thicknessTweens = DOTween.Sequence();

        Tween thicknessTween0 = DOTween.To(() => _laserVisuals[0].size, x => _laserVisuals[0].size = x, new Vector2(_laserMaxThickness, _laserVisuals[0].size.y), _thickeningTimeSeconds).SetEase(Ease.InCubic);
        Tween thicknessTween1 = DOTween.To(() => _laserVisuals[1].size, x => _laserVisuals[1].size = x, new Vector2(_laserMaxThickness, _laserVisuals[1].size.y), _thickeningTimeSeconds).SetEase(Ease.InCubic);
        thicknessTweens.Append(thicknessTween0);
        thicknessTweens.Join(thicknessTween1);

        AudioController.Fire(new SpiderLaserFiring(SpiderLaserFiring.LaserState.LaserFiring));

        bool canBreak = false;
        thicknessTweens.OnComplete(() => {

            _laserCollider.enabled = true;
            canBreak = true;
        });

        float waitStart2 = Time.time;

        //Rumble
        Rumble.main?.RemovePermanentRumble(ChosenCharacter.luna, new Vector2(0f, 0.1f));
        Rumble.main?.RemovePermanentRumble(ChosenCharacter.drillian, new Vector2(0f, 0.1f));

        Rumble.main?.AddRumble(ChosenCharacter.luna, new Vector2(0.1f, 0.3f));
        Rumble.main?.AddRumble(ChosenCharacter.drillian, new Vector2(0.1f, 0.3f));

        while (true)
        {
            // If vulnerable, break out earlier
            if ((_spider.IsVulnerable && canBreak) || _breakOut) break;
            yield return new WaitForEndOfFrame();
        }

        _laserVisuals.ForEach(e => e.enabled = false);
        _laserCollider.enabled = false;

        // VFX
        _laserChargeOuter.SetBool("Alive", false);
        _laserChargeOuter.Stop();
        _laserChargeInner.SetBool("Alive", false);
        _laserChargeInner.Stop();

        //Rumble
        Rumble.main?.RemovePermanentRumble(ChosenCharacter.luna, new Vector2(0.1f, 0.3f));
        Rumble.main?.RemovePermanentRumble(ChosenCharacter.drillian, new Vector2(0.1f, 0.3f));

        AudioController.Fire(new SpiderLaserFiring(SpiderLaserFiring.LaserState.LaserStopped));

        _breakOut = true;
        IsActive = false;
    }

    public void StopLaser()
    {
        _breakOut = true;
    }


}
