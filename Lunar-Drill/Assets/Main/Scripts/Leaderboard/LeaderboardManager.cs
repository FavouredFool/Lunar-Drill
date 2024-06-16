using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] string fileName = "LeaderboardEntries";

    LeaderboardEntryList entryList = new();
    
    string dataPath;
    
    public void Start()
    {
        dataPath = Application.persistentDataPath + "/" + fileName + ".json";

        // Deserialize instantly
        if (File.Exists(dataPath))
        {
            string fileContents = File.ReadAllText(dataPath);
            entryList = JsonUtility.FromJson<LeaderboardEntryList>(fileContents);
        }

        entryList.Entries.Sort();
    }
    
    public void AddEntry(float time, string lunaName, string drillianName)
    {
        LeaderboardEntry entry = new LeaderboardEntry(lunaName, drillianName, time);
        entryList.Entries.Add(entry);
        entryList.Entries.Sort();
        
        Debug.Log($"Add Entry: {entry}");

        // Serialize instantly
        string jsonString = JsonUtility.ToJson(entryList);
        File.WriteAllText(dataPath, jsonString);
    }
    
    [System.Serializable]
    public class LeaderboardEntryList
    {
        public List<LeaderboardEntry> Entries = new();
    }

    
    [System.Serializable]
    public class LeaderboardEntry : IComparable<LeaderboardEntry>
    {
        public string LunaName;
        public string DrillianName;
        public double Time;
        
        public LeaderboardEntry(string lunaName, string drillianName, double time)
        {
            LunaName = lunaName;
            DrillianName = drillianName;
            Time = time;
        }

        public int CompareTo(LeaderboardEntry other)
        {
            if (this.Time < other.Time)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public string GetFormattedTime()
        {
            return Time.ToString("00.000") + "s";
        }

        public override string ToString()
        {
            return NameManager.MakeTeamName(LunaName, DrillianName) + " | " + GetFormattedTime();
        }
    }
}
