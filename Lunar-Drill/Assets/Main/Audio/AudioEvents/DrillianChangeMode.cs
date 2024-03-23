using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Event fired when Drillian changes his mode
public class DrillianChangeMode : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public DrillianChangeMode(string info)
    {
        this.Info = info;
    }
}
