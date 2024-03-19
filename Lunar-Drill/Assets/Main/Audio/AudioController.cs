using System;
using System.Collections.Generic;

// Static class that handles the Distribution of Audio Events.
// Events can be fired to notify all Classes that need to know of the event.
// All Subscribers can subscribe to an the Audiocontroller so that they get notified.
// Decouples Information of Audio event and the Object that needs to receive the event.
public static class AudioController
{
    static Dictionary<Type, List<object>> subscribers = new(); // Dictionary to save whicht subscribers listen to which events.

    public static void Subscribe<T>(IAudioSubscriber<T> sub) where T : IAudioEvent // Method for a Subscriber to subscribe to 
    {
        if (!subscribers.ContainsKey(typeof(T)))
            subscribers.Add(typeof(T), new());
        subscribers[typeof(T)].Add(sub);
    }

    public static void Fire<T>(T e) where T:IAudioEvent // Function used to fire Audio Events. This notifies all (registered)Subscribers of the ivent
    {
        if (subscribers.TryGetValue(typeof(T), out var subs))
            foreach (var sub in subs)
                ((IAudioSubscriber<T>)sub).OnAudioEvent(e);
    }
}
