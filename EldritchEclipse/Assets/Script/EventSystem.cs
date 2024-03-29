using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC
{
    public class EventSystem
    {
        private static EventSystem instance;

        public static EventSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ();
                }
                return instance;
            }
        }


        Dictionary<string, EventBase> EventList = new();

        public void AddEvent(string name)
        {
            if (!EventList.TryAdd(name, new(name)))
                Debug.LogError("EVENT SYSTEM : ERROR ADDING EVENT");               
        }

        public void RemoveEvent(string name)
        {
            if (!EventList.Remove(name))
                Debug.LogError("EVENT SYSTEM : ERROR REMOVING EVENT");
        }

        //do event handling here
    }

    //Delegate, Action,Func
    public class EventBase
    {
        public string Name { get; private set; }
        Queue<Delegate> _listeners;

        public void Execute()
        {
            Debug.Log($"EVENT SYSTEM : EXECUTING \"EVENT_{Name}\"");
        }

        public EventBase(string name)
        {
            Name = name;
            _listeners = new();
        }
    }

    
}

