using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SettingSaver
{   
    
    static string SettingSaveFile
        => Path.Combine(Application.persistentDataPath, "settings.json");
    public static void Load()
    {
        if(!File.Exists(SettingSaveFile))
        {
            return;
        }
        var json = File.ReadAllText(SettingSaveFile);
        var SerializedSettings = new SerializedSettings();
        JsonUtility.FromJsonOverwrite(json, SerializedSettings);
        SerializedSettings.Deserialize();
    }


    public static void Save()
    {
        var SerializedSettings = new SerializedSettings();
        SerializedSettings.Serialize();
        var json = JsonUtility.ToJson(SerializedSettings);
        File.WriteAllText(SettingSaveFile, json);
    }
    
}
