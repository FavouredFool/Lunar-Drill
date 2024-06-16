using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class DrillianController : MonoBehaviour, IInputSubscriber<DrillianMoveDirection>, IInputSubscriber<DrillianDash>
{
    //--- Exposed Fields ------------------------

    [FormerlySerializedAs("_speed")]
    [Header("Configuration")]
    [SerializeField] [Range(0.25f, 10f)] float _goalSpeed = 5;
    [SerializeField] [Range(0.1f, 100f)] float _minGravityStrength = 1f;
    [SerializeField] [Range(0.1f, 100f)] float _maxGravityStrength = 2f;
    [SerializeField] [Range(0.1f, 100f)] float _timeTillMaxGravityOutside = 2f;

    [Header("Control")]
    [SerializeField] [Range(1, 100f)] float _maxRotationControl = 25f;
    [SerializeField] [Range(0.05f, 1f)] float _timeTillControlRegain = 0.25f;
    [SerializeField] [Range(0.1f, 2)] float _stopTime = 0.5f;
    [SerializeField] [Range(0.5f, 2)] float _speedBoost = 1.2f;
    [SerializeField] [Range(1, 25)] int _speedAdjustmentDecay = 16;
    [SerializeField] [Range(1, 25)] int _rotationAdjustmentDecay = 16;

    //[Header("Air Movement")]
    //[SerializeField][Range(0f, 100f)] float _airTurnControl = 1f;
    //[SerializeField][Range(0f, 1f)] float _airTurnPercentage = 0.25f;
    //[SerializeField][Range(0f, 100f)] float _airTurnSmoothing = 15f;

    [Header("Collision")]
    [SerializeField] LayerMask _damageCollisions;
    [SerializeField] LayerMask _spiderCollision;
    [SerializeField] LayerMask _laserCollision;
    [SerializeField] LayerMask _health;
    [SerializeField] [Range(0f, 5f)] float _invincibleTime;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] DrillianSpriteIterator _spriteIterator;
    [SerializeField] SpriteRenderer _actionAvailiableToggleRenderer;

    [Header("Ores")]
    [SerializeField] [Range(0.125f, 5f)] float _oreDistance;

    [Header("VFX")]
    [SerializeField] private VisualEffect _drillImpactOut;
    [SerializeField] private VisualEffect _drillImpactIn;
    [SerializeField] private VisualEffect _speedBoostEffect;

    //--- Properties ------------------------

    public float RotationControlT { get; set; } = 1;
    public bool IsBurrowed { get; set; } = true;
    public bool LastFrameIsBurrowed { get; set; } = true;
    public List<OreController> FollowingOres { get; } = new();
    public float OreDistance => _oreDistance;
    public bool IsActionAvaliable { get; set; } = false;


    //--- Private Fields ------------------------

    Vector2 _goalMoveDirection;
    Rigidbody2D _rigidbody;

    Tweener _controlTween;
    Vector2 _airTurnDirection;
    Vector2 _turnDirection;

    float _gravityT = 0;
    Tween _gravityTTween;

    bool _isInvincible = false;
    bool _stopMovement = false;
    bool _isStomping = false;

    float _currentSpeed;

    bool _vfxIgnore = true;

    Rumble.Profile permanentRumble = null;

    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        InputBus.Subscribe<DrillianMoveDirection>(this);
        InputBus.Subscribe<DrillianDash>(this);

        transform.position = Quaternion.Euler(0, 0, 240) * Vector2.up * Utilities.OuterOrbit;
        transform.rotation = Quaternion.Euler(0, 0, 60);

        _currentSpeed = _goalSpeed;
    }

    private void OnDestroy()
    {
        InputBus.Unsubscribe<DrillianMoveDirection>(this);
        InputBus.Unsubscribe<DrillianDash>(this);
    }

    public void FixedUpdate()
    {
        SetWorldGravity();

        SetIsBurrowed();
        UpdateStomping();
        ApplyGravity();

        ReleaseOre();
        SetControl();

        UpdateSpeed();
        MoveUpDrillian();
        RotateDrillian();

        ShootDrillImpactParticles();

        UpdateOres();

        LastFrameIsBurrowed = IsBurrowed;
    }

    public void UpdateOres()
    {
        Transform previous = transform;
        for (int i = 0; i < FollowingOres.Count; i++)
        {
            Transform ore = FollowingOres[i].transform;

            Vector2
                prevPos = previous.position,
                myPos = ore.position,
                targetPos = prevPos + (myPos - prevPos).normalized * OreDistance,
                smoothPos = Vector2.Lerp(myPos, targetPos, 0.7f);

            ore.position = smoothPos;

            previous = ore;
        }
    }

    //--- Public Methods ------------------------
    public void OnEventHappened(DrillianMoveDirection e) // Control over Signal Bus.
    {
        SetMoveDirectionInput(e.context);
    }

    //--- Public Methods ------------------------
    public void OnEventHappened(DrillianDash e) // Event to use Input System
    {
        DrillianAction(e.context);
    }

    public void DrillianAction(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!IsActionAvaliable) return;

        IsActionAvaliable = false;
        LoseActionVisual();

        if (IsBurrowed)
        {
            ActionInsideMoon();
        }
        else
        {
            ActionOutsideMoon();
        }
    }

    public void SetMoveDirectionInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 readValue = context.ReadValue<Vector2>();

        _goalMoveDirection = readValue.normalized;
    }

    public void RefreshAction()
    {
        if (!IsActionAvaliable)
        {
            RegainActionVisual();
            DOVirtual.DelayedCall(_timeTillControlRegain * 2, () => IsActionAvaliable = true);
        }
    }


    //--- Private Methods ------------------------

    void UpdateStomping()
    {
        if (IsBurrowed && !LastFrameIsBurrowed)
        {
            _isStomping = false;
        }
    }

    void UpdateSpeed()
    {
        // Update speed towards goalSpeed (https://www.youtube.com/watch?v=LSNQuFEDOyQ)
        // useful range for decay is approx. 1 to 25, slow to fast
        _currentSpeed = _goalSpeed + (_currentSpeed - _goalSpeed) * Mathf.Exp(-_speedAdjustmentDecay * Time.deltaTime);
    }

    Vector2 UpdateDirection(Vector2 direction, Vector2 goalDirection)
    {
        return goalDirection + (direction - goalDirection) * Mathf.Exp(-_rotationAdjustmentDecay * Time.deltaTime);
    }

    void ReleaseOre()
    {
        if (!IsBurrowed && LastFrameIsBurrowed)
        {
            // Release all ores
            foreach (OreController ore in FollowingOres)
            {
                ore.ReleaseOre();
            }

            FollowingOres.Clear();
        }
    }

    void SetWorldGravity()
    {
        float gravity = DOVirtual.EasedValue(_minGravityStrength, _maxGravityStrength, _gravityT, Ease.Linear);
        Physics2D.gravity = -transform.position * gravity;
    }

    void SetIsBurrowed()
    {
        bool previous = IsBurrowed;
        IsBurrowed = transform.position.magnitude <= Utilities.PlanetRadius;

        if (previous != IsBurrowed)
        {
            if (IsBurrowed)
            {
                Rumble.main?.RumbleDrillian(3, 2, 0.2f);
                Rumble.main?.RumbleDrillian(0, 1f, 0.33f);
                Rumble.main?.RumbleDrillian(0, 0.25f, 0.66f);
            }
            else
            {
                Rumble.main?.RumbleDrillian(3, 1f, 0.1f);
            }
        }
    }

    void ActionInsideMoon()
    {
        _stopMovement = true;
        DOVirtual.DelayedCall(_stopTime, () => _stopMovement = false, false)
            .OnComplete(() => _currentSpeed *= _speedBoost);
        DOVirtual.DelayedCall(Mathf.Max(_stopTime - .2f, 0), () => _stopMovement = false, false)
           .OnComplete(() =>
           {
               // VFX 
               _speedBoostEffect.SendEvent("Boost");
           });
    }

    void ActionOutsideMoon()
    {
        _isStomping = true;
        _stopMovement = true;
        DOVirtual.DelayedCall(_stopTime, () => _stopMovement = false, false)
           .OnComplete(() => _currentSpeed *= _speedBoost);
        DOVirtual.DelayedCall(Mathf.Max(_stopTime - .2f, 0), () => _stopMovement = false, false)
           .OnComplete(() =>
           {
               // VFX 
               _speedBoostEffect.SendEvent("Boost");
           });
    }

    void ApplyGravity()
    {
        if (IsBurrowed || _isStomping)
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

        if (LastFrameIsBurrowed && !IsBurrowed)
        {
            if (_gravityTTween != null && _gravityTTween.IsActive())
            {
                _gravityTTween.Kill();
            }

            _gravityT = 0;
            _gravityTTween = DOTween.To(() => _gravityT, x => _gravityT = x, 1, _timeTillMaxGravityOutside);
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

            if (_isStomping)
            {
                Vector2 currentDirection = Quaternion.Euler(0, 0, _rigidbody.rotation) * Vector2.up;
                lookDirection = UpdateDirection(currentDirection, -transform.position.normalized);
            }
            else
            {
                lookDirection = _rigidbody.velocity.normalized;
            }
        }

        _rigidbody.MoveRotation(Vector2.SignedAngle(Vector2.up, lookDirection));
    }

    void LoseActionVisual()
    {
        _actionAvailiableToggleRenderer.enabled = false;
    }

    void RegainActionVisual()
    {
        float scaleMax = _actionAvailiableToggleRenderer.transform.localScale.x;
        _actionAvailiableToggleRenderer.transform.localScale = Vector3.zero;
        _actionAvailiableToggleRenderer.enabled = true;
        DOTween.To(() => _actionAvailiableToggleRenderer.transform.localScale,
            x => _actionAvailiableToggleRenderer.transform.localScale = x, Vector3.one * scaleMax,
            _timeTillControlRegain * 2f);
    }

    void MoveUpDrillian()
    {
        if (!IsBurrowed && !_isStomping) return;

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
        _rigidbody.velocity = (!_stopMovement) ? moveDirection * _currentSpeed : moveDirection * _currentSpeed / 100f;
    }

    void GetHit()
    {
        // Camera shake
        CamShake.Instance.ShakeCamera();
        Rumble.main?.RumbleDrillian(4, 2, 0.2f);

        // Health Reduce
        _spriteIterator.Hit();
        FindObjectOfType<GameManager>().Hit(gameObject, true);

        // invincible
        _isInvincible = true;
        // Set invincible to false after one second
        DOVirtual.DelayedCall(_invincibleTime, () => _isInvincible = false, false);
        _spriteRenderer.DOColor(Color.clear, _invincibleTime).SetEase(Ease.Flash, 24, 0.75f);

        // remove all ores
        foreach (OreController ore in FollowingOres)
        {
            ore.DestroyOre();
        }

        FollowingOres.Clear();
    }

    void EvaluateCollision(Collider2D collision)
    {
        if (Utilities.LayerMaskContainsLayer(_damageCollisions, collision.gameObject.layer))
        {
            SpiderController spider = FindObjectOfType<SpiderController>();

            if (spider == null) throw new System.Exception();

            if (!_isInvincible && !spider.IsNotHurtingOnTouch)
            {
                GetHit();

                if (Utilities.LayerMaskContainsLayer(_spiderCollision, collision.gameObject.layer))
                {
                    AudioController.Fire(new DrillianHitSpider(""));
                }
                else if (Utilities.LayerMaskContainsLayer(_laserCollision, collision.gameObject.layer))
                {
                    AudioController.Fire(new DrillianHitLaser(""));
                }
            }
        }
        else if (Utilities.LayerMaskContainsLayer(_health, collision.gameObject.layer))
        {
            HealthPickup health = collision.gameObject.GetComponent<HealthPickup>();

            GameManager gameManager = FindObjectOfType<GameManager>();

            if (!health.HasBeenPickedUp && gameManager.PlayerHP != gameManager.PlayerMaxHP)
            {
                if (!health.PickupableByDrillian) return;

                health.HasBeenPickedUp = true;
                GainHealth();
                health.DestroyPickup();
            }
        }
    }

    void GainHealth()
    {
        GameManager manager = FindObjectOfType<GameManager>();

        AudioController.Fire(new LunaEnergyPickup(""));

        manager.Heal(gameObject, true);
        Rumble.main?.RumbleDrillian(1, 2, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EvaluateCollision(collision);
    }


    /* Plays VFX when Drillian leaves or enters planet. */
    private void ShootDrillImpactParticles()
    {
        if (_vfxIgnore)
        {
            _vfxIgnore = !_vfxIgnore;
            return;
        }

        if (!IsBurrowed && LastFrameIsBurrowed)
        {
            _drillImpactOut.SetVector3("StartPosition", transform.position);
            _drillImpactOut.SetVector3("DrillianUp", transform.up);
            _drillImpactOut.SendEvent("Shoot");

            AudioController.Fire(new DrillianDrilling(DrillianDrilling.DrillState.DrillingStopped));
            AudioController.Fire(new DrillianChangeMode(""));
        }
        else if (IsBurrowed && !LastFrameIsBurrowed)
        {
            _drillImpactIn.SetVector3("StartPosition", transform.position);
            _drillImpactIn.SetVector3("DrillianUp", transform.up);
            _drillImpactIn.SendEvent("Shoot");

            AudioController.Fire(new DrillianDrilling(DrillianDrilling.DrillState.DrillingStarted));
            AudioController.Fire(new DrillianChangeMode(""));
        }
    }


}
