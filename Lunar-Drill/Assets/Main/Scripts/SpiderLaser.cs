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
    [SerializeField] Line[] _laserVisuals;
    [SerializeField] BoxCollider2D _laserCollider;
    [SerializeField][Range(0.1f, 1f)] float _laserMaxThickness;
    [SerializeField][Range(0.01f, 1f)] float _laserMinThickness;
    [SerializeField][Range(0.01f, 1f)] float _thickeningSpeed;

    [Header("Timings")]
    [SerializeField][Range(0.1f, 10f)] float _preLaserDuration;
    [SerializeField][Range(0.1f, 10f)] float _laserDuration;

    [Header("VFX")]
    [SerializeField] VisualEffect _laserChargeOuter;
    [SerializeField] VisualEffect _laserChargeInner;

    public bool isActive { get; private set; }=false;

    //--- Private Fields ------------------------




    //--- Public Methods ------------------------

    public IEnumerator ShootLaser()
    {
        isActive = true;

        // VFX
        _laserChargeOuter.SetFloat("Charge Time", _preLaserDuration);
        _laserChargeOuter.SetBool("Alive", true);
        _laserChargeOuter.SendEvent("Charge");
        _laserChargeInner.SetFloat("Charge Time", _preLaserDuration);
        _laserChargeInner.SetBool("Alive", true);
        _laserChargeInner.SendEvent("Charge");

        _laserVisuals.ForEach(e => e.Thickness = _laserMinThickness);
        _laserVisuals.ForEach(e => e.enabled = true);

        foreach (Line laserVisual in _laserVisuals)
        {
            DOTween.To(() => laserVisual.Color, x => laserVisual.Color = x, Color.clear, _preLaserDuration).SetEase(Ease.InFlash, 12, 0);
        }
        
        yield return new WaitForSeconds(_preLaserDuration);

        Sequence thicknessTweens = DOTween.Sequence();

        Tween thicknessTween0 = DOTween.To(() => _laserVisuals[0].Thickness, x => _laserVisuals[0].Thickness = x, _laserMaxThickness, _thickeningSpeed).SetEase(Ease.InSine);
        Tween thicknessTween1 = DOTween.To(() => _laserVisuals[1].Thickness, x => _laserVisuals[1].Thickness = x, _laserMaxThickness, _thickeningSpeed).SetEase(Ease.InSine);
        thicknessTweens.Append(thicknessTween0);
        thicknessTweens.Join(thicknessTween1);

        thicknessTweens.OnComplete(() => _laserCollider.enabled = true);

        yield return new WaitForSeconds(_laserDuration);

        _laserVisuals.ForEach(e => e.enabled = false);
        _laserCollider.enabled = false;

        // VFX
        _laserChargeOuter.SetBool("Alive", false);
        _laserChargeOuter.Stop();
        _laserChargeInner.SetBool("Alive", false);
        _laserChargeInner.Stop();

        isActive = false;
    }


}
