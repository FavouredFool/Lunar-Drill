using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DrillianMetricManager : MonoBehaviour
{
    static string FileName = "DrillianMetrics";
    public static bool LeaderboardIsEnabled = true;

    public static DrillianMetricList EntryList = new();
    public static DrillianMetricEntry LatestEntry;
    
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
            EntryList = JsonUtility.FromJson<DrillianMetricList>(fileContents);
        }
        
        EntryList.Entries.Sort();
    }
    
    public void AddEntry(long timeInSec, bool isVersionA, bool hasWon, List<Activation> activations)
    {
        DrillianMetricEntry entry = new DrillianMetricEntry(timeInSec, isVersionA, hasWon, activations);
        EntryList.Entries.Add(entry);

        LatestEntry = entry;
        
        // Serialize instantly
        string jsonString = JsonUtility.ToJson(EntryList);
        File.WriteAllText(DataPath, jsonString);
    }
    
    [System.Serializable]
    public class DrillianMetricList
    {
        public List<DrillianMetricEntry> Entries = new();
    }
    
    [System.Serializable]
    public class DrillianMetricEntry : IComparable<DrillianMetricEntry>
    {
        public long DateTimeStampInSeconds;
        public bool IsVersionA;
        public bool HasWon;
        public List<Activation> Activations;
        
        public DrillianMetricEntry(long timeInSec, bool isVersionA, bool hasWon, List<Activation> activations)
        {
            DateTimeStampInSeconds = timeInSec;
            IsVersionA = isVersionA;
            HasWon = hasWon;
            Activations = activations;
        }
        
        public int CompareTo(DrillianMetricEntry other)
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
    public class Activation
    {
        public bool WasActivatedOutside;
        public float ActivationTime;
        public int RemainingSpiderHP;
        public string SpiderAttack;
        
        public Activation(bool wasActivatedOutside, float activationTime, int remainingSpiderHp, string spiderAttack)
        {
            WasActivatedOutside = wasActivatedOutside;
            ActivationTime = activationTime;
            RemainingSpiderHP = remainingSpiderHp;
            SpiderAttack = spiderAttack;
        }
    }
}
