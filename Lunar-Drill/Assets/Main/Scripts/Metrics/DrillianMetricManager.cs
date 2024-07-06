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
    
    public void AddEntry(bool isVersionA, int outsideActivations, int insideActivations, int dormantActivations, int chargingActivations, int attackActivations)
    {
        DrillianMetricEntry entry = new DrillianMetricEntry(outsideActivations, insideActivations, dormantActivations, chargingActivations, attackActivations);
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
    public class DrillianMetricEntry
    {
        public int OutsideActivations;
        public int InsideActivations;

        public int DormantActivations;
        public int ChargingActivations;
        public int AttackActivations;
        
        public DrillianMetricEntry(int outsideActivations, int insideActivations, int dormantActivations, int chargingActivations, int attackActivations)
        {
            OutsideActivations = outsideActivations;
            InsideActivations = insideActivations;
            DormantActivations = dormantActivations;
            ChargingActivations = chargingActivations;
            AttackActivations = attackActivations;
        }
    }
}
