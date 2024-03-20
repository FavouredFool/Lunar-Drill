using System;
using System.Collections.Generic;

public static class InputBus
{
    static Dictionary<Type, List<object>> subscribers = new();
    public static void Subscribe<T>(IInputSubscriber<T> sub) where T : IInputSignal
    {
        if (!subscribers.ContainsKey(typeof(T)))
            subscribers.Add(typeof(T), new());
        subscribers[typeof(T)].Add(sub);
    }
    public static void Fire<T>(T e) where T : IInputSignal
    {
        if (subscribers.TryGetValue(typeof(T), out var subs))
            foreach (var sub in subs)
                ((IInputSubscriber<T>)sub).OnEventHappened(e);
    }
}
