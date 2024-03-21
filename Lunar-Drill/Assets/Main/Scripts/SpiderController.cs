using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] bool _constantlyRotating = false;
    [SerializeField][Range(0.01f, 1f)] float _rotationSpeed = 5f;

    [Header("Movement Smoothing")]
    [SerializeField][Range(0.1f, 100f)] float _movementStartAngleThreshold;
    [SerializeField][Range(0.1f, 10f)] float _movementArrivedAngleThreshold;
    [SerializeField] LayerMask _lunaLaser;
    [SerializeField] LayerMask _drillian;

    [Header("Overheat")]
    [SerializeField][Range(0.01f, 1)] float _overheatGain = 0.25f;
    [SerializeField][Range(0.01f, 1)] float _overheatLoss = 0.05f;

    [Header("Sprites")]
    [SerializeField] SpriteRenderer[] _spriteRenderers;

    [Header("Hit")]
    [SerializeField][Range(0.01f, 5f)] float _invincibleTime = 5f;

    public int MoveSign { get; private set; } = 0;
    public float OverheatT { get; set; } = 0;
    public bool IsVulnerable { get; set; } = false;
    public bool IsInvincible { get; set; } = false;
    public bool IsNotHurtingOnTouch => IsVulnerable || IsInvincible;


    //--- Private Fields ------------------------

    Rigidbody2D _rigidbody;
    float _orbitRotationT = 0.75f;
    SpiderLaser _spiderLaser;
    bool _mustReachThresholdForMovement = false;
    Vector2 _goalRotation = Vector2.zero;


    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spiderLaser = GetComponent<SpiderLaser>();
    }

    public void Start()
    {
        StartCoroutine(DetermineMoves());
    }

    public void FixedUpdate()
    {
        CalculateOrbitRotation();

        EvaluateOverheat();

        SetSpiderPosition();
        SetSpiderRotation();
    }


    //--- Private Methods ------------------------

    void EvaluateOverheat()
    {
        if (Mathf.Approximately(OverheatT, 1))
        {
            if (!IsVulnerable)
            {
                IsVulnerable = true;

                // stop laser??
                // Change sprite here
            }
        }
        else
        {
            OverheatT = Mathf.Clamp01(OverheatT - _overheatLoss * Time.deltaTime);
        }
    }

    IEnumerator DetermineMoves()
    {
        if (_constantlyRotating)
        {
            StartCoroutine(RotateCircle());
            yield break;
        }

        yield return new WaitForSeconds(2f);

        StartCoroutine(LaserMovement());
    }

    IEnumerator LaserMovement()
    {
        while (true)
        {
            yield return null;

            if (IsVulnerable) continue;

            int random = Random.Range(0, 3);
            if (random == 0)
            {
                yield return ShootLuna();
            }
            else if (random == 1) 
            {
                yield return ShootRandom();
            }
            else
            {
                yield return SimpleMovement();
            }

            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator ShootRandom()
    {
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

    IEnumerator SimpleMovement()
    {
        _goalRotation = Random.insideUnitCircle.normalized;

        yield return new WaitForSeconds(Random.Range(2f, 6f));
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

    IEnumerator RotateCircle()
    {

        while (true)
        {
            yield return null;

            if (IsVulnerable) continue;

            _goalRotation = Quaternion.Euler(0, 0, 45) * Vector2.up;

            yield return new WaitForSeconds(2f);

            _goalRotation = Quaternion.Euler(0, 0, 135) * Vector2.up;

            yield return new WaitForSeconds(2f);

            _goalRotation = Quaternion.Euler(0, 0, 225) * Vector2.up;

            yield return new WaitForSeconds(2f);

            _goalRotation = Quaternion.Euler(0, 0, 315) * Vector2.up;

            yield return new WaitForSeconds(2f);
        }
    }

    void CalculateOrbitRotation()
    {
        if (_goalRotation.magnitude < 0.1f) return;

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
        float angle = _orbitRotationT.Remap(0, 1, 0, 360);

        Vector2 rotatedVector = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        Vector2 position = rotatedVector * Utilities.InnerOrbit;

        _rigidbody.MovePosition(position);
    }

    void SetSpiderRotation()
    {
        _rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.forward, transform.position));
    }

    void IncreaseHeat()
    {
        OverheatT = Mathf.Clamp01(OverheatT + _overheatGain * Time.deltaTime);
    }

    void GetDamaged()
    {
        FindObjectOfType<GameManager>().SpiderHP -= 1;
        IsVulnerable = false;
        IsInvincible = true;
        OverheatT = 0;
        DOVirtual.DelayedCall(4, () => IsInvincible = false, false);

        foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.DOColor(Color.clear, _invincibleTime).SetEase(Ease.Flash, 48, 0.75f);
        }

        
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
}
