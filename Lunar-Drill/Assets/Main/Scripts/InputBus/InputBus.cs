using System;
using System.Collections.Generic;
using UnityEngine;

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

        if (e is not Signal_AnyFire)
            Fire(new Signal_AnyFire());
    }

    public static void Unsubscribe<T>(IInputSubscriber<T> sub) where T : IInputSignal // Method for a Subscriber to unsubscribe from the Audio Controller
    {
        if (!subscribers.ContainsKey(typeof(T)))
            return;
        subscribers[typeof(T)].Remove(sub);
    }
}

public class Signal_AnyFire : IInputSignal
{

}