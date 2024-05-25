using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    [SerializeField]
    EventReference _musicEvent; // the event that plays the music

    EventInstance _music;

    // TODO: change after merging with new scenes.
    void Awake()
    {
        
    }

    void OnDestroy()
    {
        
    }
}
