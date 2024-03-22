using DG.Tweening;
using Shapes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class LunaController : MonoBehaviour, IInputSubscriber<LunaShoot>, IInputSubscriber<LunaMoveGoal>
{
    //--- Exposed Fields ------------------------

    [Header("Configuration")]
    [SerializeField] [Range(0.1f, 10f)] float _maxRotationSpeed;
    [SerializeField] [Range(0.1f, 1f)] float _slowLaseringPercent;

    [Header("Movement Smoothing")]
    [SerializeField] [Range(0.1f, 100f)] float _movementStartAngleThreshold;
    [SerializeField] [Range(0.1f, 10f)] float _movementArrivedAngleThreshold;

    [Header("Laser")]
    [SerializeField] Line _laserVisual;
    [SerializeField] BoxCollider2D _laserCollider;
    [SerializeField] [Range(0.01f, 1f)] float _laserSpeed;

    [Header("Knockback")]
    [SerializeField] [Range(0f, 3f)] float _laserKnockbackStrength;
    [SerializeField] [Range(0.1f, 3f)] float _orbitReturnStrength;

    [Header("Collision")]
    [SerializeField] LayerMask _damageCollisions;
    [SerializeField] LayerMask _ores;
    [SerializeField] LayerMask _health;
    [SerializeField] [Range(0f, 5f)] float _invincibleTime;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] LunaSpriteIterator _spriteIterator;

    [Header("Energy")]
    [SerializeField] [Range(0.01f, 0.33f)] float _energyIncrease;
    [SerializeField] [Range(0.01f, 10f)] float _energyDecrease;

    [Header("VFX")]
    [SerializeField] VisualEffect _laserCharge;
    [SerializeField] VisualEffect _energyCollect;

    public int MoveSign { get; private set; } = 0;
    public float EnergyT { get; private set; } = 0.33f;


    //--- Private Fields ------------------------

    float _orbitRotationT = 0.33f;
    Vector2 _goalDirection;
    bool _mustReachThresholdForMovement = false;
    Rigidbody2D _rigidbody;
    Tweener _laserStartTween;
    Tweener _laserEndTween;
    bool _currentlyLasering = false;
    float _distanceFromOrbit = 0f;
    float _laserStartPoint = 0f;
    float _laserEndPoint = 0f;

    bool _isInvincible = false;


    //--- Unity Methods ------------------------

    public void OnValidate()
    {
        _movementArrivedAngleThreshold = Mathf.Min(_movementArrivedAngleThreshold, _movementStartAngleThreshold);
    }

    public void Awake()
    {
        _goalDirection = Vector2.zero;
        _rigidbody = GetComponent<Rigidbody2D>();
        InputBus.Subscribe<LunaMoveGoal>(this);
        InputBus.Subscribe<LunaShoot>(this);

        transform.position = Quaternion.Euler(0, 0, 120) * Vector2.up * Utilities.OuterOrbit;
        transform.rotation = Quaternion.Euler(0, 0, -60);
    }

    public void FixedUpdate()
    {
        DecreaseEnergy();
        SetLaserSize();
        CalculateDistanceFromOrbit();
        CalculateOrbitRotation();
        SetLunaPosition();
        SetLunaRotation();
    }


    //--- Public Methods ------------------------
    public void OnEventHappened(LunaShoot e) // Event to use Input System
    {
        ShootInput(e.context);
    }

    public void OnEventHappened(LunaMoveGoal e) // Event to use Input System
    {
        SetGoalDirectionInput(e.context);
    }


    public void SetLaserSize()
    {
        _laserStartPoint = 0.32f;
        _laserEndPoint = transform.position.magnitude - Utilities.PlanetRadius;

        // Move Laser
        bool startAnimating = _laserStartTween != null && _laserStartTween.IsActive();
        bool endAnimating = _laserEndTween != null && _laserEndTween.IsActive();

        // collider
        _laserCollider.offset = new Vector2(0, (_laserEndPoint - _laserStartPoint) / 2 + _laserStartPoint);
        _laserCollider.size = new Vector2(0.25f, _laserEndPoint - _laserStartPoint);

        if (startAnimating || endAnimating) return;
        if (!_currentlyLasering) return;

        _laserVisual.Start = new Vector3(0, _laserStartPoint, 0);
        _laserVisual.End = new Vector3(0, _laserEndPoint, 0);


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

            StartLasering();
        }
        else if (context.canceled)
        {
            if (!_currentlyLasering) return;

            EndLasering();
        }
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

    void StartLasering()
    {
        if (Mathf.Approximately(EnergyT, 0)) return;

        _laserCollider.enabled = false;
        _laserVisual.enabled = true;

        if (_laserStartTween != null && _laserStartTween.IsActive())
        {
            _laserStartTween.Kill();
        }

        if (_laserEndTween != null && _laserEndTween.IsActive())
        {
            _laserEndTween.Kill();
        }

        _currentlyLasering = true;

        _laserVisual.Start = new Vector3(0, _laserStartPoint, 0);
        _laserVisual.End = new Vector3(0, _laserStartPoint, 0);
        _laserStartTween = DOTween.To(() => _laserVisual.End, x => _laserVisual.End = x, new Vector3(0, _laserEndPoint, 0), _laserSpeed).SetEase(Ease.InQuad);
        _laserStartTween.OnComplete(() => _laserCollider.enabled = true);

        // VFX
        _laserCharge.SendEvent("Charge");
        _laserCharge.SetBool("Alive", true);
    }

    void EndLasering()
    {
        _currentlyLasering = false;

        _laserVisual.Start = new Vector3(0, _laserStartPoint, 0);
        _laserEndTween = DOTween.To(() => _laserVisual.Start, x => _laserVisual.Start = x, new Vector3(0, _laserEndPoint, 0), _laserSpeed).SetEase(Ease.InQuad);
        _laserEndTween.OnComplete(() =>
        {
            _laserCollider.enabled = false;
            _laserVisual.Start = new Vector3(0, _laserStartPoint, 0);
            _laserVisual.End = new Vector3(0, _laserStartPoint, 0);
        });

        // VFX
        _laserCharge.Stop();
        _laserCharge.SetBool("Alive", false);
    }

    void DecreaseEnergy()
    {
        if (!_currentlyLasering) return;

        EnergyT = Mathf.Clamp01(EnergyT - _energyDecrease * Time.deltaTime);

        if (Mathf.Approximately(0, EnergyT))
        {
            EndLasering();
        }
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
        Vector2 position = rotatedVector * (Utilities.OuterOrbit + _distanceFromOrbit);

        _rigidbody.MovePosition(position);
    }

    void SetLunaRotation()
    {
        _rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.forward, -transform.position));
    }

    void GetHit()
    {
        // Health Reduce
        _spriteIterator.Hit();
        FindObjectOfType<GameManager>().Hit(gameObject, true);

        // invincible
        _isInvincible = true;
        // Set invincible to false after one second
        DOVirtual.DelayedCall(_invincibleTime, () => _isInvincible = false, false);
        _spriteRenderer.DOColor(Color.clear, _invincibleTime).SetEase(Ease.Flash, 24, 0.75f);

        // Splash-Effect, 

        // Time Scale down

    }

    void GainHealth()
    {
        GameManager manager = FindObjectOfType<GameManager>();

        manager.Heal(gameObject, true);
    }

    void GainEnergy()
    {
        EnergyT = Mathf.Clamp01(EnergyT + _energyIncrease);
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
        else if (Utilities.LayerMaskContainsLayer(_ores, collision.gameObject.layer))
        {
            if (Mathf.Approximately(EnergyT, 1)) return;

            GainEnergy();

            // VFX
            _energyCollect.SendEvent("Collect");
            collision.gameObject.GetComponent<OreController>().DestroyOre();
        }
        else if (Utilities.LayerMaskContainsLayer(_health, collision.gameObject.layer))
        {
            GainHealth();
            collision.gameObject.GetComponent<HealthPickup>().DestroyPickup();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EvaluateCollision(collision);
    }
}
