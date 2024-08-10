using DG.Tweening;
using UnityEngine;

public class MineController : MonoBehaviour
{
    #region --- Exposed Fields ---
    [Header("Layers")]
    [SerializeField] LayerMask _destroyLayer; // Luna laser

    [Header("Visuals")]
    [SerializeField] SpriteRenderer _mineVisuals;

    [Header("Movement")]
    [SerializeField] [Range(0f, 10000f)] float _gravityStrength = 1f;
    [SerializeField] [Range(0.1f, 2)] float _inAirDuration = 1.5f;
    [SerializeField] [Range(0, 100f)] float _startImpulseScale = 0.5f;
    #endregion

    #region --- Private Fields ---
    Rigidbody2D _rigidbody;
    bool _applyGravity = true;
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
    }

    private void Start()
    {
        _rigidbody.AddForce(UnscaledStartForce * _startImpulseScale, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        if (!_applyGravity)
            return;

        if (transform.position.magnitude < Utilities.InnerOrbit - 0.1f)
        {
            _applyGravity = false;
            _rigidbody.velocity = Vector2.zero;
            return;
        }

        Vector2 gravityDirection = -(Vector2)transform.position + MapPointOntoPlanetSurface(transform.position);
        _rigidbody.velocity += gravityDirection * _gravityStrength * Time.fixedDeltaTime;
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
                // TODO: Rumble?
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

        MoveTween.Kill();
        MoveTween = transform.DOScale(0, .5f).SetEase(Ease.InBack);

        Destroy(gameObject, 5f);
    }

    #endregion
}
