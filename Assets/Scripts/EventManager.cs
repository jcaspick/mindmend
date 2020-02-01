using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EventDetails
{
    public Vector2Int coordinates;
    public Node node;

    public EventDetails(Vector2Int coordinates)
    {
        this.coordinates = coordinates;
    }

    public EventDetails(Node node)
    {
        this.node = node;
        this.coordinates = node.gridCoordinates;
    }
}

public enum EventType
{
    SpaceClicked,
    EditorMarkerEnter,
    EditorMarkerExit,
    EditorMarkerClick
}

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    Dictionary<EventType, Action<EventDetails>> eventDictionary;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        eventDictionary = new Dictionary<EventType, Action<EventDetails>>();
    }

    public static void AddListener(EventType type, Action<EventDetails> callback)
    {
        Action<EventDetails> callbackList = null;
        if (instance.eventDictionary.TryGetValue(type, out callbackList))
        {
            callbackList += callback;
            instance.eventDictionary[type] = callbackList;
        }
        else
        {
            instance.eventDictionary.Add(type, callback);
        }
    }

    public static void RemoveListener(EventType type, Action<EventDetails> callback)
    {
        Action<EventDetails> callbackList = null;
        if (instance.eventDictionary.TryGetValue(type, out callbackList))
        {
            callbackList -= callback;
            instance.eventDictionary[type] = callbackList;
        }
        else
        {
            Debug.LogWarning("you tried to remove a non-existent listener, this is probably a bug");
        }
    }

    public static void Invoke(EventType type, EventDetails details)
    {
        Action<EventDetails> callbackList = null;
        if (instance.eventDictionary.TryGetValue(type, out callbackList))
        {
            if (callbackList != null)
            {
                callbackList.Invoke(details);
            }
        }
    }
}