using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class OreSpawner : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Configuration")]
    [SerializeField][Range(1, 20)] int _maxOres;
    [SerializeField][Range(0.1f, 5)] float _maxSpawnSpeed;
    [SerializeField] [Range(0, 1)] float _maxChargedPercentage = 0.5f;
    [SerializeField] [Range(0, 24)] float _chargedPercentageDecay = 2f;

    [Header("Blueprint")]
    [SerializeField] OreController _oreBlueprint;

    [Header("Placement")]
    [SerializeField][Range(0.05f, 0.5f)] float _planetOuterPaddingPercentage;
    [SerializeField][Range(0.1f, 5)] float _orePadding;


    //--- Private Fields ------------------------

    float _spawnT = 0;
    float _spawnSpeed;
    List<OreController> _activeOres = new();
    public float _chargedPercentage = 0;

    DrillianController _drillian;
    
    //--- Unity Methods ------------------------

    public void Start()
    {
        _drillian = FindObjectOfType<DrillianController>();
    }

    public void FixedUpdate()
    {
        UpdateChargedPercentage();
        
        _spawnSpeed = DOVirtual.EasedValue(_maxSpawnSpeed, 0, _activeOres.Count / (float)_maxOres, Ease.Linear);
        
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

    void UpdateChargedPercentage()
    {
        if (!ChargedOreWanted()) return;
        
        _chargedPercentage = _maxChargedPercentage + (_chargedPercentage - _maxChargedPercentage) * Mathf.Exp(-_chargedPercentageDecay * Time.deltaTime);
    }
    
    void SpawnOre()
    {
        OreController ore = Instantiate(_oreBlueprint, transform);
        ore.transform.position = GetSpawnPosition();
        ore.transform.rotation = Quaternion.LookRotation(Vector3.forward, Random.insideUnitSphere.normalized);
        _activeOres.Add(ore);
    }

    Vector2 GetSpawnPosition()
    {
        Vector2 finalPlacement = Vector2.zero;
        
        for (int i = 0; i < 10000; i++)
        {
            bool placementAllowed = true;
            Vector2 placementIdea = Random.insideUnitCircle * (Utilities.PlanetRadius * (1 - _planetOuterPaddingPercentage));

            foreach (OreController ore in _activeOres)
            {
                float distance = Vector2.Distance(ore.transform.position, placementIdea);

                if (distance < _orePadding)
                {
                    placementAllowed = false;
                }
            }

            if (placementAllowed)
            {
                finalPlacement = placementIdea;
                break;
            }
        }

        return finalPlacement;
    }

    public bool ChargedOreShouldSpawn()
    {
        bool randomCharged = Random.Range(0f, 1f) < _chargedPercentage;

        bool willSpawnChargedOre = ChargedOreWanted() && randomCharged;

        if (willSpawnChargedOre)
        {
            _chargedPercentage = 0;
        }
        
        return willSpawnChargedOre;
    }

    public bool ChargedOreWanted()
    {
        bool oreIsCharged = _activeOres.Any(e => e.IsCharged);
        bool drillianCharged = _drillian.IsActionAvaliable;

        return !oreIsCharged && !drillianCharged;
    }


}
