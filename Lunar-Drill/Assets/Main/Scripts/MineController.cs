using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class MineController : MonoBehaviour
{
    #region --- Exposed Fields ---
    [Header("Layers")]
    [SerializeField] LayerMask _destroyLayer; // Luna laser
    [SerializeField] LayerMask _mineLayer; // Luna laser

    [Header("Visuals")]
    [SerializeField] [Range(0.05f, 0.3f)] float _insideOffset = 0.2f;
    [SerializeField] SpriteRenderer _mineVisuals;
    [SerializeField] Sprite _mineLilac, _mineRed;
    [SerializeField] [Range(1, 20)] float _blinkSpeed;
    [SerializeField] VisualEffect _explosion;

    [Header("Movement")]
    [SerializeField] [Range(0f, 10000f)] float _gravityStrength = 1f;
    [SerializeField] [Range(0, 100f)] float _startImpulseScale = 0.5f;
    [SerializeField] [Range(0, 20f)] float _angularVelocity = 5;
    #endregion

    #region --- Private Fields ---
    Rigidbody2D _rigidbody;
    bool _applyGravity = true;
    bool _currentSpriteLilac = true;
    #endregion

    #region --- Public Fields ---
    public bool Active { get; set; } = false;
    public Tween MoveTween { get; set; }
    public Vector2 UnscaledStartForce { get; set; }
    #endregion

    #region --- Unity Methods ---
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        float size = transform.localScale.x;
        transform.localScale = Vector3.zero;
        MoveTween = transform.DOScale(size, 0.33f).SetEase(Ease.OutBack).OnComplete(() => Active = true);

        transform.Rotate(Vector3.forward, Random.Range(0, 360));
    }

    private void Start()
    {
        _rigidbody.AddForce(UnscaledStartForce * _startImpulseScale, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        if (!_applyGravity)
        {
            // Start blinking once touching ground
            if (Mathf.Round(Utilities.Remap(Mathf.Sin(Time.time * _blinkSpeed), -1, 1, 0, 1)) == 0 && !_currentSpriteLilac)
            {
                _mineVisuals.sprite = _mineLilac;
                _currentSpriteLilac = true;
            }
            else
            {
                _mineVisuals.sprite = _mineRed;
                _currentSpriteLilac = false;
            }

            return;
        }


        if (transform.position.magnitude < Utilities.InnerOrbit - _insideOffset)
        {
            _applyGravity = false;
            _rigidbody.velocity = Vector2.zero;
            return;
        }

        Vector2 gravityDirection = -(Vector2)transform.position + MapPointOntoPlanetSurface(transform.position);
        _rigidbody.velocity += gravityDirection * _gravityStrength * Time.fixedDeltaTime;
        transform.transform.Rotate(Vector3.forward, _angularVelocity);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO Should exploding mines have a damage zone affecting every character?
        // TODO Does something happen if spider touches mines?
        // TODO Should Luna/Drillian also make the mine explode while they hit it but are invincible? (hits with them are handled in respective controllers)

        if (Active) // Make sure collisions are just checked while mine is active
        {
            if (_destroyLayer == (_destroyLayer | 1 << collision.gameObject.layer))
            {
                DestroyMine();
            }
            else if (_mineLayer == (_mineLayer | 1 << collision.gameObject.layer))
            {
                if (collision.gameObject.GetComponent<MineController>())
                {
                    MineController otherMine = collision.gameObject.GetComponent<MineController>();
                    if (otherMine.Active)
                        DestroyMine();
                }

            }
        }
    }
    #endregion

    #region --- Private Methods ---

    /*
     * Maps any given point onto the surface of the planet.
     */
    Vector2 MapPointOntoPlanetSurface(Vector2 point)
    {
        return point - (point.magnitude - Utilities.InnerOrbit) * point.normalized;
    }
    #endregion

    #region --- Public Methods ---
    public void DestroyMine()
    {
        Active = false;

        // Removing mine from spawner
        MineSpawner spawner = transform.parent.GetComponent<MineSpawner>();
        if (spawner)
            spawner.RemoveMine(this);

        _explosion.Play();
        MoveTween.Kill();
        MoveTween = transform.DOScale(0, .5f).SetEase(Ease.InExpo);
        DOVirtual.DelayedCall(0.3f, () => _mineVisuals.enabled = false);

        Destroy(gameObject, 2f);
    }

    #endregion
}
