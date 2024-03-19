using UnityEngine;
using UnityEngine.InputSystem;

public class LunaController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField][Range(2,5)] float _innerOrbitRange;
    [SerializeField][Range(2, 5)] float _outerOrbitRange;
    [SerializeField][Range(0.1f, 10f)] float _rotationSpeed;
    [SerializeField][Range(0.1f, 10f)] float _graviationSpeed;
    [SerializeField][Range(0.1f, 100f)] float _movementStartAngleThreshold;
    [SerializeField][Range(0.1f, 10f)] float _movementArrivedAngleThreshold;

    //--- Private Fields ------------------------

    float _orbitRotationT;
    float _orbitDistanceT;
    Vector2 _goalDirection;
    bool _mustReachThresholdForMovement = false;
    Rigidbody2D _rigidbody;


    //--- Unity Methods ------------------------

    public void OnValidate()
    {
        _innerOrbitRange = Mathf.Min(_innerOrbitRange, _outerOrbitRange);
        _movementArrivedAngleThreshold = Mathf.Min(_movementArrivedAngleThreshold, _movementStartAngleThreshold);
    }

    public void Awake()
    {
        _orbitRotationT = 0;
        _orbitDistanceT = 1;
        _goalDirection = Vector2.zero;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        CalculateOrbitDistance();
        CalculateOrbitRotation();
        SetLunaPosition();
        SetLunaRotation();
    }


    //--- Public Methods ------------------------

    public void ShootInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        _orbitDistanceT += 0.1f;
    }

    public void SetGoalDirectionInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        _goalDirection = context.ReadValue<Vector2>();
    }


    //--- Private Methods ------------------------

    void CalculateOrbitDistance()
    {
        // move
        _orbitDistanceT -= _graviationSpeed * Time.deltaTime;
        // guard
        _orbitDistanceT = Mathf.Clamp01(_orbitDistanceT);
    }

    void CalculateOrbitRotation()
    {
        if (_goalDirection.magnitude < 0.1f) return;

        Vector2 currentDirection = transform.position.normalized;

        float angle = Vector2.Angle(currentDirection, _goalDirection);

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
            sign = -(int)Mathf.Sign(_goalDirection.x * currentDirection.y - _goalDirection.y * currentDirection.x);
        }

        // increase
        _orbitRotationT += sign * _rotationSpeed * Time.deltaTime;
        // guard
        if (_orbitRotationT >= 1)
        {
            _orbitRotationT -= 1;
        }
    }

    void SetLunaPosition()
    {
        float angle = _orbitRotationT.Remap(0, 1, 0, 360);
        float distance = _orbitDistanceT.Remap(0, 1, _innerOrbitRange, _outerOrbitRange);

        Vector2 rotatedVector = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        Vector2 position = rotatedVector * distance;

        _rigidbody.MovePosition(position);
    }

    void SetLunaRotation()
    {
        _rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.forward, -transform.position));
    }

}
