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
    [SerializeField] GameObject _laserSprite;
    [SerializeField] BoxCollider2D _laserCollider;
    [SerializeField] [Range(0.01f, 1f)] float _laserSpeed;

    [Header("Knockback")]
    [SerializeField] [Range(0f, 3f)] float _laserKnockbackStrength;
    [SerializeField] [Range(0.1f, 3f)] float _orbitReturnStrength;

    [Header("Collision")]
    [SerializeField] LayerMask _damageCollisions;
    [SerializeField] LayerMask _drillian;
    [SerializeField] LayerMask _laser;
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
    [SerializeField] GameObject _energyEmpty;

    public int MoveSign { get; private set; } = 0;
    public float EnergyT { get; private set; } = 0.33f;
    public bool EnergyGained { get; private set; } = false;
    public bool EnergyCritical => EnergyT < 0.05f;
    public bool EnergyFull => EnergyT > 0.95f;
    public bool EnergyEmpty => _energyEmpty.activeSelf;

    public bool CurrentlyLasering => _currentlyLasering;


    //--- Private Fields ------------------------

    float _orbitRotationT = 0.33f;
    Vector2 _goalDirection;
    bool _mustReachThresholdForMovement = false;
    Rigidbody2D _rigidbody;
    Tweener _laserStartTweenPosition;
    Tweener _laserStartTweenScale;
    Tweener _laserEndTweenPosition;
    Tweener _laserEndTweenScale;
    bool _currentlyLasering = false;
    bool _laseringLastFrame = false;
    float _distanceFromOrbit = 0f;
    float _laserStartPoint = 0f;
    float _laserEndPoint = 0f;

    bool _isInvincible = false;

    Rumble.Profile permanentRumble = null;

    [SerializeField] LunaControllerMarker _lunaControllMarker;


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

    private void OnDestroy()
    {
        InputBus.Unsubscribe<LunaMoveGoal>(this);
        InputBus.Unsubscribe<LunaShoot>(this);
    }

    public void FixedUpdate()
    {
        DecreaseEnergy();
        SetLaserSize();
        CalculateDistanceFromOrbit();
        CalculateOrbitRotation();
        SetLunaPosition();
        SetLunaRotation();
        LunaSound();

        _laseringLastFrame = _currentlyLasering;
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


    public void LunaSound()
    {
        if (_currentlyLasering)
        {
            AudioController.Fire(new LunaLaserFiring(LunaLaserFiring.LaserState.LaserFiring, EnergyT));
        }
        else if (_laseringLastFrame)
        {
            AudioController.Fire(new LunaLaserFiring(LunaLaserFiring.LaserState.LaserStopped, EnergyT));
        }
    }

    public void SetLaserSize()
    {
        _laserStartPoint = 0.32f;
        _laserEndPoint = transform.position.magnitude - Utilities.PlanetRadius;

        // Move Laser
        bool startAnimating = (_laserStartTweenPosition != null && _laserStartTweenPosition.IsActive()) || (_laserStartTweenScale != null && _laserStartTweenScale.IsActive());
        bool endAnimating = (_laserEndTweenPosition != null && _laserEndTweenPosition.IsActive()) || (_laserEndTweenScale != null && _laserEndTweenScale.IsActive());

        // collider
        _laserCollider.offset = new Vector2(0, (_laserEndPoint - _laserStartPoint) / 2 + _laserStartPoint);
        _laserCollider.size = new Vector2(0.25f, _laserEndPoint - _laserStartPoint);

        if (startAnimating || endAnimating) return;
        if (!_currentlyLasering) return;

        float length = Math.Abs(_laserStartPoint - _laserEndPoint);
        _laserSprite.transform.localScale = new Vector3(0.045f, length, 0);
        _laserSprite.transform.localPosition = new Vector3(0, length / 2 + _laserStartPoint, 0);
    }

    public void CalculateDistanceFromOrbit()
    {
        if (!_currentlyLasering)
        {
            if (_laserEndTweenPosition != null && _laserEndTweenPosition.IsActive()) return;

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
            if (!_currentlyLasering)
            {
                _energyEmpty.SetActive(false); // Need to stop animations that play while player is firing without having energy
                return;
            }

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

        _lunaControllMarker.SetGoalDirectionInput(readValue);
    }

    public void PauseInput(InputAction.CallbackContext context)
    {
        Debug.Log("PAUSE");
        if (context.performed)
        {
            InputBus.Fire(new Pause(context));
        }
    }


    //--- Private Methods ------------------------

    void StartLasering()
    {
        if (Mathf.Approximately(EnergyT, 0))
        {
            _energyEmpty.SetActive(true); // Show player that laser cannot fire atm
            return;
        }

        _laserCollider.enabled = false;
        _laserSprite.SetActive(true);

        if (_laserStartTweenPosition != null && _laserStartTweenPosition.IsActive())
            _laserStartTweenPosition.Kill();
        if (_laserStartTweenScale != null && _laserStartTweenScale.IsActive())
            _laserStartTweenScale.Kill();
        if (_laserEndTweenPosition != null && _laserEndTweenPosition.IsActive())
            _laserEndTweenPosition.Kill();
        if (_laserEndTweenScale != null && _laserEndTweenScale.IsActive())
            _laserEndTweenScale.Kill();


        _currentlyLasering = true;

        float length = Math.Abs(_laserStartPoint - _laserEndPoint);
        _laserStartTweenPosition = DOTween.To(() => _laserSprite.transform.localPosition, x => _laserSprite.transform.localPosition = x, new Vector3(0, length / 2 + _laserStartPoint, 0), _laserSpeed).SetEase(Ease.InQuad);
        _laserStartTweenScale = DOTween.To(() => _laserSprite.transform.localScale, x => _laserSprite.transform.localScale = x, new Vector3(0.045f, length, 0), _laserSpeed).SetEase(Ease.InQuad);
        _laserStartTweenScale.OnComplete(() => _laserCollider.enabled = true);

        // VFX
        _laserCharge.SendEvent("Charge");
        _laserCharge.SetBool("Alive", true);

        Rumble.main?.RumbleLuna(3, 0.5f, 0.1f);
        permanentRumble = Rumble.main?.RumbleLuna(1, 0.5f);
    }

    void EndLasering()
    {
        _currentlyLasering = false;

        float length = Math.Abs(_laserStartPoint - _laserEndPoint);
        _laserEndTweenPosition = DOTween.To(() => _laserSprite.transform.localPosition, x => _laserSprite.transform.localPosition = x, new Vector3(0, length / 2 + _laserStartPoint, 0), _laserSpeed).SetEase(Ease.InQuad);
        _laserEndTweenScale = DOTween.To(() => _laserSprite.transform.localScale, x => _laserSprite.transform.localScale = x, new Vector3(0.045f, length, 0), _laserSpeed).SetEase(Ease.InQuad);
        _laserEndTweenScale.OnComplete(() =>
        {
            _laserCollider.enabled = false;
            _laserSprite.SetActive(false);
            _laserSprite.transform.localScale = new Vector3(0.045f, 0, 0);
            _laserSprite.transform.localPosition = new Vector3(0, _laserStartPoint, 0);
        });

        // VFX
        _laserCharge.Stop();
        _laserCharge.SetBool("Alive", false);

        Rumble.main?.RemoveRumbleAnywhere(permanentRumble);
    }

    void DecreaseEnergy()
    {
        if (!_currentlyLasering) return;

        EnergyT = Mathf.Clamp01(EnergyT - _energyDecrease * Time.deltaTime);

        if (Mathf.Approximately(0, EnergyT))
        {
            EndLasering();
            _energyEmpty.SetActive(true); // if energy depletes while holding button, start empty animation
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
        // Camera shake
        CamShake.Instance.ShakeCamera();
        Rumble.main?.RumbleLuna(4, 2, 0.2f);

        // Health Reduce
        _spriteIterator.Hit();
        FindObjectOfType<GameManager>().Hit(gameObject, true);

        // invincible
        _isInvincible = true;
        // Set invincible to false after one second
        DOVirtual.DelayedCall(_invincibleTime, () => _isInvincible = false, false);
        _spriteRenderer.DOColor(Color.clear, _invincibleTime).SetEase(Ease.Flash, 24, 0.75f);
    }

    void GainHealth()
    {
        GameManager manager = FindObjectOfType<GameManager>();

        AudioController.Fire(new LunaEnergyPickup(""));

        manager.Heal(gameObject, true);
        Rumble.main?.RumbleLuna(1, 2, 0.1f);
    }

    void GainEnergy()
    {
        EnergyT = Mathf.Clamp01(EnergyT + _energyIncrease);
        Rumble.main?.RumbleLuna(1, 1, 0.1f);
        EnergyGained = true;
        DOVirtual.DelayedCall(1f, () => { EnergyGained = false; });
        if (!_currentlyLasering && _energyEmpty.activeSelf) // this is true, if player kept holding fire button while laser emtpy. New energy will be used immediatly without having to re-press button.
        {
            _energyEmpty.SetActive(false);
            StartLasering();
        }
    }

    void EvaluateCollision(Collider2D collision)
    {
        if (Utilities.LayerMaskContainsLayer(_damageCollisions, collision.gameObject.layer))
        {
            if (!_isInvincible)
            {
                GetHit();

                if (Utilities.LayerMaskContainsLayer(_drillian, collision.gameObject.layer))
                {
                    AudioController.Fire(new LunaHitDrillian(""));
                }
                else if (Utilities.LayerMaskContainsLayer(_laser, collision.gameObject.layer))
                {
                    AudioController.Fire(new LunaHitLaser(""));
                }

            }
        }
        else if (Utilities.LayerMaskContainsLayer(_ores, collision.gameObject.layer))
        {
            if (Mathf.Approximately(EnergyT, 1)) return;

            // VFX
            if (!collision.gameObject.GetComponent<OreController>().Collected)
            {
                GainEnergy();

                _energyCollect.SendEvent("Collect");

                AudioController.Fire(new LunaEnergyPickup(""));

                collision.gameObject.GetComponent<OreController>().Collected = true;
            }

            collision.gameObject.GetComponent<OreController>().DestroyOre();
        }
        else if (Utilities.LayerMaskContainsLayer(_health, collision.gameObject.layer))
        {
            HealthPickup health = collision.gameObject.GetComponent<HealthPickup>();

            GameManager gameManager = FindObjectOfType<GameManager>();

            if (!health.HasBeenPickedUp && gameManager.PlayerHP != gameManager.PlayerMaxHP)
            {
                health.HasBeenPickedUp = true;
                GainHealth();
                health.DestroyPickup();
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EvaluateCollision(collision);
    }


}
