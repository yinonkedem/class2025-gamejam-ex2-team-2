using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    protected EventManager()
    {
        Init();
    }
    private class Event : UnityEvent<GameObject> { } //empty class; just needs to exist
    private Dictionary<string, Event> eventDictionary;
    public const string EVENT_DECREASE_PLAYER_LIFE = "Event: decrese the player's life";




    private void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Event>();
        }
    }


    public void StartListening(string eventName, UnityAction<GameObject> listener)
    {
        Event thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new Event();
            thisEvent.AddListener(listener);
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    public void TriggerEvent(string eventName, GameObject data)
    {
        Event thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(data);
        }
    }
}
