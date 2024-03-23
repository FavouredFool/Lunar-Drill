using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] Camera _cam;

    [SerializeField] float _shakeDuration;
    [SerializeField] Vector3 _shakeStrength;
    [SerializeField] int _shakeVibrato;
    [SerializeField] [Range(0, 90)] float _shakeRandomness;
    [SerializeField] bool _shakeFadeOut = true;

    //--- Private Fields ------------------------

    private static CamShake _instance;
    public static CamShake Instance { get => _instance; }

    //--- Unity Methods ------------------------

    private void Awake()
    {
        // Singelton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    //--- Public Methods ------------------------

    public void ShakeCamera()
    {
        _cam.DOShakePosition(_shakeDuration,_shakeStrength,_shakeVibrato,_shakeRandomness,_shakeFadeOut,ShakeRandomnessMode.Harmonic).SetUpdate(true);
    }

    //--- Private Methods ------------------------
}
