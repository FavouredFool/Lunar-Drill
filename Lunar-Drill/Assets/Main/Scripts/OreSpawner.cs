using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Configuration")]
    [SerializeField][Range(1, 20)] int _maxOres;
    [SerializeField][Range(0.1f, 5)] int _maxSpawnSpeed;

    [Header("Blueprint")]
    [SerializeField] OreController _oreBlueprint;


    //--- Private Fields ------------------------

    float _spawnT = 0;
    float _spawnSpeed;
    List<OreController> _activeOres = new();


    //--- Unity Methods ------------------------

    public void FixedUpdate()
    {
        _spawnSpeed = DOVirtual.EasedValue(_maxSpawnSpeed, 0, _activeOres.Count / (float)_maxOres, Ease.Linear);
        Debug.Log(_spawnSpeed);

        _spawnT += _spawnSpeed * Time.deltaTime;

        if (_spawnT > 1)
        {
            _spawnT -= 1;
            SpawnOre();
        }
    }


    //--- Public Methods ------------------------

    public void RemoveOre(OreController oreToRemove)
    {
        _activeOres.Remove(oreToRemove);
    }


    //--- Private Methods ------------------------

    void SpawnOre()
    {
        Debug.Log("SpawnOre");
        OreController ore = Instantiate(_oreBlueprint, transform);
        _activeOres.Add(ore);
    }


}
