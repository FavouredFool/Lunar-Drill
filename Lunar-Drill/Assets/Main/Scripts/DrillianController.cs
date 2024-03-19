using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class DrillianController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField][Range(0.25f, 10f)] float _speed = 5;
    [SerializeField][Range(1f, 10f)] float _planetRadius = 2.5f;
    [SerializeField][Range(0.1f, 100f)] float _gravityStrength = 1f;

    //--- Private Fields ------------------------

    Vector2 _moveDirection;
    Rigidbody2D _rigidbody;
    bool _isBurrowed;

    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        SetWorldGravity();

        SetIsBurrowed();
        ApplyGravity();

        RotateDrillian();
        MoveUpDrillian();
    }

    //--- Public Methods ------------------------

    public void SetMoveDirectionInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 readValue = context.ReadValue<Vector2>();

        if (readValue.magnitude < 0.1f) return;

        _moveDirection = readValue;
    }

    //--- Private Methods ------------------------

    void SetWorldGravity()
    {
        Physics2D.gravity = -transform.position * _gravityStrength;
    }

    void SetIsBurrowed()
    {
        _isBurrowed = transform.position.magnitude <= _planetRadius;
    }

    void ApplyGravity()
    {
        if (_isBurrowed)
        {
            _rigidbody.gravityScale = 0;
        }
        else
        {
            _rigidbody.gravityScale = 1;
        }
    }

    void RotateDrillian()
    {
        // Dies richtet sich an der derzeitigen Velocity aus.
        _rigidbody.MoveRotation(Vector2.SignedAngle(Vector2.up, _moveDirection));
    }

    void MoveUpDrillian()
    {
        if (!_isBurrowed) return;

        // Move along up-Vector
        _rigidbody.velocity = transform.up * _speed;
    }
}
