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
    DrillianController _drillian;
    LunaController _luna;
    Rigidbody2D _rigidbody;
    bool _applyGravity = true;
    float _timePassed = 0f;
    #endregion

    #region --- Public Fields ---
    public bool Active { get; set; } = false;
    public Tween MoveTween { get; set; }
    public Vector2 SpawnPosition { get; set; }
    public Vector2 MidPosition { get; set; }
    public Vector2 GoalPosition { get; set; }
    #endregion

    #region --- Unity Methods ---
    private void Awake()
    {
        _drillian = FindObjectOfType<DrillianController>();
        _luna = FindObjectOfType<LunaController>();
        _rigidbody = GetComponent<Rigidbody2D>();

        float size = transform.localScale.x;
        transform.localScale = Vector3.zero;
        MoveTween = transform.DOScale(size, 0.33f).SetEase(Ease.OutBack).OnComplete(() => Active = true);
    }

    private void Start()
    {
        transform.position = SpawnPosition;
      //  _rigidbody.AddForce(CalculateQuadraticBezierPointTangent(0, SpawnPosition, MidPosition, GoalPosition) * _startImpulseScale, ForceMode2D.Impulse);
    }

    //private void FixedUpdate()
    //{
    //    if (!_applyGravity)
    //        return;

    //    if (transform.position.magnitude < Utilities.InnerOrbit - 0.1f)
    //    {
    //        Debug.Log("Done");
    //        _applyGravity = false;
    //        _rigidbody.velocity = Vector2.zero;
    //        return;
    //    }
    //    //if ((MapPointOntoPlanetSurface(transform.position) - (Vector2)transform.position).magnitude
    //    //   < (MapPointOntoPlanetSurface(GoalPosition) - GoalPosition).magnitude)
    //    //{
    //    //    _applyGravity = false;
    //    //    return;
    //    //}

    //    float maxSteepness = CalculateQuadraticBezierPointTangent(0, SpawnPosition, MidPosition, GoalPosition).magnitude;
    //    float minSteepness = CalculateQuadraticBezierPointTangent(.5f, SpawnPosition, MidPosition, GoalPosition).magnitude; // the way the midpoint is positioned the min value is always at t=0.5, in case trajectory will no be "perfect" arc anymore, this needs to be adjusted
    //    float tangentMagnitude = CalculateQuadraticBezierPointTangent(_timePassed / _inAirDuration, SpawnPosition, MidPosition, GoalPosition).magnitude;
    //    //tangentMagnitude = Mathf.InverseLerp(minSteepness, maxSteepness, tangentMagnitude);

    //    Vector2 gravityDirection;

    //    ////Debug.Log(gravityDirection);

    //    //if (_timePassed >= _inAirDuration / 2f)
    //    //{
    //    //    gravityDirection = (MapPointOntoPlanetSurface(CalculateQuadraticBezierPoint(1, SpawnPosition, MidPosition, GoalPosition)) - (Vector2)transform.position).normalized;
    //    //}
    //    //else
    //    //{
    //    // gravityDirection = ((Vector2)transform.position - MapPointOntoPlanetSurface(CalculateQuadraticBezierPoint(1, SpawnPosition, MidPosition, GoalPosition))).normalized;
    //    //}

    //    // if ((MapPointOntoPlanetSurface(transform.position) - (Vector2)transform.position).magnitude
    //    //   >= (MapPointOntoPlanetSurface(MidPosition) - MidPosition).magnitude)
    //    // {
    //    //    Debug.Log("Above zenit");
    //    //     tangentMagnitude *= -1;
    //    ////  // gravityDirection =   CalculateQuadraticBezierPoint(_timePassed / _inAirDuration, SpawnPosition, MidPosition, GoalPosition) - (Vector2)transform.position;
    //    ////  // Debug.Log("Up");
    //    ////  //Debug.Log($"{transform.position}, {MidPosition}, {gravityDirection}");
    //    //}
    //    //else
    //    //{
    //    //    gravityDirection = (Vector2)transform.position - CalculateQuadraticBezierPoint(_timePassed / _inAirDuration, SpawnPosition, MidPosition, GoalPosition);
    //    //    Debug.Log("Down");
    //    //    Debug.Log($"{transform.position}, {GoalPosition}, {gravityDirection}");
    //    //}

 
    //    gravityDirection = -(Vector2)transform.position + MapPointOntoPlanetSurface(transform.position);
    //    Debug.Log(gravityDirection);
    //    _rigidbody.velocity += gravityDirection * _gravityStrength * Time.fixedDeltaTime;




    //    //Debug.Log(_rigidbody.velocity);

    //    _timePassed += Time.fixedDeltaTime;
      
           
    //    //if (_timePassed > _inAirDuration)
    //    //{
    //    //    Debug.Log("Done");
    //    //    _rigidbody.velocity = Vector2.zero;
    //    //    _applyGravity = false;
    //    //}

    //}

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
    * Calculates a point at time t in a path between points p0, p1 and p2.
    */
    Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }

    /*
     * Calculates a point at time t in a path between points p0, p1 and p2.
    */
    Vector2 CalculateQuadraticBezierPointTangent(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
    }

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
