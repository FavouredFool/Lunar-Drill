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
                SpawnMines(_spider.transform.position.normalized * Utilities.InnerOrbit, _spider.transform.position.normalized);
                _spawnTime = Time.time;
            }
        }
    }
    #endregion

    #region --- Public Methods ---

    /*
    * Spawns _spawnAmount mines.
    * SpawnPosition: Where the mines start off.
    * AngleSpider: The angle with which the spider is jumping out of the planet.
    */
    public void SpawnMines(Vector3 spawnPosition, Vector3 angleSpider)
    {
        // --- Helper variables to figure out goal positions ---
        // spawn position mapped onto planet surface (might already be on there, but it is more flexible like this)
        Vector2 spawnPositionSurface = spawnPosition - (spawnPosition.magnitude - Utilities.InnerOrbit) * spawnPosition.normalized;
        // angle of spawn position on planet surface in radians
        float spawnPositionSurfaceAngle = Mathf.Atan2(spawnPositionSurface.y, spawnPositionSurface.x);

        // Angles to left and right of spawn mapped onto planet surface
        // TODO adjust left and right based on angleSpider
        float coverageAngle = 2 * Mathf.PI * _planetCoverage;
        float spawnLeftAngle = spawnPositionSurfaceAngle - coverageAngle / 2;
        float spawnRightAngle = spawnPositionSurfaceAngle + coverageAngle / 2;

        float deltaSpawnAngle = spawnRightAngle - spawnLeftAngle;
        deltaSpawnAngle = deltaSpawnAngle < 0 ? deltaSpawnAngle += Mathf.PI * 2 : deltaSpawnAngle; // Ensure it is positive
        float angleIncrement = deltaSpawnAngle / (_spawnAmount - 1);

        // --- Spawning mines and animating them along a path ---
        for (int i = 0; i < _spawnAmount; i++)
        {
            // TODO differnt speed depending on path lenght? -> with this test below it looks stupid and I think all should have same time but different speeds then I guess
            //float duration = inAirDuration * ((Mathf.Abs(_spawnAmount / 2f - i)) / (_spawnAmount * (_spawnAmount + 1) / 2));

            float inAirDuration = 1.5f; // TODO dependent on distance?
            float additionalHightMul = 2;

            // --- Path Positions ---
            float goalAngle = spawnLeftAngle + i * angleIncrement;
            Vector2 goalPosition = new Vector2(Utilities.InnerOrbit * Mathf.Cos(goalAngle),
                                            Utilities.InnerOrbit * Mathf.Sin(goalAngle));

            float midPositionAngle = (goalAngle + spawnPositionSurfaceAngle) / 2;


            Vector2 midPosition = new Vector2(Utilities.InnerOrbit * Mathf.Cos(midPositionAngle),
                                           Utilities.InnerOrbit * Mathf.Sin(midPositionAngle));

            // (1 + spawnPosition.magnitude - Utilities.InnerOrbit) is there to adjust if mines do not start of on planet surface
            // might need to be calculated in a smarter way, but works for now
            midPosition += midPosition.normalized * additionalHightMul * (1 + spawnPosition.magnitude - Utilities.InnerOrbit);

            // --- Spawning mine ---
            MineController mine = Instantiate(_mineBlueprint, transform);
            mine.MoveTween = DOTween.To(() => 0f,
                t => mine.transform.position = CalculateQuadraticBezierPoint(t, spawnPosition, midPosition, goalPosition)
                , 1f
                , inAirDuration)
                .SetEase(Ease.InSine);
            _activeMines.Add(mine);
        }


    }
    public void RemoveMine(MineController mine)
    {
        _activeMines.Remove(mine);
    }
    #endregion

    #region --- Private Methods ---
    Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }
    #endregion
}
