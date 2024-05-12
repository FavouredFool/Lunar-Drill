using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Event to signal that the game was (un-) paused.
public class MenuPauseAudio : IAudioEvent
{
    public enum PauseState
    {
        GameRunning,
        GamePaused
    }

    public PauseState State;

    public MenuPauseAudio(PauseState state)
    {
        this.State = state;
    }
}
