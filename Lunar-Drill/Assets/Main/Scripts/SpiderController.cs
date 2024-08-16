

using System;
using DG.Tweening;
using System.Collections;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class SpiderController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Manager")]
    [SerializeField] SpiderManager _spiderManager;
    
    [Header("Speed")]
    [SerializeField] [Range(0.01f, 10f)] float _rotationSpeed = 1f;

    [Header("Movement Smoothing")]
    [SerializeField] [Range(0.1f, 100f)] float _movementStartAngleThreshold;
    [SerializeField] [Range(0.1f, 10f)] float _movementArrivedAngleThreshold;
    [SerializeField] [Range(1, 25)] int _rotationAdjustmentDecay = 16;
    [SerializeField] LayerMask _lunaLaser;
    [SerializeField] LayerMask _drillian;
    
    [Header("Digging")]
    [SerializeField] [Range(1, 100f)] float _maxRotationControl = 7.5f;

    [SerializeField] [Range(1, 100f)] float _digSpeed = 10;

    [Header("Overheat")]
    [SerializeField] [Range(0.01f, 1)] float _overheatGain = 0.25f;
    [SerializeField] [Range(0.01f, 1)] float _overheatLoss = 0.05f;

    [Header("Sprites")]
    [SerializeField] SpriteRenderer[] _spriteRenderers;
    [SerializeField] SpiderSpriteIterator _spriteIterator;

    [Header("Hit")]
    [SerializeField] [Range(0.01f, 5f)] float _invincibleTime = 5f;
    [SerializeField] HealthPickup _healthPickupBlueprint;
    [SerializeField][Range(0.01f, 20f)] float _regenerateTime = 5f;
    [SerializeField] Transform _pickupParent;

    [Header("VFX")]
    [SerializeField] VisualEffect _energyLoss;
    [SerializeField] Texture2D _energyLossRed;

    bool _vfxActive = false;

    public SpriteRenderer[] SpriteRenderers => _spriteRenderers;
    public SpiderSpriteIterator SpriteIterator => _spriteIterator;
    public int MoveSign { get; private set; } = 0;
    public float InvinvibilityTime => _invincibleTime;
    public float OverheatT { get; set; } = 0;
    public float RegenerateT { get; set; } = 0;
    public bool IsVulnerable { get; set; } = false;
    public bool IsInvincible { get; set; } = false;
    public bool IsNotHurtingOnTouch => IsVulnerable || IsInvincible;
    public bool IsShieldCritical => OverheatT > 0.8f;
    public SpiderAttackStateMetric SpiderAttack { get; set; } = SpiderAttackStateMetric.NONE;
    public float RegenerateTime => _regenerateTime;
    public float InvincibleTime => _invincibleTime;
    public SpiderLaser SpiderLaser { get; set; }
    public Rigidbody2D Rigidbody { get; set; }
    public Vector2 GoalRotation { get; set; } = Vector2.up;
    public float SpiderBodyOrbit { get; set; }



    //--- Private Fields ------------------------

    
    float _orbitRotationT = 0f;
    bool _mustReachThresholdForMovement = false;
    Tween _hasJustBeenHitTween;
    
    MineSpawner _mineSpawner;
    DrillianController _drillianController;

    //--- Unity Methods ------------------------

    public void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        SpiderLaser = GetComponent<SpiderLaser>();

        SpiderBodyOrbit = ((Vector2)transform.position).magnitude;

        // VFX
        _energyLoss.SetTexture("Main Texture", _energyLossRed);
    }

    public void Start()
    {
        _mineSpawner = FindObjectOfType<MineSpawner>();
        _drillianController = FindObjectOfType<DrillianController>();

        ResetOrbitTToGoalRotation();
    }

    public void FixedUpdate()
    {
        EvaluateOverheat();
        VulnerableVFX();
    }


    //--- Private Methods ------------------------


    public void ApplyGravityToVelocity()
    {
        float gravity = 2f;
        Rigidbody.velocity += -(Vector2)transform.position * gravity * Time.deltaTime;
    }
    
    public void SetVelocity()
    {
        Vector2 moveDirection = Vector3.RotateTowards(Rigidbody.velocity.normalized, GoalRotation, _maxRotationControl * Time.deltaTime, float.PositiveInfinity);
        Rigidbody.velocity = moveDirection * _digSpeed;
    }

    public void UpdateDigRotation()
    {
        float currentAngle = Vector2.SignedAngle(Vector2.up, GoalRotation);
        float goalAngle = Vector2.SignedAngle(Vector2.up, (_drillianController.transform.position - transform.position).normalized);
        
        float angleDecay = 0.6f;
        
        float angleDiff = NormalizeAngle(currentAngle - goalAngle);
        float lerpedAngle = NormalizeAngle(goalAngle + angleDiff * Mathf.Exp(-angleDecay * Time.deltaTime));
        
        GoalRotation = Quaternion.Euler(0,0, lerpedAngle) * Vector2.up;
    }



    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }

    public void EndFly()
    {
        Rigidbody.velocity = Vector3.zero;
        
        ResetGoalRotation(); 
        ResetOrbitTToGoalRotation();
        
        SetSpiderPosition();
        SetSpiderRotation();
    }
    
    public void EndDig()
    {
        ThrowMines();
    }

    void ThrowMines()
    {
        _mineSpawner.SpawnMines(transform.position.normalized * Utilities.InnerOrbit, 0/*Vector2.SignedAngle(Vector2.up, Rigidbody.velocity.normalized)*/, 2, 0.1f);
    }

    void EvaluateOverheat()
    {
        if (Mathf.Approximately(OverheatT, 1))
        {
            if (!IsVulnerable)
            {
                _spiderManager.SpiderStateManager.SetState(new SpiderVulnerableState(_spiderManager));
            }
        }
        else
        {
            OverheatT = Mathf.Clamp01(OverheatT - _overheatLoss * Time.deltaTime);
        }
    }
    

    public void SetMovementGoalRotation(float innerAngle, float outerAngle)
    {
        do
        {
            GoalRotation = Random.insideUnitCircle.normalized;
        }
        while (Vector2.Angle(transform.position.normalized, GoalRotation) < innerAngle || Vector2.Angle(transform.position.normalized, GoalRotation) > outerAngle);
    }

    public bool ArrivedAtGoalRotation()
    {
        return Vector2.Dot(GoalRotation, transform.position.normalized) >= 0.99f;
    }

    public IEnumerator WaitUntilArrivedAtGoalRotation()
    {
        while (!ArrivedAtGoalRotation()) yield return new WaitForEndOfFrame();
    }

    public void CalculateOrbitRotation()
    {
        if (GoalRotation.magnitude < 0.1f) return;

        if (IsVulnerable) return;

        Vector2 currentDirection = transform.position.normalized;

        float angle = Vector2.Angle(currentDirection, GoalRotation);

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
            MoveSign = -(int)Mathf.Sign(GoalRotation.x * currentDirection.y - GoalRotation.y * currentDirection.x);
        }
        
        // increase
        _orbitRotationT += MoveSign * _rotationSpeed * Time.deltaTime;

        // guard
        if (_orbitRotationT >= 1)
        {
            _orbitRotationT -= 1;
        }
    }

    public void ResetGoalRotation()
    {
        GoalRotation = transform.position.normalized;
    }
    
    public void ResetOrbitTToGoalRotation()
    {
        _orbitRotationT = Vector2.SignedAngle(Vector2.up, GoalRotation).Remap(-180, 180, 0, 1);
    }

    public void SetSpiderPosition()
    {
        if (IsVulnerable) return;
        float goalAngle = _orbitRotationT.Remap(0, 1, -180, 180);

        Vector2 rotatedGoalVector = Quaternion.Euler(0f, 0f, goalAngle) * Vector2.up;
        Vector2 goalPosition = rotatedGoalVector * SpiderBodyOrbit;

        // Smooth Movement
        Vector2 currentVector = transform.position.normalized;
        Vector2 currentPosition = currentVector * SpiderBodyOrbit;
        
        Rigidbody.MovePosition(UpdateDirection(currentPosition, goalPosition));
    }
    
    Vector2 UpdateDirection(Vector2 direction, Vector2 goalDirection)
    {
        return goalDirection + (direction - goalDirection) * Mathf.Exp(-_rotationAdjustmentDecay * Time.deltaTime);
    }

    public void SetSpiderRotation()
    {
        if (IsVulnerable) return;

        Rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.forward, transform.position));
    }

    void IncreaseHeat()
    {
        OverheatT = Mathf.Clamp01(OverheatT + _overheatGain * Time.deltaTime);
        _spriteIterator.ShieldHit(0.1f);
        Rumble.instance?.RumbleLuna(0, 0.5f, Time.fixedDeltaTime);
    }

    public void SpawnHP()
    {
        Instantiate(_healthPickupBlueprint, transform.position, Quaternion.LookRotation(Vector3.forward, transform.position.normalized),_pickupParent);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_lunaLaser == (_lunaLaser | (1 << collision.gameObject.layer)) && !IsInvincible)
        {
            IncreaseHeat();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_drillian == (_drillian | (1 << collision.gameObject.layer)) && IsVulnerable)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderDamagedState(_spiderManager));
        }
    }

    private void VulnerableVFX()
    {
        if (IsVulnerable && !_vfxActive)
        {
            _energyLoss.SendEvent("Discharge");
            _energyLoss.SetBool("Alive", true);
            _vfxActive = !_vfxActive;

            AudioController.Fire(new SpiderVulnurable(SpiderVulnurable.VulnurableState.SpiderVulnurable));
        }
        else if (!IsVulnerable && _vfxActive)
        {
            _energyLoss.Stop();
            _energyLoss.SetBool("Alive", false);
            _vfxActive = !_vfxActive;

            AudioController.Fire(new SpiderVulnurable(SpiderVulnurable.VulnurableState.SpiderInvulnurable));
        }
    }

    public string GetSpiderAttackString()
    {
        return SpiderAttack switch
        {
            SpiderAttackStateMetric.NONE => "none",
            SpiderAttackStateMetric.CHARGING => "charging",
            SpiderAttackStateMetric.ATTACK => "attack",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GoalRotation * SpiderBodyOrbit, 0.25f);
        Gizmos.color = Color.blue;
        float angle = Utilities.Remap(_orbitRotationT, 0, 1, -180, 180);
        Gizmos.DrawSphere(Quaternion.Euler(0, 0, angle)* Vector2.up * Utilities.InnerOrbit * 0.75f, 0.25f);
    }
}
