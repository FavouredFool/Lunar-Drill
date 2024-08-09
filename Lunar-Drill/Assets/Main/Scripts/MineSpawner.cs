using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    #region --- Exposed Fields ---
    [Header("Configuration")]
    [SerializeField] [Range(1, 20)] int _maxMines;

    [Header("Blueprint")]
    [SerializeField] MineController _mineBlueprint;

    [Header("Spawning")]
    [SerializeField] [Range(1, 10)] int _spawnAmount;
    [SerializeField] [Range(0.1f, 0.4f)] float _planetCoverage;

    [Header("Arc")]
    [SerializeField] [Range(0.1f, 2)] float _inAirDuration = 1.5f;
    [SerializeField] [Range(0, 5f)] float _additionalHightMul = 2.5f;

    [Header("TESTING")]
    [SerializeField] [Range(-90, 90)] float _spawnAngle = 60;
    #endregion

    #region --- Private Variables ---
    List<MineController> _activeMines = new();
    SpiderController _spider;

    float _spawnTime; // TODO for testing
    #endregion

    #region --- Public Fields ---
    #endregion

    #region --- Unity Fields ---
    private void Start()
    {
        // TODO: Test spawning
        _spider = FindObjectOfType<SpiderController>();
        _spawnTime = Time.time;
    }

    private void Update()
    {
        // TODO: Test spawning
        if (_activeMines.Count + _spawnAmount < _maxMines)
        {
            if (Time.time - _spawnTime >= 2)
            {
                Vector2 angle = Quaternion.Euler(0, 0, _spawnAngle) * _spider.transform.position.normalized;
                SpawnMines(_spider.transform.position.normalized * Utilities.InnerOrbit, angle);
                _spawnTime = Time.time;
            }
        }
    }
    #endregion

    #region --- Public Methods ---

    /*
    * Spawns _spawnAmount mines.
    * SpawnPosition: Where the mines start off.
    * DirectionSpider: The angle with which the spider is jumping out of the planet.
    */
    public void SpawnMines(Vector2 spawnPosition, Vector2 directionSpider)
    {
        // spawn position mapped onto planet surface (might already be on there, but it is more flexible like this)
        Vector2 spawnPositionSurface = MapPointOntoPlanetSurface(spawnPosition);
        // angle of spawn position on planet surface in radians
        float spawnPositionSurfaceAngle = Mathf.Atan2(spawnPositionSurface.y, spawnPositionSurface.x);

        // --- Figuring out which part of planet surface should be covered in mines ---

        // Direction of spider project onto planet
        Vector2 directionSpiderSurface = MapPointOntoPlanetSurface(spawnPositionSurface + directionSpider);
        float angleSpiderSurfaceAngle = Mathf.Atan2(directionSpiderSurface.y, directionSpiderSurface.x);

        // How much of angle compared to normal is spider coming out at
        float angleDot = Vector2.Dot(directionSpider, spawnPositionSurface.normalized);
        // How much of surface angle is covered by mines
        float coverageAngle = 2 * Mathf.PI * _planetCoverage;
        // How much does the angle of the spider influence the amount of shifting of the covered area in the direction of the spider direction
        float angleShift = (1 - angleDot) * 1.2f * coverageAngle;

        // Angles to left and right of spawn mapped onto planet surface
        float spawnLeftAngle = spawnPositionSurfaceAngle
            - (coverageAngle + Mathf.Sign(spawnPositionSurfaceAngle - angleSpiderSurfaceAngle) * angleShift) / 2;
        float spawnRightAngle = spawnPositionSurfaceAngle
            + (coverageAngle + -1 * Mathf.Sign(spawnPositionSurfaceAngle - angleSpiderSurfaceAngle) * angleShift) / 2; ;

        #region THIS IS THE SAME AS THE TWO LINES ABOVE; I JUST LEFT IT AS AN EXPLANATION
        // THIS IS THE SAME AS THE TWO LINES ABOVE; I JUST LEFT IT AS AN EXPLANATION
        //if (spawnPositionSurfaceAngle < angleSpiderSurfaceAngle) // Spider coming out towards left
        //{
        //    spawnLeftAngle = spawnPositionSurfaceAngle - (coverageAngle - angleShift) / 2;
        //    spawnRightAngle = spawnPositionSurfaceAngle + (coverageAngle + angleShift) / 2;
        //}
        //else if (spawnPositionSurfaceAngle > angleSpiderSurfaceAngle) // Spider coming out towards right
        //{
        //    spawnLeftAngle = spawnPositionSurfaceAngle - (coverageAngle + angleShift) / 2;
        //    spawnRightAngle = spawnPositionSurfaceAngle + (coverageAngle - angleShift) / 2;
        //}
        //else // Spider coming out straight
        //{
        //    spawnLeftAngle = spawnPositionSurfaceAngle - coverageAngle / 2;
        //    spawnRightAngle = spawnPositionSurfaceAngle + coverageAngle / 2;
        //}
        #endregion

        // Equal spread of mines between left and right angles
        float deltaSpawnAngle = spawnRightAngle - spawnLeftAngle;
        float angleIncrement = _spawnAmount > 1 ? deltaSpawnAngle / (_spawnAmount - 1) : deltaSpawnAngle;

        // --- Spawning mines and animating them along a path ---
        for (int i = 0; i < _spawnAmount; i++)
        {
            // TODO differnt speed depending on path lenght? -> with this test below it looks stupid and I think all should have same time but different speeds then I guess
            // TODO if they have differnt heights, then this should be adjusted
            //float duration = inAirDuration * ((Mathf.Abs(_spawnAmount / 2f - i)) / (_spawnAmount * (_spawnAmount + 1) / 2));

            // --- Path Positions ---
            float goalAngle = spawnLeftAngle + i * angleIncrement;
            Vector2 goalPosition = new Vector2(Utilities.InnerOrbit * Mathf.Cos(goalAngle),
                                            Utilities.InnerOrbit * Mathf.Sin(goalAngle));

            float midPositionAngle = (goalAngle + spawnPositionSurfaceAngle) / 2;


            Vector2 midPosition = new Vector2(Utilities.InnerOrbit * Mathf.Cos(midPositionAngle),
                                           Utilities.InnerOrbit * Mathf.Sin(midPositionAngle));

            // (1 + spawnPosition.magnitude - Utilities.InnerOrbit) is there to adjust if mines do not start of on planet surface
            // might need to be calculated in a smarter way, but works for now
            midPosition += midPosition.normalized * _additionalHightMul * (1 + spawnPosition.magnitude - Utilities.InnerOrbit);

            // --- Spawning mine ---
            MineController mine = Instantiate(_mineBlueprint, transform);

            // Movement
            float maxSteepness = CalculateQuadraticBezierPointTangent(0, spawnPosition, midPosition, goalPosition).magnitude;
            float minSteepness = CalculateQuadraticBezierPointTangent(.5f, spawnPosition, midPosition, goalPosition).magnitude; // the way the midpoint is positioned the min value is always at t=0.5, in case trajectory will no be "perfect" arc anymore, this needs to be adjusted
            float mineEase(float time, float duration, float overshootOrAmplitude, float period)
            {
                float steepness = Mathf.Clamp01(Mathf.InverseLerp(minSteepness, maxSteepness, CalculateQuadraticBezierPointTangent(time / duration, spawnPosition, midPosition, goalPosition).magnitude));
                float unadjusted = Mathf.Clamp01(-0.5f * (Mathf.Cos(Mathf.PI * Mathf.Clamp01(time / duration)) - 1)); // InOutSine
                return Mathf.Clamp01(Mathf.Pow(unadjusted, 1 / (1 + steepness)));
            }
            mine.MoveTween = DOTween.To(() => 0f,
                t => mine.transform.position = CalculateQuadraticBezierPoint(t, spawnPosition, midPosition, goalPosition)
                , 1f
                , _inAirDuration)
                .SetEase(mineEase);
            _activeMines.Add(mine);
        }
    }

    public void RemoveMine(MineController mine)
    {
        _activeMines.Remove(mine);
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
}
