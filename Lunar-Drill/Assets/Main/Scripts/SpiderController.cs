

using System;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class SpiderController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Speed")]
    [SerializeField] [Range(0.01f, 10f)] float _slowRotationSpeed = 0.5f;
    [SerializeField] [Range(0.01f, 10f)] float _midRotationSpeed = 1f;
    [SerializeField] [Range(0.01f, 10f)] float _fastRotationSpeed = 5f;

    [Header("Movement Smoothing")]
    [SerializeField] [Range(0.1f, 100f)] float _movementStartAngleThreshold;
    [SerializeField] [Range(0.1f, 10f)] float _movementArrivedAngleThreshold;
    [SerializeField] [Range(1, 25)] int _rotationAdjustmentDecay = 16;
    [SerializeField] LayerMask _lunaLaser;
    [SerializeField] LayerMask _drillian;
    

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


    public enum SpiderState { Level1, Level2, Level3, Level4 };
    public enum SpiderSpeed { SLOW, MID, FAST };

    bool _vfxActive = false;

    public int MoveSign { get; private set; } = 0;
    public float InvinvibilityTime => _invincibleTime;
    public float OverheatT { get; set; } = 0;
    public float RegenerateT { get; set; } = 0;
    public bool IsVulnerable { get; set; } = false;
    public bool IsInvincible { get; set; } = false;
    public bool IsNotHurtingOnTouch => IsVulnerable || IsInvincible;
    public bool IsShieldCritical => OverheatT > 0.8f;
    public SpiderAttackStateMetric SpiderAttack { get; set; } = SpiderAttackStateMetric.NONE;



    //--- Private Fields ------------------------

    Rigidbody2D _rigidbody;
    float _orbitRotationT = 0f;
    SpiderLaser _spiderLaser;
    bool _mustReachThresholdForMovement = false;
    Vector2 _goalRotation = Vector2.up;
    Tween _regenerateVulnerableTween;
    SpiderSpeed _spiderSpeed = SpiderSpeed.MID;
    Tween _hasJustBeenHitTween;
    bool _hasJustBeenHit = false;
    bool _isDigging = false;

    float _spiderBodyOrbit;


    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spiderLaser = GetComponent<SpiderLaser>();

        _spiderBodyOrbit = ((Vector2)transform.position).magnitude;

        // VFX
        _energyLoss.SetTexture("Main Texture", _energyLossRed);
    }

    public void Start()
    {
        StartCoroutine(MoveLoop());
    }

    public void FixedUpdate()
    {
        if (_isDigging)
        {
            //Debug.Log(_goalRotation);
            // needs to do "_isDigging = false" at some point
            _rigidbody.MovePosition(Vector2.MoveTowards(transform.position, _goalRotation * _spiderBodyOrbit, _midRotationSpeed));

            if (Vector2.Distance(transform.position, _goalRotation * _spiderBodyOrbit) < 0.01f)
            {
                _isDigging = false;
            }
        }
        else
        {
            CalculateOrbitRotation();

            EvaluateOverheat();

            SetSpiderPosition();
            SetSpiderRotation();

            VulnerableVFX();
        }
    }


    //--- Private Methods ------------------------


    IEnumerator MoveLoop()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        
        while (true)
        {
            yield return null;
            if (IsVulnerable) yield return null;
            yield return SpiderBrains(gameManager);
        }
    }

    void EvaluateOverheat()
    {
        if (Mathf.Approximately(OverheatT, 1))
        {
            if (!IsVulnerable)
            {
                IsVulnerable = true;

                _spriteIterator.Stun(float.MaxValue); //Change this to overheat time

                RegenerateT = 0;
                _regenerateVulnerableTween = DOTween.To(() => RegenerateT, e => RegenerateT = e, 1, _regenerateTime);
                _regenerateVulnerableTween.OnComplete(() =>
                {
                    OverheatT = 0;
                    IsVulnerable = false;
                    _spriteIterator.CancelStun();
                });

                _spiderLaser.StopLaser();
            }
        }
        else
        {
            OverheatT = Mathf.Clamp01(OverheatT - _overheatLoss * Time.deltaTime);
        }
    }

    IEnumerator GetNextAbility(SpiderState spiderState)
    {
        switch (spiderState)
        {
            case SpiderState.Level1:
                return GetLevel1Ability();
            case SpiderState.Level2:
                return GetLevel2Ability();
            case SpiderState.Level3:
                return GetLevel3Ability();
            case SpiderState.Level4:
                return GetLevel4Ability();
        }

        return Movement(80, 170);
    }

    IEnumerator GetLevel1Ability()
    {
        // Stand: 20%
        // Movement: 80%

        Debug.Log("ONLY ONCE");
        
        float randomT = Random.Range(0f, 1f);
        
        if (randomT > 0f)
        {
            return Digging();
        }
        if (randomT > 0.8f)
        {
            return Wait();
        }
        else
        {
            return Movement(80, 170);
        }
    }

    IEnumerator GetLevel2Ability()
    {
        // Stand: 20% Chance
        // Movement: 40% Chance
        // RandomLaserShort: 30% Chance
        // LunaLaser: 10% Chance

        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.8f)
        {
            return Wait();
        }
        else if (randomT > 0.4f)
        {
            return Movement(80, 170);
        }
        else if (randomT > 0.1f)
        {
            return RandomLaserShort();
        }
        else
        {
            return LunaLaser();
        }
    }

    IEnumerator GetLevel3Ability()
    {
        // Wait: 10% Chance
        // Movement: 40% Chance
        // RandomLaserShort: 25% Chance
        // RandomLaserLong: 0% Chance
        // Digging: 25%
        // LunaLaser: 0% Chance

        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.9f)
        {
            return Wait();
        }
        else if (randomT > 0.5f)
        {
            return Movement(80, 170);
        }
        else if (randomT > 0.25f)
        {
            return RandomLaserShort();
        }
        else
        {
            return Digging();
        }
    }


    IEnumerator GetLevel4Ability()
    {
        // Stand: 05% Chance
        // Movement: 25% Chance
        // RandomLaserShort: 35% Chance
        // RandomLaserLong: 0% Chance
        // LunaLaser: 0% Chance
        // Digging: 35% Chance

        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.95f)
        {
            return Wait();
        }
        else if (randomT > 0.7f)
        {
            return Movement(80, 170);
        }
        else if (randomT > 0.35f)
        {
            return RandomLaserShort();
        }
        else
        {
            return Digging();
        }
    }

    IEnumerator SpiderBrains(GameManager gameManager)
    {
        SpiderState spiderState;

        int spiderHP = gameManager.SpiderHP;

        if (spiderHP == gameManager.SpiderMaxHP)
        {
            spiderState = SpiderState.Level1;
        }
        else if (spiderHP == gameManager.SpiderMaxHP - 1)
        {
            spiderState = SpiderState.Level2;
        }
        else if (spiderHP == gameManager.SpiderHP - 2)
        {
            spiderState = SpiderState.Level3;
        }
        else
        {
            spiderState = SpiderState.Level4;
        }

        yield return GetNextAbility(spiderState);

        yield return new WaitForEndOfFrame();
    }

    IEnumerator Wait()
    {
        if (_hasJustBeenHit) yield break;

        float duration = Random.Range(1.5f, 3.5f);

        float startTime = Time.time;

        while (Time.time - startTime < duration || IsVulnerable)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Movement(float innerAngle, float outerAngle)
    {
        _spiderSpeed = SpiderSpeed.MID;

        _goalRotation = Random.insideUnitCircle.normalized;

        while (Vector2.Angle(transform.position.normalized, _goalRotation) < innerAngle || Vector2.Angle(transform.position.normalized, _goalRotation) > outerAngle)
        {
            _goalRotation = Random.insideUnitCircle.normalized;
        }

        yield return MoveToPosition(SpiderSpeed.MID);
    }

    IEnumerator MoveToPosition(SpiderSpeed speed)
    {

        _spiderSpeed = speed;

        while (Vector2.Dot(_goalRotation, transform.position.normalized) < 0.99f && !IsVulnerable)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator RandomLaserLong()
    {
        if (_hasJustBeenHit) yield break;

        _spiderSpeed = SpiderSpeed.MID;

        _goalRotation = Random.insideUnitCircle.normalized;

        // Start PreLaser

        StartCoroutine(_spiderLaser.ShootLaser());
        yield return Movement(90, 179);
        yield return Movement(35, 60);
        yield return Movement(35, 120);
        yield return Movement(35, 179);
        _spiderLaser.StopLaser();

        yield return new WaitForSeconds(Random.Range(1f, 2.5f));
    }

    IEnumerator RandomLaserShort()
    {
        if (_hasJustBeenHit) yield break;

        _spiderSpeed = SpiderSpeed.MID;

        _goalRotation = Random.insideUnitCircle.normalized;

        // Start PreLaser

        StartCoroutine(_spiderLaser.ShootLaser());
        yield return Movement(120, 179);
        yield return Movement(45, 90);
        _spiderLaser.StopLaser();

        yield return new WaitForSeconds(Random.Range(1f, 2.5f));
    }

    IEnumerator Digging()
    {
        Debug.Log("digging");
        _isDigging = true;
        _goalRotation = -_goalRotation;
        
        while (_isDigging)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator LunaLaser()
    {
        if (_hasJustBeenHit) yield break;

        _spiderSpeed = SpiderSpeed.MID;

        GoalMoveOppositeOfLuna();

        // increase speed drastically
        yield return MoveToPosition(SpiderSpeed.MID);

        StartCoroutine(_spiderLaser.ShootLaser());
        
        // Verfolge Luna
        GoalMoveOpposite(LunaIsClockwise(), 179);

        yield return MoveToPosition(SpiderSpeed.MID);

        // Verfolge Luna weiter
        GoalMoveOpposite(LunaIsClockwise(), Random.Range(60, 120));

        yield return MoveToPosition(SpiderSpeed.MID);

        _spiderLaser.StopLaser();

        yield return new WaitForSeconds(Random.Range(2f, 3.5f));
    }

    void GoalMoveOppositeOfLuna()
    {
        LunaController lunaController = FindObjectOfType<LunaController>();

        if (lunaController == null) throw new System.Exception();

        // go on opposite side
        _goalRotation = -lunaController.transform.position.normalized;
    }

    bool LunaIsClockwise()
    {
        LunaController lunaController = FindObjectOfType<LunaController>();

        if (lunaController == null) throw new System.Exception();

        return Vector2.SignedAngle(transform.position.normalized, lunaController.transform.position.normalized) >= 0;
    }

    void GoalMoveOpposite(bool clockwise, float angle)
    {
        float angleToMoveTo = clockwise ? angle : -angle;
        _goalRotation = Quaternion.Euler(0, 0, angleToMoveTo) * transform.position.normalized;
    }


    void CalculateOrbitRotation()
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

        float rotationSpeed;

        switch (_spiderSpeed)
        {
            case SpiderSpeed.SLOW:
                rotationSpeed = _slowRotationSpeed;
                break;
            case SpiderSpeed.FAST:
                rotationSpeed = _fastRotationSpeed;
                break;
            default:
                rotationSpeed = _midRotationSpeed;
                break;
        }

        // increase
        _orbitRotationT += MoveSign * rotationSpeed * Time.deltaTime;

        // guard
        if (_orbitRotationT >= 1)
        {
            _orbitRotationT -= 1;
        }
    }

    void SetSpiderPosition()
    {
        if (IsVulnerable) return;
        float goalAngle = _orbitRotationT.Remap(0, 1, 0, 360);

        Vector2 rotatedGoalVector = Quaternion.Euler(0f, 0f, goalAngle) * Vector2.up;
        Vector2 goalPosition = rotatedGoalVector * Utilities.InnerOrbit;

        // Smooth Movement
        Vector2 currentVector = transform.position.normalized;
        Vector2 currentPosition = currentVector * Utilities.InnerOrbit;
        
        _rigidbody.MovePosition(UpdateDirection(currentPosition, goalPosition));
    }
    
    Vector2 UpdateDirection(Vector2 direction, Vector2 goalDirection)
    {
        return goalDirection + (direction - goalDirection) * Mathf.Exp(-_rotationAdjustmentDecay * Time.deltaTime);
    }

    void SetSpiderRotation()
    {
        if (IsVulnerable) return;

        _rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.forward, transform.position));
    }

    void IncreaseHeat()
    {
        OverheatT = Mathf.Clamp01(OverheatT + _overheatGain * Time.deltaTime);
        _spriteIterator.ShieldHit(0.1f);
        Rumble.instance?.RumbleLuna(0, 0.5f, Time.fixedDeltaTime);
    }

    void GetDamaged()
    {
        if (_regenerateVulnerableTween != null && _regenerateVulnerableTween.IsActive())
        {
            _regenerateVulnerableTween.Kill();
        }

        // Camera shake
        CamShake.Instance.ShakeCamera();

        // Sound
        AudioController.Fire(new SpiderHit(""));

        //Damage
        _spriteIterator.Hit();
        FindObjectOfType<GameManager>().Hit(gameObject, false);

        IsVulnerable = false;
        IsInvincible = true;
        OverheatT = 0;
        SpawnHP();
        DOVirtual.DelayedCall(4, () => IsInvincible = false, false);

        _hasJustBeenHit = true;
        DOVirtual.DelayedCall(2, () => _hasJustBeenHit = false);

        foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.DOColor(Color.clear, _invincibleTime).SetEase(Ease.Flash, 48, 0.75f);
        }

        Rumble.instance?.RumbleBoth(4, 1f, 0.33f);
    }

    void SpawnHP()
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
            GetDamaged();
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
