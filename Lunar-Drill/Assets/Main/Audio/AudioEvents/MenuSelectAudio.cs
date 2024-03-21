using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelectAudio : IAudioEvent
{
    public string Info;

    public MenuSelectAudio(string info)
    {
        this.Info = info;
    }
}
