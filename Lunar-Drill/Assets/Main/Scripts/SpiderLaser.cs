using DG.Tweening;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLaser : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Laser")]
    [SerializeField] Line _laserVisual;
    [SerializeField] BoxCollider2D _laserCollider;
    [SerializeField][Range(0.1f, 1f)] float _laserMaxThickness;
    [SerializeField][Range(0.1f, 1f)] float _laserMinThickness;
    [SerializeField][Range(0.01f, 1f)] float _thickeningSpeed;
    [SerializeField] Color _preLaserColor;
    [SerializeField] Color _laserColor;


    //--- Private Fields ------------------------




    //--- Public Methods ------------------------

    public IEnumerator ShootLaser()
    {
        // Step 1:
        // Show pre-laser
        // flicker pre-laser
        // quickly increase time
        // The collider must match accordingly

        _laserVisual.Color = _preLaserColor;
        _laserVisual.Thickness = _laserMinThickness;
        _laserVisual.enabled = true;
        
        yield return new WaitForSeconds(2);

        _laserVisual.Color = _laserColor;
        DOTween.To(() => _laserVisual.Thickness, x => _laserVisual.Thickness = x, _laserMaxThickness, _thickeningSpeed).SetEase(Ease.InSine);

        yield return new WaitForSeconds(3);

        _laserVisual.enabled = false;

        yield return new WaitForSeconds(2);

        yield return ShootLaser();
    }


}
