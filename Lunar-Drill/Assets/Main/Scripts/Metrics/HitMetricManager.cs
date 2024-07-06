using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMetricManager : MonoBehaviour
{
    [System.Serializable]
    public class HitMetricList
    {
        public List<HitMetricEntry> Entries = new();
    }
    
    [System.Serializable]
    public class HitMetricEntry
    {
        public List<Hit> Hits;
        
        public HitMetricEntry(List<Hit> hits)
        {
            Hits = hits;
        }
    }
    
    [System.Serializable]
    public class Hit
    {
        public string HitPlayer;
        public int RemainingSpiderHP;
        public string SpiderAttack;
        public bool WasFriendlyFire;
        
        
        public Hit(string hitPlayer, int remainingSpiderHp, string spiderAttack, bool wasFriendlyFire)
        {
            HitPlayer = hitPlayer;
            RemainingSpiderHP = remainingSpiderHp;
            SpiderAttack = spiderAttack;
            WasFriendlyFire = wasFriendlyFire;
        }
    }
}
