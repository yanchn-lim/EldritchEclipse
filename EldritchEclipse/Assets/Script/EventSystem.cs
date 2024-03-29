using System;
using System.Linq;
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
                    instance = new();
                }
                return instance;
            }
        }
        string DebugFormat = "EVENT SYSTEM : ";

        Dictionary<string, EventBase> EventList = new();

        // adding / removing
        public void AddEvent(string name)
        {
            if (EventList.ContainsKey(name))
            {
                Debug.LogError($"{DebugFormat}EVENT ALREADY IN LIST!");
                return;
            }

            if (!EventList.TryAdd(name, new(name)))
                Debug.LogError($"{DebugFormat}ERROR ADDING EVENT!");               
        }

        public void RemoveEvent(string name)
        {
            if (!EventList.ContainsKey(name))
            {
                Debug.LogError($"{DebugFormat}EVENT NOT IN LIST!");
                return;
            }

            if (!EventList.Remove(name))
                Debug.LogError($"{DebugFormat}ERROR REMOVING EVENT!");
        }


        //event handling
        public EventReturn TriggetEvent(string name)
        {
            var evnt = EventList[name];
            var data = evnt.Execute();

            return data;
        }

        public EventReturn TriggetEvent<T>(string name, EventArgs args)
        {
            var evnt = EventList[name];
            var returnData = evnt.Execute();

            return returnData;
        }
    }

    public struct EventArgs
    {
        public string Name { get; private set; }
        public Hashtable Payload;

        public void AddArgs(string name, object data)
        {
            Payload.Add(name, data);
        }

        public EventArgs(string name)
        {
            Name = name;
            Payload = new();
        }
    }

    public struct EventReturn
    {
        public Hashtable Payload;

        public void AddData(string name, object data)
        {
            Payload.Add(name,data);
        }
    }

    //Delegate, Action,Func
    public class EventBase
    {
        public string Name { get; private set; }
        Queue<Delegate> _listeners;

        public EventReturn Execute()
        {
            Debug.Log($"EVENT SYSTEM : EXECUTING \"EVENT_{Name}\"");

            //do check if event requires data

            //if Action, ask for args

            //if Func, ask for args + returns

            //if Predicate, ask for bool

            return default;
        }

        public EventBase(string name)
        {
            Name = name;
            _listeners = new();
        }
    }

    
}

