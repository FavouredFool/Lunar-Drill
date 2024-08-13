

using System;
using DG.Tweening;
using System.Collections;
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



    //--- Private Fields ------------------------

    
    float _orbitRotationT = 0f;
    bool _mustReachThresholdForMovement = false;
    Vector2 _goalRotation = Vector2.up;
    Tween _hasJustBeenHitTween;
    bool _isDigging = false;

    float _spiderBodyOrbit;

    MineSpawner _mineSpawner;
    DrillianController _drillianController;

    //--- Unity Methods ------------------------

    public void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        SpiderLaser = GetComponent<SpiderLaser>();

        _spiderBodyOrbit = ((Vector2)transform.position).magnitude;

        // VFX
        _energyLoss.SetTexture("Main Texture", _energyLossRed);
    }

    public void Start()
    {
        _mineSpawner = FindObjectOfType<MineSpawner>();
        _drillianController = FindObjectOfType<DrillianController>();
    }

    public void FixedUpdate()
    {
        if (!_isDigging)
        {
            EvaluateOverheat();
        }
        
        VulnerableVFX();
    }


    //--- Private Methods ------------------------


    public void SetVelocity()
    {
        Vector2 moveDirection = Vector3.RotateTowards(Rigidbody.velocity.normalized, _goalRotation, _maxRotationControl * Time.deltaTime, float.PositiveInfinity);
        Rigidbody.velocity = moveDirection * _digSpeed;
    }

    void UpdateDigRotation()
    {
        float currentAngle = Vector2.SignedAngle(Vector2.up, _goalRotation);
        float goalAngle = Vector2.SignedAngle(Vector2.up, (_drillianController.transform.position - transform.position).normalized);
        
        float angleDecay = 0.6f;
        
        float angleDiff = NormalizeAngle(currentAngle - goalAngle);
        float lerpedAngle = NormalizeAngle(goalAngle + angleDiff * Mathf.Exp(-angleDecay * Time.deltaTime));
        
        _goalRotation = Quaternion.Euler(0,0, lerpedAngle) * Vector2.up;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_goalRotation * _spiderBodyOrbit, 0.25f);
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere((_drillianController.transform.position - transform.position).normalized * _spiderBodyOrbit, 0.25f);
    }

    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
    
    public void EndDig()
    {
        _isDigging = false;
        ThrowMines();
        Rigidbody.velocity = Vector3.zero;
        ResetOrbitTToGoalRotation();
    }

    void ThrowMines()
    {
        _mineSpawner.SpawnMines(transform.position.normalized * Utilities.InnerOrbit, Vector2.SignedAngle(Vector2.up, transform.up), 2, 0.1f);
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
            _goalRotation = Random.insideUnitCircle.normalized;
        }
        while (Vector2.Angle(transform.position.normalized, _goalRotation) < innerAngle || Vector2.Angle(transform.position.normalized, _goalRotation) > outerAngle);
    }

    public bool ArrivedAtGoalRotation()
    {
        return Vector2.Dot(_goalRotation, transform.position.normalized) >= 0.99f;
    }

    public IEnumerator WaitUntilArrivedAtGoalRotation()
    {
        while (!ArrivedAtGoalRotation()) yield return new WaitForEndOfFrame();
    }

    IEnumerator Digging()
    {
        _isDigging = true;
        _goalRotation = -_goalRotation;
        SetVelocity();
        
        while (_isDigging)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    //IEnumerator LunaLaser()
    //{
    //    GoalMoveOppositeOfLuna();
    //
    //    // increase speed drastically
    //    yield return MoveToPosition();
    //
    //    StartCoroutine(_spiderLaser.ShootLaser());
    //    
    //    // Verfolge Luna
    //    GoalMoveOpposite(LunaIsClockwise(), 179);
    //
    //    yield return MoveToPosition();
    //
    //    // Verfolge Luna weiter
    //    GoalMoveOpposite(LunaIsClockwise(), Random.Range(60, 120));
    //
    //    yield return MoveToPosition();
    //
    //    _spiderLaser.StopLaser();
    //
    //    yield return new WaitForSeconds(Random.Range(2f, 3.5f));
    //}

    //void GoalMoveOppositeOfLuna()
    //{
    //    LunaController lunaController = FindObjectOfType<LunaController>();
    //
    //    if (lunaController == null) throw new System.Exception();
    //
    //    // go on opposite side
    //    _goalRotation = -lunaController.transform.position.normalized;
    //}
    //
    //bool LunaIsClockwise()
    //{
    //    LunaController lunaController = FindObjectOfType<LunaController>();
    //
    //    if (lunaController == null) throw new System.Exception();
    //
    //    return Vector2.SignedAngle(transform.position.normalized, lunaController.transform.position.normalized) >= 0;
    //}

    void GoalMoveOpposite(bool clockwise, float angle)
    {
        float angleToMoveTo = clockwise ? angle : -angle;
        _goalRotation = Quaternion.Euler(0, 0, angleToMoveTo) * transform.position.normalized;
    }
    


    public void CalculateOrbitRotation()
    {
        if (_goalRotation.magnitude < 0.1f) return;

        if (IsVulnerable) return;

        Vector2 currentDirection = transform.position.normalized;

        float angle = Vector2.Angle(currentDirection, _goalRotation);

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
            MoveSign = -(int)Mathf.Sign(_goalRotation.x * currentDirection.y - _goalRotation.y * currentDirection.x);
        }
        
        // increase
        _orbitRotationT += MoveSign * _rotationSpeed * Time.deltaTime;

        // guard
        if (_orbitRotationT >= 1)
        {
            _orbitRotationT -= 1;
        }
    }

    void ResetOrbitTToGoalRotation()
    {
        _orbitRotationT = Vector2.SignedAngle(Vector2.up, _goalRotation).Remap(-180, 180, 0, 1);
    }

    public void SetSpiderPosition()
    {
        if (IsVulnerable) return;
        float goalAngle = _orbitRotationT.Remap(0, 1, -180, 180);

        Vector2 rotatedGoalVector = Quaternion.Euler(0f, 0f, goalAngle) * Vector2.up;
        Vector2 goalPosition = rotatedGoalVector * _spiderBodyOrbit;

        // Smooth Movement
        Vector2 currentVector = transform.position.normalized;
        Vector2 currentPosition = currentVector * _spiderBodyOrbit;
        
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

}
