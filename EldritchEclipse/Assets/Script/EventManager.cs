using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private static EventManager instance;

    public static EventManager Instance // making the eventmanager singleton
    {
        get
        {
            if (instance == null)
            {
                instance = new EventManager();
            }
            return instance;
        }
    }

    private Dictionary<Event, List<Delegate>> eventListeners;

    public EventManager() //constructor to initalize eventListeners dictionary
    {
        eventListeners = new();
    }

    /* Method for add, remove and trigger events
     * Tried to use method overloading to make all of them use the same method but with
     * different returns and parameters
     */

    //uses the Action delegate which does not return a value
    #region ADD/REMOVE/TRIGGER (NO PARAMETERS)
    public void AddListener(Event eventName, Action listener)
    {
        // If the event does not exist in the dictionary, add it.
        if (!eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName] = new List<Delegate>();
        }

        // Add the listener to the event's list of delegates.
        eventListeners[eventName].Add(listener);
    }

    public void RemoveListener(Event eventName, Action listener)
    {
        // If the event exists, remove the listener from its list of delegates.
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName].Remove(listener);
        }
    }

    public void TriggerEvent(Event eventName)
    {
        // If the event exists, invoke all listeners associated with it.
        if (eventListeners.ContainsKey(eventName))
        {
            var listeners = eventListeners[eventName].ToArray();
            foreach (var listener in listeners)
            {
                if (listener is Action action)
                {
                    action.Invoke();
                }
            }
        }
        else
        {
            Debug.LogError("Event not in list");
        }

    }
    #endregion

    //similar to the above but allows passing of parameters
    #region ADD/REMOVE/TRIGGER (W/ PARAM)

    //takes in 1 parameter
    public void AddListener<TParam>(Event eventName, Action<TParam> listener)
    {
        if (!eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName] = new List<Delegate>();
        }

        eventListeners[eventName].Add(listener);
    }

    public void RemoveListener<TParam>(Event eventName, Action<TParam> listener)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName].Remove(listener);
        }
    }

    public void TriggerEvent<TParam>(Event eventName, TParam param)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            var listeners = eventListeners[eventName].ToArray();
            foreach (var listener in listeners)
            {
                if (listener is Action<TParam> action)
                {
                    action.Invoke(param);
                }
            }
        }
    }

    //takes in 2 parameters
    public void AddListener<TParam1, TParam2>(Event eventName, Action<TParam1, TParam2> listener)
    {
        if (!eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName] = new List<Delegate>();
        }

        eventListeners[eventName].Add(listener);
    }

    public void RemoveListener<TParam1, TParam2>(Event eventName, Action<TParam1, TParam2> listener)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName].Remove(listener);
        }
    }

    public void TriggerEvent<TParam1, TParam2>(Event eventName, TParam1 param1, TParam2 param2)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            foreach (var listener in eventListeners[eventName])
            {
                if (listener is Action<TParam1, TParam2> action)
                {
                    action.Invoke(param1, param2);
                }
            }
        }
    }

    //take in 3 parameters
    public void AddListener<TParam1, TParam2,TParam3>(Event eventName, Action<TParam1, TParam2, TParam3> listener)
    {
        if (!eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName] = new List<Delegate>();
        }

        eventListeners[eventName].Add(listener);
    }

    public void RemoveListener<TParam1, TParam2, TParam3>(Event eventName, Action<TParam1, TParam2, TParam3> listener)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName].Remove(listener);
        }
    }

    public void TriggerEvent<TParam1, TParam2, TParam3>(Event eventName, TParam1 param1, TParam2 param2,TParam3 param3)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            foreach (var listener in eventListeners[eventName])
            {
                if (listener is Action<TParam1, TParam2, TParam3> action)
                {
                    action.Invoke(param1, param2,param3);
                }
            }
        }
    }

    //take in 4 parameters
    public void AddListener<TParam1, TParam2, TParam3, TParam4>(Event eventName, Action<TParam1, TParam2, TParam3, TParam4> listener)
    {
        if (!eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName] = new List<Delegate>();
        }

        eventListeners[eventName].Add(listener);
    }

    public void RemoveListener<TParam1, TParam2, TParam3, TParam4>(Event eventName, Action<TParam1, TParam2, TParam3, TParam4> listener)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName].Remove(listener);
        }
    }

    public void TriggerEvent<TParam1, TParam2, TParam3, TParam4>(Event eventName, TParam1 param1, TParam2 param2, TParam3 param3,TParam4 param4)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            foreach (var listener in eventListeners[eventName])
            {
                if (listener is Action<TParam1, TParam2, TParam3, TParam4> action)
                {
                    action.Invoke(param1, param2, param3, param4);
                }
            }
        }
    }
    #endregion

    //uses the Func delegate which returns a value
    #region ADD/REMOVE/TRIGGER (W/ RETURN)
    public void AddListener<TResult>(Event eventName, Func<TResult> listener)
    {
        if (!eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName] = new List<Delegate>();
        }

        eventListeners[eventName].Add(listener);
    }

    public void RemoveListener<TResult>(Event eventName, Func<TResult> listener)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName].Remove(listener);
        }
    }

    public TResult TriggerEvent<TResult>(Event eventName)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            var listeners = eventListeners[eventName].ToArray();

            foreach (var listener in listeners)
            {
                if (listener is Func<TResult> function)
                {
                    return function.Invoke();
                }
            }
        }

        Debug.Log("returning default...");
        return default(TResult);
    }
    #endregion

    //similar to the above but allows passing of parameters w/ return of a value
    #region ADD/REMOVE/TRIGGER (W/ RETURN + PARAM)
    public void AddListener<TParam, TResult>(Event eventName, Func<TParam, TResult> listener)
    {
        if (!eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName] = new List<Delegate>();
        }

        eventListeners[eventName].Add(listener);
    }

    public void RemoveListener<TParam, TResult>(Event eventName, Func<TParam, TResult> listener)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName].Remove(listener);
        }
    }

    public TResult TriggerEvent<TParam, TResult>(Event eventName, TParam param)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            var listeners = eventListeners[eventName].ToArray();

            foreach (var listener in listeners)
            {
                if (listener is Func<TParam, TResult> function)
                {
                    return function.Invoke(param);
                }
            }
        }

        Debug.Log("returning default...");
        return default(TResult);
    }
    #endregion
}

//list of possible events
public enum Event
{
    PLAYER_XP_GAIN
}

