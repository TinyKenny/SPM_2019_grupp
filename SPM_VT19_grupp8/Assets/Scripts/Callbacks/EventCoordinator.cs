using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventCoordinator : MonoBehaviour
{
    public static EventCoordinator CurrentEventCoordinator;

    private void Awake()
    {
        CurrentEventCoordinator = this;
    }

    public delegate void EventListener(EventInfo eventInfo);
    public Dictionary<Type, EventListener> EventListeners;

    public void RegisterEventListener<T>(EventListener eventListener) where T : EventInfo
    {
        if (EventListeners == null)
        {
            EventListeners = new Dictionary<Type, EventListener>();
        }

        if(!EventListeners.ContainsKey(typeof(T)) || EventListeners[typeof(T)] == null)
        {
            EventListeners[typeof(T)] = eventListener;
            return;
        }

        EventListeners[typeof(T)] += eventListener;
    }

    public void UnregisterEventListener<T>(EventListener eventListener) where T : EventInfo
    {
        if(EventListeners == null || !EventListeners.ContainsKey(typeof(T)) || EventListeners[typeof(T)] == null)
        {
            return;
        }

        EventListeners[typeof(T)] -= eventListener;
    }

    public void ActivateEvent(EventInfo eventInfo)
    {
        Type t = eventInfo.GetType();

        if(EventListeners == null || !EventListeners.ContainsKey(t) || EventListeners[t] == null)
        {
            return;
        }

        EventListeners[t](eventInfo);
    }
}
