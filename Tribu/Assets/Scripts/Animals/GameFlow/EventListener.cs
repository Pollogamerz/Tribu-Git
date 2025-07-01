using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventListener<T> : MonoBehaviour
{
    public EventChannel<T> channel;
    public UnityEvent<T> unityEvent;

    protected void Awake() => channel.Register(this);

    private void OnDestroy() => channel.Deregister(this);
    
    public void Raise(T value)
    {
        unityEvent?.Invoke(value);
    }
}
