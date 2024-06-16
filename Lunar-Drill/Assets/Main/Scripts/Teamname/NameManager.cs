using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class NameManager : MonoBehaviour
{
    [SerializeField] TMP_Text TeamNameUI;
    
    public static string LunaTeamName { get; set; } = "Lunar";
    public static string DrillianTeamName { get; set; } = "Drill";

    static List<string> LunaNameOptions { get; set; }
    static List<string> DrillianNameOptions { get; set; }
    
    // Yes i hardcoded them. Sue me. -Tim
    static string[] LunaNames =
    {
        // Ich habe Octagon zu octa und omega zu mega gekürzt, weil die eine Silbe zu viel hatten.
        "Octa",
        "Turbo",
        "Stellar",
        "Solar",
        "Umbra",
        "Giant",
        "Titan",
        "Cyber",
        "Quantum",
        "Cosmic",
        "Astral",
        "Martian",
        "Neon",
        "Orbit",
        "Crystal",
        "Heavy",
        "Iron",
        "Vivid",
        "Ultra",
        "Aether",
        "Delta",
        "Exo",
        "Extra",
        "Fractal",
        "Gamma",
        "Halo",
        "Hybrid",
        "Macro",
        "Plasma",
        "Neo",
        "Neural",
        "Static",
        "Data",
        "Hyper",
        "Lumen",
        "Nano",
        "Meta",
        "Mega",
        "Sonic",
        "Sonar",
        "Alpha",
        "Apex",
        "Echo",
        "Fusion",
        "Vortex",
        "Matrix",
        "Nexus",
        "Super",
        "Pixel",
        "Quasar",
        "Shadow",
        "Spectrum",
        "Aeon",
        "Nova",
        "Synapse",
        "System",
        "Theta",
        "Xeno",
        "Thunder",
        "Twilight",
        "Mega",
        "Major",
        "Epic",
        "Fatal",
        "Final",
    };
    
    static string[] DrillianNames =
    {
        "Nox",
        "Core",
        "Gem",
        "Deep",
        "Rock",
        "Lode",
        "Quartz",
        "Vein",
        "Spark",
        "Arc",
        "Orb",
        "Dark",
        "Flux",
        "Heat",
        "Jet",
        "Psi",
        "Pulse",
        "Rift",
        "Shift",
        "Crust",
        "Dust",
        "Mine",
        "Lock",
        "Warp",
        "Wave",
        "Lens",
        "Zone",
        "Zoom",
        "Reign",
        "Quake",
        "Bolt",
        "Boost",
        "Cloud",
        "Droid",
        "Gene",
        "Space",
        "Blink",
        "Edge",
        "Flash",
        "Fuse",
        "Grid",
        "Mask",
        "Zen",
        "Link",
        "Ray",
        "Sync",
        "Ion",
        "Trace",
        "Blaze",
        "Byte",
        "Dawn",
        "Forge",
        "Code",
        "Gear",
        "Mint",
        "Ring",
        "Sky",
        "Star",
        "Volt",
        "White",
        "Red",
        "Blue",
        "Glaze",
        "Strike",
    };

    public void SetNameOptions()
    {
        (List<string>, List<string>) nameOptions = GetNameOptions();
        LunaNameOptions = nameOptions.Item1;
        DrillianNameOptions = nameOptions.Item2;

        // TODO TEMPORARY JUST SO THAT THERE'S SOME RANDOM NAMES. THIS CURRENTLY CHANGES EVERY SCENE CHANGE
        LunaTeamName = LunaNameOptions[0];
        DrillianTeamName = DrillianNameOptions[0];
        UpdateTeamNameUI();
    }
    
    public static (List<string>, List<string>) GetNameOptions()
    {
        List<string> lunaOptions = new List<string>(3);
        List<string> drillianOptions = new List<string>(3);

        for (int i = 0; i < 3; i++)
        {
            lunaOptions.Add(GetNameFromNames(LunaNames, lunaOptions));
            drillianOptions.Add(GetNameFromNames(DrillianNames, drillianOptions));
        }

        return (lunaOptions, drillianOptions);
    }

    static string GetNameFromNames(string[] allNames, List<string> blockedNames)
    {
        for (int k = 0; k < 100000; k++)
        {
            int randomIndex = Random.Range(0, allNames.Length);
            string name = allNames[randomIndex];
            if (!blockedNames.Contains(name))
            {
                return name;
            }
        }
        
        Assert.IsTrue(false);
        return "";
    }

    void UpdateTeamNameUI()
    {
        TeamNameUI.text = MakeTeamName(LunaTeamName, DrillianTeamName);
    }
    
    public static string MakeTeamName(string lunaName, string drillianName) => lunaName + " // " + drillianName;
}
