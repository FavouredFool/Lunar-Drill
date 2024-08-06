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
    [SerializeField] [Range(0.1f, 5f)] float _outerRadius;
    [SerializeField] [Range(0.1f, 5f)] float _planetRadius;
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
                SpawnMines();
                _spawnTime = Time.time;
            }
        }
    }
    #endregion

    #region --- Private Methods ---
    #endregion

    #region --- Public Methods ---

    /*
    * Spawns a mine given...
    */
    public void SpawnMines()
    {
        // TODO: this should just be the spider position once she jumps
        Vector2 spawnPoint = _spider.transform.position + _spider.transform.position.normalized;
        // spawn position mapped onto planet surface
        Vector2 spawnPointSurface = spawnPoint - (_outerRadius - _planetRadius) * spawnPoint.normalized;
        // angle of spawn position in radians
        float spawnPointAngle = Mathf.Atan2(spawnPointSurface.y, spawnPointSurface.x);

        // Angles to left and right of spawn mapped onto planet surface
        // TODO see if left and right should be same size
        float coverageAngle = 2 * Mathf.PI * _planetCoverage;
        float spawnLeftAngle = spawnPointAngle - coverageAngle / 2;
        float spawnRightAngle = spawnPointAngle + coverageAngle / 2;

        float deltaSpawnAngle = spawnRightAngle - spawnLeftAngle;
        deltaSpawnAngle = deltaSpawnAngle < 0 ? deltaSpawnAngle += Mathf.PI * 2 : deltaSpawnAngle; // Ensure it is positive
        float angleIncrement = deltaSpawnAngle / (_spawnAmount - 1);

        // Spawning mines
        for (int i = 0; i < _spawnAmount; i++)
        {
            float goalAngle = spawnLeftAngle + i * angleIncrement;
            Vector2 goalPoint = new Vector2(_planetRadius * Mathf.Cos(goalAngle),
                                            _planetRadius * Mathf.Sin(goalAngle));

            MineController mine = Instantiate(_mineBlueprint, transform);

            mine.transform.position = spawnPoint;
            DOTween.To(() => (Vector2)mine.transform.position, x => mine.transform.position = x, goalPoint, 1).SetEase(Ease.OutSine);
            _activeMines.Add(mine);
        }
    }

    public void RemoveMine(MineController mine)
    {
        _activeMines.Remove(mine);
    }
    #endregion
}
