using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    public void AddEntry(long totalSeconds, bool isVersionA, bool hasWon, List<Hit> hits)
    {
        HitMetricEntry entry = new HitMetricEntry(totalSeconds, isVersionA, hasWon, hits);
        EntryList.Entries.Add(entry);
        EntryList.Entries.Sort();

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
    public class HitMetricEntry : IComparable<HitMetricEntry>
    {
        public long DateTimeStampInSeconds;
        public bool IsVersionA;
        public bool HasWon;
        public List<Hit> Hits;
        
        public HitMetricEntry(long dateTimeStampInSeconds, bool isVersionA, bool hasWon, List<Hit> hits)
        {
            DateTimeStampInSeconds = dateTimeStampInSeconds;
            IsVersionA = isVersionA;
            HasWon = hasWon;
            Hits = hits;
        }
        
        public int CompareTo(HitMetricEntry other)
        {
            if (this.DateTimeStampInSeconds < other.DateTimeStampInSeconds)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
    
    [System.Serializable]
    public class Hit
    {
        public float HitTime;
        public string HitPlayer;
        public int RemainingSpiderHP;
        public string SpiderAttack;
        public bool WasFriendlyFire;
        
        
        public Hit(float hitTime, string hitPlayer, int remainingSpiderHp, string spiderAttack, bool wasFriendlyFire)
        {
            HitTime = hitTime;
            HitPlayer = hitPlayer;
            RemainingSpiderHP = remainingSpiderHp;
            SpiderAttack = spiderAttack;
            WasFriendlyFire = wasFriendlyFire;
        }
    }
}
