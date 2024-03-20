using DG.Tweening;
using Shapes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LunaController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Extern Information")]
    [SerializeField][Range(2, 5)] float _planetRadius = 2.5f;
    [SerializeField][Range(2, 5)] float _outerOrbitRange;

    [Header("Configuration")]
    [SerializeField][Range(0.1f, 10f)] float _maxRotationSpeed;
    [SerializeField][Range(0.1f, 1f)] float _slowLaseringPercent;

    [Header("Movement Smoothing")]
    [SerializeField][Range(0.1f, 100f)] float _movementStartAngleThreshold;
    [SerializeField][Range(0.1f, 10f)] float _movementArrivedAngleThreshold;

    [Header("Laser")]
    [SerializeField] Line _laserVisual;
    [SerializeField] BoxCollider2D _laserCollider;
    [SerializeField][Range(0.01f, 1f)] float _laserSpeed;

    [Header("Knockback")]
    [SerializeField][Range(0.1f, 3f)] float _laserKnockbackStrength;
    [SerializeField][Range(0.1f, 3f)] float _orbitReturnStrength;


    public int MoveSign { get; private set; } = 0;

    //--- Private Fields ------------------------

    float _orbitRotationT;
    Vector2 _goalDirection;
    bool _mustReachThresholdForMovement = false;
    Rigidbody2D _rigidbody;
    Tweener _laserStartTween;
    Tweener _laserEndTween;
    bool _currentlyLasering = false;
    float _distanceFromOrbit = 0f;
    float _laserStartPoint = 0f;
    float _laserEndPoint = 0f;


    //--- Unity Methods ------------------------

    public void OnValidate()
    {
        _movementArrivedAngleThreshold = Mathf.Min(_movementArrivedAngleThreshold, _movementStartAngleThreshold);
    }

    public void Awake()
    {
        _orbitRotationT = 0;
        _goalDirection = Vector2.zero;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        SetLaserSize();
        CalculateDistanceFromOrbit();
        CalculateOrbitRotation();
        SetLunaPosition();
        SetLunaRotation();
    }


    //--- Public Methods ------------------------

    public void SetLaserSize()
    {
        _laserStartPoint = 0.32f;
        _laserEndPoint = transform.position.magnitude - _planetRadius;

        // Move Laser
        bool startAnimating = _laserStartTween != null && _laserStartTween.IsPlaying();
        bool endAnimating = _laserEndTween != null && _laserEndTween.IsPlaying();

        // collider
        _laserCollider.offset = new Vector2(0, (_laserEndPoint - _laserStartPoint) / 2 + _laserStartPoint);
        _laserCollider.size = new Vector2(0.25f, _laserEndPoint - _laserStartPoint);

        if (startAnimating || endAnimating) return;
        if (!_currentlyLasering) return;

        _laserVisual.Start = new Vector3(0, _laserStartPoint, 0);
        _laserVisual.End = new Vector3(0, _laserEndPoint, 0);

        
    }

    public void SetLaserColliderSize()
    {

    }

    public void CalculateDistanceFromOrbit()
    {
        if (!_currentlyLasering)
        {
            if (_laserEndTween != null && _laserEndTween.IsActive()) return;

            // reduce
            _distanceFromOrbit = Mathf.Max(0, _distanceFromOrbit -= _orbitReturnStrength * Time.deltaTime);
        }
        else
        {
            // increase
            _distanceFromOrbit = Mathf.Max(0, _distanceFromOrbit += _laserKnockbackStrength * Time.deltaTime);
        }

    }

    public void ShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_currentlyLasering) return;

            _laserCollider.enabled = false;

            if (_laserStartTween != null && _laserStartTween.IsPlaying())
            {
                _laserStartTween.Kill();
            }

            if (_laserEndTween != null && _laserEndTween.IsPlaying())
            {
                _laserEndTween.Kill();
            }

            _currentlyLasering = true;

            _laserVisual.Start = new Vector3(0, _laserStartPoint, 0);
            _laserVisual.End = new Vector3(0, _laserStartPoint, 0);
            _laserStartTween = DOTween.To(() => _laserVisual.End, x => _laserVisual.End = x, new Vector3(0, _laserEndPoint, 0), _laserSpeed).SetEase(Ease.InQuad);
            _laserStartTween.OnComplete(() => _laserCollider.enabled = true);
        }
        else if (context.canceled)
        {
            _currentlyLasering = false;

            _laserVisual.Start = new Vector3(0, _laserStartPoint, 0);
            _laserEndTween = DOTween.To(() => _laserVisual.Start, x => _laserVisual.Start = x, new Vector3(0, _laserEndPoint, 0), _laserSpeed).SetEase(Ease.InQuad);
            _laserEndTween.OnComplete(() => {
                _laserCollider.enabled = false;
                _laserVisual.Start = new Vector3(0, _laserStartPoint, 0);
                _laserVisual.End = new Vector3(0, _laserStartPoint, 0);
            });
        }

        // Shoot lazer!

        // Interpolate LaserStartup with Dotween -> OnEnd of that, activate collider

        // 

    }

    public void SetGoalDirectionInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 readValue = context.ReadValue<Vector2>();

        if (Utilities.Approximately(readValue, Vector2.zero))
        {
            _goalDirection = Vector2.zero;
        }
        else
        {
            _goalDirection = readValue.normalized;
        }
    }


    //--- Private Methods ------------------------

    void CalculateOrbitRotation()
    {
        if (_goalDirection.magnitude < 0.1f) return;

        Vector2 currentDirection = transform.position.normalized;

        float angle = Vector2.Angle(currentDirection, _goalDirection);

        if (_mustReachThresholdForMovement && _movementStartAngleThreshold > angle)
        {
            return;
        }


        if (angle < _movementArrivedAngleThreshold)
        {
            _mustReachThresholdForMovement = true;
            MoveSign = 0;
        }
        else
        {
            // Dot product to find out if you should move clockwise or counterclockwise
            _mustReachThresholdForMovement = false;
            MoveSign = -(int)Mathf.Sign(_goalDirection.x * currentDirection.y - _goalDirection.y * currentDirection.x);
        }

        float currentRotationSpeed = (_currentlyLasering) ? _maxRotationSpeed * (1 - _slowLaseringPercent) : _maxRotationSpeed;

        // increase
        _orbitRotationT += MoveSign * currentRotationSpeed * Time.deltaTime;
        // guard
        if (_orbitRotationT >= 1)
        {
            _orbitRotationT -= 1;
        }
    }

    void SetLunaPosition()
    {
        float angle = _orbitRotationT.Remap(0, 1, 0, 360);

        Vector2 rotatedVector = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        Vector2 position = rotatedVector * (_outerOrbitRange + _distanceFromOrbit);

        _rigidbody.MovePosition(position);
    }

    void SetLunaRotation()
    {
        _rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.forward, -transform.position));
    }

}
