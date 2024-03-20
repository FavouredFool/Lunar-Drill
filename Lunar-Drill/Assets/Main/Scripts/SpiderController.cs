using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] bool _constantlyRotating = false;
    [SerializeField][Range(2, 5)] float _innerOrbitRange = 3;
    [SerializeField][Range(0.01f, 1f)] float _rotationSpeed = 5f;

    [Header("Movement Smoothing")]
    [SerializeField][Range(0.1f, 100f)] float _movementStartAngleThreshold;
    [SerializeField][Range(0.1f, 10f)] float _movementArrivedAngleThreshold;


    //--- Private Fields ------------------------

    Rigidbody2D _rigidbody;
    float _orbitRotationT = 0.75f;
    SpiderLaser _spiderLaser;
    bool _mustReachThresholdForMovement = false;
    Vector2 _goalRotation = Vector2.zero;


    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spiderLaser = GetComponent<SpiderLaser>();
    }

    public void Start()
    {
        StartCoroutine(DetermineMoves());
    }

    public void FixedUpdate()
    {
        CalculateOrbitRotation();
        SetSpiderPosition();
        SetSpiderRotation();
    }


    //--- Private Methods ------------------------

    IEnumerator DetermineMoves()
    {
        yield return null;

        StartCoroutine(_spiderLaser.ShootLaser());

        yield return new WaitForSeconds(1f);

        _goalRotation = Vector2.down;

        yield return new WaitForSeconds(5f);

        _goalRotation = Vector2.up;
    }

    void CalculateOrbitRotation()
    {
        if (_goalRotation.magnitude < 0.1f) return;

        Vector2 currentDirection = transform.position.normalized;

        float angle = Vector2.Angle(currentDirection, _goalRotation);

        if (_mustReachThresholdForMovement && _movementStartAngleThreshold > angle)
        {
            return;
        }

        int sign;

        if (angle < _movementArrivedAngleThreshold)
        {
            _mustReachThresholdForMovement = true;
            sign = 0;
        }
        else
        {
            // Dot product to find out if you should move clockwise or counterclockwise
            _mustReachThresholdForMovement = false;
            sign = -(int)Mathf.Sign(_goalRotation.x * currentDirection.y - _goalRotation.y * currentDirection.x);
        }

        // increase
        _orbitRotationT += sign * _rotationSpeed * Time.deltaTime;

        // guard
        if (_orbitRotationT >= 1)
        {
            _orbitRotationT -= 1;
        }
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
