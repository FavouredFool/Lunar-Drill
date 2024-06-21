using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    static string FileName = "LeaderboardEntries";
    public static bool LeaderboardIsEnabled = true;

    public static LeaderboardEntryList EntryList = new();
    
    static string DataPath;
    
    public void Start()
    {
        DataPath = Application.persistentDataPath + "/" + FileName + ".json";

        // Deserialize instantly
        if (File.Exists(DataPath))
        {
            string fileContents = File.ReadAllText(DataPath);
            EntryList = JsonUtility.FromJson<LeaderboardEntryList>(fileContents);
        }

        EntryList.Entries.Sort();
    }
    
    public void AddEntry(float time, string lunaName, string drillianName)
    {
        LeaderboardEntry entry = new LeaderboardEntry(lunaName, drillianName, time);
        EntryList.Entries.Add(entry);
        EntryList.Entries.Sort();
        
        Debug.Log($"Add Entry: {entry}");

        // Serialize instantly
        string jsonString = JsonUtility.ToJson(EntryList);
        File.WriteAllText(DataPath, jsonString);
    }
    public static void Clear()
    {
        EntryList.Entries.Clear();
        string jsonString = JsonUtility.ToJson(EntryList);
        File.WriteAllText(DataPath, jsonString);
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

        public bool isLast => false;
        
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
        public (string,string) ToStrings()
        {
            return (NameManager.MakeTeamName(LunaName, DrillianName), GetFormattedTime());
        }
    }
}
