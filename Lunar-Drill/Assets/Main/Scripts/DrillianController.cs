using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class DrillianController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField][Range(0.25f, 10f)] float _speed = 5;
    [SerializeField][Range(1f, 10f)] float _planetRadius = 2.5f;
    [SerializeField][Range(0.1f, 100f)] float _gravityStrength = 1f;
    [SerializeField][Range(1, 100f)] float _maxRotationControl = 25f;
    [SerializeField][Range(0.05f, 1f)] float _timeTillControlRegain = 0.25f;


    //--- Private Fields ------------------------

    Vector2 _goalMoveDirection;
    Rigidbody2D _rigidbody;
    bool _isBurrowed;
    bool _lastFrameIsBurrowed;
    Tweener _controlTween;
    public float RotationControlT { get; set; } = 1;

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

        SetControl();

        MoveUpDrillian();
        RotateDrillian();

        _lastFrameIsBurrowed = _isBurrowed;
    }

    //--- Public Methods ------------------------

    public void SetMoveDirectionInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 readValue = context.ReadValue<Vector2>();

        if (readValue.magnitude < 0.1f) return;

        _goalMoveDirection = readValue.normalized;
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

    void SetControl()
    {
        if (!_isBurrowed)
        {
            if (_controlTween != null)
            {
                _controlTween.Kill();
            }

            RotationControlT = 0;
        }
        
        if (!_lastFrameIsBurrowed && _isBurrowed)
        {
            Debug.Log("entered");
            _controlTween = DOTween.To(() => RotationControlT, x => RotationControlT = x, 1, _timeTillControlRegain).SetEase(Ease.InQuad);
        }
    }

    void RotateDrillian()
    {
        // Dies richtet sich an der derzeitigen Velocity aus.
        _rigidbody.MoveRotation(Vector2.SignedAngle(Vector2.up, _rigidbody.velocity.normalized));
    }

    void MoveUpDrillian()
    {
        if (!_isBurrowed) return;

        float rotationControl = Utilities.Remap(RotationControlT, 0, 1, 0, _maxRotationControl);
        Vector2 moveDirection = Vector3.RotateTowards(_rigidbody.velocity.normalized, _goalMoveDirection, rotationControl * Time.deltaTime, float.PositiveInfinity);
        
        // Move along up-Vector
        _rigidbody.velocity = moveDirection * _speed;
    }
}
