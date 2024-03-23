using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.VFX;

public class DrillianController : MonoBehaviour, IInputSubscriber<DrillianMoveDirection>
{
    //--- Exposed Fields ------------------------

    [Header("Configuration")]
    [SerializeField] [Range(0.25f, 10f)] float _speed = 5;
    [SerializeField] [Range(0.1f, 100f)] float _minGravityStrength = 1f;
    [SerializeField] [Range(0.1f, 100f)] float _maxGravityStrength = 2f;
    [SerializeField] [Range(0.1f, 100f)] float _timeTillMaxGravityOutside = 2f;

    [Header("Control")]
    [SerializeField] [Range(1, 100f)] float _maxRotationControl = 25f;
    [SerializeField] [Range(0.05f, 1f)] float _timeTillControlRegain = 0.25f;

    //[Header("Air Movement")]
    //[SerializeField][Range(0f, 100f)] float _airTurnControl = 1f;
    //[SerializeField][Range(0f, 1f)] float _airTurnPercentage = 0.25f;
    //[SerializeField][Range(0f, 100f)] float _airTurnSmoothing = 15f;

    [Header("Collision")]
    [SerializeField] LayerMask _damageCollisions;
    [SerializeField] LayerMask _spiderCollision;
    [SerializeField] LayerMask _laserCollision;
    [SerializeField] [Range(0f, 5f)] float _invincibleTime;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] DrillianSpriteIterator _spriteIterator;

    [Header("Ores")]
    [SerializeField] [Range(0.125f, 5f)] float _oreDistance;

    [Header("VFX")]
    [SerializeField] private VisualEffect _drillImpactOut;
    [SerializeField] private VisualEffect _drillImpactIn;

    //--- Properties ------------------------

    public float RotationControlT { get; set; } = 1;
    public bool IsBurrowed { get; set; } = true;
    public bool LastFrameIsBurrowed { get; set; } = true;
    public List<OreController> FollowingOres { get; } = new();
    public float OreDistance => _oreDistance;


    //--- Private Fields ------------------------

    Vector2 _goalMoveDirection;
    Rigidbody2D _rigidbody;

    Tweener _controlTween;
    Vector2 _airTurnDirection;
    Vector2 _turnDirection;

    float _gravityT = 0;
    Tween _gravityTTween;

    bool _isInvincible = false;

    bool _vfxIgnore = true;

    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        InputBus.Subscribe(this);

        transform.position = Quaternion.Euler(0, 0, 240) * Vector2.up * Utilities.OuterOrbit;
        transform.rotation = Quaternion.Euler(0, 0, 60);
    }

    private void OnDestroy()
    {
        InputBus.Unsubscribe(this);
    }

    public void FixedUpdate()
    {
        SetWorldGravity();

        SetIsBurrowed();
        ApplyGravity();

        ReleaseOre();
        SetControl();

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

    public void SetMoveDirectionInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 readValue = context.ReadValue<Vector2>();

        _goalMoveDirection = readValue.normalized;
    }

    //--- Private Methods ------------------------

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
                Rumble.main?.AddRumble(ChosenCharacter.drillian, new Vector2(0.1f, 0.3f));
                Rumble.main?.AddRumble(ChosenCharacter.drillian, new Vector2(0.5f, 0.5f),0.1f);
            }
            else
            {
                Rumble.main?.RemovePermanentRumble(ChosenCharacter.drillian, new Vector2(0.1f, 0.3f));
                Rumble.main?.AddRumble(ChosenCharacter.drillian, new Vector2(0.5f, 0.5f), 0.1f);
            }
        }
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
        // Camera shake
        CamShake.Instance.ShakeCamera();

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

        Rumble.main?.AddRumble(ChosenCharacter.drillian, new Vector2(0.9f, 1f), 0.1f);
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
        }
        else if (IsBurrowed && !LastFrameIsBurrowed)
        {
            _drillImpactIn.SetVector3("StartPosition", transform.position);
            _drillImpactIn.SetVector3("DrillianUp", transform.up);
            _drillImpactIn.SendEvent("Shoot");
        }
    }


}
