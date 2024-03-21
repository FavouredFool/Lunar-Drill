using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class LaserChargeRotate : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] private VisualEffect _rotateMe;
    [SerializeField] private List<float> _availableRoations;

    [SerializeField] private float _rotateRateInRotationsPerSecond;

    //--- Private Fields ------------------------

    private int _currentRotationIdx = 0;
    private float _timeSinceLastSwitch = 0;

    //--- Unity Methods ------------------------

    private void Update()
    {
        // Rotate charge star
        if (_rotateMe.GetBool("Alive"))
        {
            _timeSinceLastSwitch += Time.deltaTime;
            if (_timeSinceLastSwitch >= 1 / _rotateRateInRotationsPerSecond)
            {
                if (_currentRotationIdx == _availableRoations.Count() - 1)
                {
                    _currentRotationIdx = 0;
                }
                else
                {
                    _currentRotationIdx++;
                }

                _rotateMe.SetFloat("Rotation", _availableRoations[_currentRotationIdx]);

                _timeSinceLastSwitch = 0;
            }
        }
    }

    //--- Public Methods ------------------------

    //--- Private Methods ------------------------
}
