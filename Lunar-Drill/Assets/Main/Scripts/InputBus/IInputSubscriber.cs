using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputSubscriber<T> where T : IInputSignal
{
    void OnEventHappened(T e);
}
