using DG.Tweening;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLaser : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Positioning")]
    [SerializeField][Range(0f, 5f)] float _laserStartOffset;

    [Header("Laser")]
    [SerializeField] Line _laserVisual;
    [SerializeField] BoxCollider2D _laserCollider;
    [SerializeField][Range(0.1f, 1f)] float _laserMaxThickness;
    [SerializeField][Range(0.01f, 1f)] float _laserMinThickness;
    [SerializeField][Range(0.01f, 1f)] float _thickeningSpeed;

    [Header("Timings")]
    [SerializeField][Range(0.1f, 10f)] float _preLaserDuration;
    [SerializeField][Range(0.1f, 10f)] float _laserDuration;



    //--- Private Fields ------------------------




    //--- Public Methods ------------------------

    public IEnumerator ShootLaser()
    {
        _laserVisual.Thickness = _laserMinThickness;
        _laserVisual.enabled = true;

        DOTween.To(() => _laserVisual.Color, x => _laserVisual.Color = x, Color.clear, _preLaserDuration).SetEase(Ease.InFlash, 52, 0);
        yield return new WaitForSeconds(_preLaserDuration);

        Tween thicknessTween = DOTween.To(() => _laserVisual.Thickness, x => _laserVisual.Thickness = x, _laserMaxThickness, _thickeningSpeed).SetEase(Ease.InSine);
        thicknessTween.OnComplete(() => _laserCollider.enabled = true);

        yield return new WaitForSeconds(_laserDuration);

        _laserVisual.enabled = false;
        _laserCollider.enabled = false;
    }


}
