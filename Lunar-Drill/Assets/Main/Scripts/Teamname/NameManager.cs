using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NameManager : MonoBehaviour
{
    public static string LunaTeamName { get; set; } = "Lunar";
    public static string DrillianTeamName { get; set; } = "Drill";

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
}
