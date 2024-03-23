using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpiderController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] [Range(0.01f, 1f)] float _rotationSpeed = 5f;

    [Header("Movement Smoothing")]
    [SerializeField] [Range(0.1f, 100f)] float _movementStartAngleThreshold;
    [SerializeField] [Range(0.1f, 10f)] float _movementArrivedAngleThreshold;
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

    [Header("VFX")]
    [SerializeField] VisualEffect _energyLoss;
    [SerializeField] Texture2D _energyLossRed;

    public enum SpiderState { Level1, Level2, Level3, Level4 };

    bool _vfxActive = false;

    public int MoveSign { get; private set; } = 0;
    public float InvinvibilityTime => _invincibleTime;
    public float OverheatT { get; set; } = 0;
    public float RegenerateT { get; set; } = 0;
    public bool IsVulnerable { get; set; } = false;
    public bool IsInvincible { get; set; } = false;
    public bool IsNotHurtingOnTouch => IsVulnerable || IsInvincible;


    //--- Private Fields ------------------------

    Rigidbody2D _rigidbody;
    float _orbitRotationT = 0f;
    SpiderLaser _spiderLaser;
    bool _mustReachThresholdForMovement = false;
    Vector2 _goalRotation = Vector2.zero;
    Tween _regenerateVulnerableTween;


    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spiderLaser = GetComponent<SpiderLaser>();

        // VFX
        _energyLoss.SetTexture("Main Texture", _energyLossRed);
    }

    public void Start()
    {
        StartCoroutine(MoveLoop());
    }

    public void FixedUpdate()
    {
        CalculateOrbitRotation();

        EvaluateOverheat();

        SetSpiderPosition();
        SetSpiderRotation();

        VulnerableVFX();
    }


    //--- Private Methods ------------------------


    IEnumerator MoveLoop()
    {
        while (true)
        {
            yield return null;
            if (IsVulnerable) yield return null;
            yield return LaserMovement();
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

                Rumble.main?.AddRumble(ChosenCharacter.luna, new Vector2(0f, 0.1f));
                Rumble.main?.AddRumble(ChosenCharacter.drillian, new Vector2(0f, 0.1f));

                // stop current move??
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

        return Movement();
    }

    IEnumerator GetLevel1Ability()
    {
        // Stand: 25%
        // Movement: 75%

        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.75f)
        {
            return Wait();
        }
        else
        {
            return Movement();
        }
    }

    IEnumerator GetLevel2Ability()
    {
        // Stand: 20% Chance
        // Movement: 30% Chance
        // RandomLaser: 35% Chance
        // LunaLaser: 15% Chance

        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.85f)
        {
            return Wait();
        }
        else if (randomT > 0.5f)
        {
            return Movement();
        }
        else if (randomT > 0.15f)
        {
            return RandomLaser();
        }
        else
        {
            return LunaLaser();
        }
    }

    IEnumerator GetLevel3Ability()
    {
        // Stand: 15% Chance
        // Movement: 25% Chance
        // RandomLaser: 30% Chance
        // LunaLaser: 20% Chance
        // BlinkLaser: 10% Chance

        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.85f)
        {
            return Wait();
        }
        else if (randomT > 0.6f)
        {
            return Movement();
        }
        else if (randomT > 0.3f)
        {
            return RandomLaser();
        }
        else if (randomT > 0.1f)
        {
            return LunaLaser();
        }
        else
        {
            return BlinkLaser();
        }
    }


    IEnumerator GetLevel4Ability()
    {
        // Stand: 05% Chance
        // Movement: 20% Chance
        // RandomLaser: 25% Chance
        // LunaLaser: 25% Chance
        // BlinkLaser: 25% Chance

        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.95f)
        {
            return Wait();
        }
        else if (randomT > 0.75f)
        {
            return Movement();
        }
        else if (randomT > 0.5f)
        {
            return RandomLaser();
        }
        else if (randomT > 0.25f)
        {
            return LunaLaser();
        }
        else
        {
            return BlinkLaser();
        }
    }

    IEnumerator LaserMovement()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

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

        // the spiders brains

        // Each Move should be seperate, no interplay between moves
        // moves should be randomized with changing percentage-chances (weights)

        // Also its based on spiders HP
        // 4 Spider HP total
        // First HP is tutorial -> Only Movement
        // Second HP is opposite-luna lasers + randomized lasers
        // Third HP starts with laser-blink once, and afterwards is like second HP
        // Fourth HP is everything-everywhere-all-at-once (with a high chance of blink)

        yield return GetNextAbility(spiderState);


        yield return new WaitForEndOfFrame();
    }

    IEnumerator ShootRandom()
    {
        if (IsVulnerable) yield break;

        _goalRotation = Random.insideUnitCircle.normalized;

        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        StartCoroutine(_spiderLaser.ShootLaser());

        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        _goalRotation = Random.insideUnitCircle.normalized;

        yield return new WaitForSeconds(5f);
    }

    IEnumerator ShootLuna()
    {
        GoalMoveOppositeOfLuna();

        // Hier brächte ich logik die checkt wann ich angekommen bin
        yield return new WaitForSeconds(Random.Range(2f, 4f));

        StartCoroutine(_spiderLaser.ShootLaser());

        yield return new WaitForSeconds(0.5f);

        // check ob luna links oder rechts ist
        GoalMoveOpposite(LunaIsClockwise());

        yield return new WaitForSeconds(Random.Range(2f, 4f));
    }

    IEnumerator Wait()
    {
        float duration = Random.Range(1.5f, 3f);

        float startTime = Time.time;

        while (Time.time - startTime < duration || IsVulnerable)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Movement()
    {
        _goalRotation = Random.insideUnitCircle.normalized;

        while (Vector2.Angle(transform.position.normalized, _goalRotation) < 45 || Vector2.Angle(transform.position.normalized, _goalRotation) > 135)
        {
            _goalRotation = Random.insideUnitCircle.normalized;
        }

        while (Vector2.Dot(_goalRotation, transform.position.normalized) < 0.95f && !IsVulnerable)
        {
            Debug.Log(Vector2.Dot(_goalRotation, transform.position.normalized));
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator RandomLaser()
    {
        yield return null;
    }

    IEnumerator LunaLaser()
    {
        yield return null;
    }

    IEnumerator BlinkLaser()
    {
        yield return null;
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

    void GoalMoveOpposite(bool clockwise)
    {
        float angle = clockwise ? 181 : 179;
        _goalRotation = Quaternion.Euler(0, 0, angle) * transform.position.normalized;
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

        // increase
        _orbitRotationT += MoveSign * _rotationSpeed * Time.deltaTime;

        // guard
        if (_orbitRotationT >= 1)
        {
            _orbitRotationT -= 1;
        }
    }

    void SetSpiderPosition()
    {
        if (IsVulnerable) return;

        float angle = _orbitRotationT.Remap(0, 1, 0, 360);

        Vector2 rotatedVector = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        Vector2 position = rotatedVector * Utilities.InnerOrbit;

        _rigidbody.MovePosition(position);
    }

    void SetSpiderRotation()
    {
        if (IsVulnerable) return;

        _rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.forward, transform.position));
    }

    void IncreaseHeat()
    {
        OverheatT = Mathf.Clamp01(OverheatT + _overheatGain * Time.deltaTime);
        _spriteIterator.ShieldHit(0.5f);
    }

    void GetDamaged()
    {
        if (_regenerateVulnerableTween != null && _regenerateVulnerableTween.IsActive())
        {
            _regenerateVulnerableTween.Kill();
        }

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

        foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.DOColor(Color.clear, _invincibleTime).SetEase(Ease.Flash, 48, 0.75f);
        }

        Rumble.main?.AddRumble(ChosenCharacter.luna, new Vector2(0.7f, 0.8f), 0.2f);
        Rumble.main?.AddRumble(ChosenCharacter.drillian, new Vector2(0.7f, 0.8f), 0.2f);

        Rumble.main?.RemovePermanentRumble(ChosenCharacter.luna, new Vector2(0f, 0.1f));
        Rumble.main?.RemovePermanentRumble(ChosenCharacter.drillian, new Vector2(0f, 0.1f));
    }

    void SpawnHP()
    {
        Instantiate(_healthPickupBlueprint, transform.position, Quaternion.LookRotation(Vector3.forward, transform.position.normalized));
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
            _vfxActive = !_vfxActive;
        }
        else if (!IsVulnerable && _vfxActive)
        {
            _energyLoss.Stop();
            _vfxActive = !_vfxActive;
        }
    }

}
