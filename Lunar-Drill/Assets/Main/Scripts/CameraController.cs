using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField][Range(0.1f, 10f)] float _startZoom;
    [SerializeField][Range(0.1f, 10f)] float _endZoom;
    [SerializeField][Range(0.1f, 20f)] float _zoomTime;

    float _zoomT = 0;
    Camera _camera;


    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void Start()
    {
#if !UNITY_EDITOR
        DOTween.To(() => _zoomT, e => _zoomT = e, 1, _zoomTime).SetUpdate(true).SetEase(Ease.InOutSine);
#endif
    }


    void Update()
    {
#if !UNITY_EDITOR
        _camera.orthographicSize = DOVirtual.EasedValue(_startZoom, _endZoom, _zoomT, Ease.Linear);
#endif
    }
}
