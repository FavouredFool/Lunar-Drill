using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class DrillianController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Extern Information")]
    [SerializeField][Range(1f, 10f)] float _planetRadius = 2.5f;

    [Header("Configuration")]
    [SerializeField][Range(0.25f, 10f)] float _speed = 5;
    [SerializeField][Range(0.1f, 100f)] float _gravityStrength = 1f;

    [Header("Control")]
    [SerializeField][Range(1, 100f)] float _maxRotationControl = 25f;
    [SerializeField][Range(0.05f, 1f)] float _timeTillControlRegain = 0.25f;

    //[Header("Air Movement")]
    //[SerializeField][Range(0f, 100f)] float _airTurnControl = 1f;
    //[SerializeField][Range(0f, 1f)] float _airTurnPercentage = 0.25f;
    //[SerializeField][Range(0f, 100f)] float _airTurnSmoothing = 15f;

    [Header("Collision")]
    [SerializeField] LayerMask _damageCollisions;
    [SerializeField][Range(0f, 5f)] float _invincibleTime;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer _spriteRenderer;


    //--- Properties ------------------------
    public float RotationControlT { get; set; } = 1;
    public bool IsBurrowed { get; set; }
    public bool LastFrameIsBurrowed { get; set; }


    //--- Private Fields ------------------------

    Vector2 _goalMoveDirection;
    Rigidbody2D _rigidbody;

    Tweener _controlTween;
    Vector2 _airTurnDirection;
    Vector2 _turnDirection;

    bool _isInvincible = false;


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

        LastFrameIsBurrowed = IsBurrowed;
    }

    //--- Public Methods ------------------------

    public void SetMoveDirectionInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 readValue = context.ReadValue<Vector2>();

        _goalMoveDirection = readValue.normalized;
    }

    //--- Private Methods ------------------------

    void SetWorldGravity()
    {
        Physics2D.gravity = -transform.position * _gravityStrength;
    }

    void SetIsBurrowed()
    {
        IsBurrowed = transform.position.magnitude <= _planetRadius;
    }

    void ApplyGravity()
    {
        if (IsBurrowed)
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
        if (!IsBurrowed)
        {
            if (_controlTween != null)
            {
                _controlTween.Kill();
            }

            RotationControlT = 0;
        }
        
        if (!LastFrameIsBurrowed && IsBurrowed)
        {
            _controlTween = DOTween.To(() => RotationControlT, x => RotationControlT = x, 1, _timeTillControlRegain);
        }
    }

    void RotateDrillian()
    {
        if (!IsBurrowed && LastFrameIsBurrowed)
        {
            _airTurnDirection = _rigidbody.velocity.normalized;
            _turnDirection = _airTurnDirection;
        }

        Vector2 lookDirection;
        if (IsBurrowed)
        {
            lookDirection = _rigidbody.velocity.normalized;
        }
        else
        {
            //// a certain percentage of the rotation in the air is the velocity, another comes from the airTurnDirection
            //_airTurnDirection = (_rigidbody.velocity.normalized * (1 - _airTurnPercentage) + _airTurnDirection * _airTurnPercentage).normalized;
            //
            //_airTurnDirection = Vector3.RotateTowards(_airTurnDirection, _goalMoveDirection, _airTurnControl * Time.deltaTime, float.PositiveInfinity);
            //
            //_turnDirection = Vector3.RotateTowards(_turnDirection, _airTurnDirection, _airTurnSmoothing * Time.deltaTime, float.PositiveInfinity);
            //
            //lookDirection = _turnDirection;

            lookDirection = _rigidbody.velocity.normalized;
        }


        _rigidbody.MoveRotation(Vector2.SignedAngle(Vector2.up, lookDirection));
    }

    void MoveUpDrillian()
    {
        if (!IsBurrowed) return;

        Vector2 moveDirection;

        if (!LastFrameIsBurrowed || _goalMoveDirection.magnitude < 0.1f)
        {
            moveDirection = transform.up;
        }
        else
        {
            float rotationControl = Utilities.Remap(RotationControlT, 0, 1, 0, _maxRotationControl);
            moveDirection = Vector3.RotateTowards(_rigidbody.velocity.normalized, _goalMoveDirection, rotationControl * Time.deltaTime, float.PositiveInfinity);
        }

        
        // Move along up-Vector
        _rigidbody.velocity = moveDirection * _speed;
    }

    void GetHit()
    {
        // Health Reduce
        FindObjectOfType<GameManager>().PlayerHP -= 1;

        // invincible
        _isInvincible = true;
        // Set invincible to false after one second
        DOVirtual.DelayedCall(_invincibleTime, () => _isInvincible = false, false);
        _spriteRenderer.DOColor(Color.clear, _invincibleTime).SetEase(Ease.Flash, 24, 0.75f);


        // Splash-Effect, 

        // Time Scale down

    }

    void EvaluateCollision(Collider2D collision)
    {
        if (Utilities.LayerMaskContainsLayer(_damageCollisions, collision.gameObject.layer))
        {
            if (!_isInvincible)
            {
                GetHit();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EvaluateCollision(collision);
    }

}
