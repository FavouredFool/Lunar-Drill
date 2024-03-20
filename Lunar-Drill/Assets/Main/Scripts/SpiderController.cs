using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField][Range(2, 5)] float _innerOrbitRange = 3;
    [SerializeField][Range(0.01f, 1f)] float _rotationSpeed = 5f;


    //--- Private Fields ------------------------

    Rigidbody2D _rigidbody;
    float _orbitRotationT = 0.75f;
    SpiderLaser _spiderLaser;


    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spiderLaser = GetComponent<SpiderLaser>();
    }

    public void Start()
    {
        StartCoroutine(_spiderLaser.ShootLaser());
    }

    public void FixedUpdate()
    {
        AdjustPositionT();
        SetSpiderPosition();
        SetSpiderRotation();
    }


    //--- Private Methods ------------------------

    void AdjustPositionT()
    {
        _orbitRotationT += _rotationSpeed * Time.deltaTime * Mathf.Sin(Time.time * 2);
    }

    void SetSpiderPosition()
    {
        float angle = _orbitRotationT.Remap(0, 1, 0, 360);

        Vector2 rotatedVector = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        Vector2 position = rotatedVector * _innerOrbitRange;

        _rigidbody.MovePosition(position);
    }

    void SetSpiderRotation()
    {
        _rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.forward, transform.position));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"hit on spider: {collision}");
    }
}
