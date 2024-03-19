using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for classes that want to know of certain AudioEvents.
public interface IAudioSubscriber<T> where T : IAudioEvent 
{
    void OnAudioEvent(T audioEvent); // This Function will be called when the AudioEvent occurs.
}
