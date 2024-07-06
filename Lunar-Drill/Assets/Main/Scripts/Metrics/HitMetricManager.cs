using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HitMetricManager : MonoBehaviour
{
    static string FileName = "HitMetrics";
    public static bool LeaderboardIsEnabled = true;

    public static HitMetricList EntryList = new();
    public static HitMetricEntry LatestEntry;
    
    public static int LatestIndex {
        get
        {
            if (LatestEntry == null) return 0;
            return EntryList.Entries.IndexOf(LatestEntry);
        }
    }
    
    static string DataPath;
    
    public void Start()
    {
        DataPath = Application.persistentDataPath + "/" + FileName + ".json";

        // Deserialize instantly
        if (File.Exists(DataPath))
        {
            string fileContents = File.ReadAllText(DataPath);
            EntryList = JsonUtility.FromJson<HitMetricList>(fileContents);
        }

        EntryList.Entries.Sort();
    }
    
    public void AddEntry(bool isVersionA, List<Hit> hits)
    {
        HitMetricEntry entry = new HitMetricEntry(hits);
        EntryList.Entries.Add(entry);

        LatestEntry = entry;
        
        // Serialize instantly
        string jsonString = JsonUtility.ToJson(EntryList);
        File.WriteAllText(DataPath, jsonString);
    }

    
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
