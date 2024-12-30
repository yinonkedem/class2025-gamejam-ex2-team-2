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
    public const string EVENT_ADD_ATTACK_TO_PINK_PLAYER = "Event: add atack for the pink player";
    public const string EVENT_ADD_ATTACK_TO_GREY_PLAYER = "Event: add atack for the pink player";
    public const string EVENT_GREY_PLAYER_HIT_FROM_ATTACK = "Event: hit logic when grey player get hit";
    public const string EVENT_PINK_PLAYER_HIT_FROM_ATTACK = "Event: hit logic when pink player get hit";


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
