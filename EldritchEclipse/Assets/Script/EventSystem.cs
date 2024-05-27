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
        string _debugFormat = "EVENT SYSTEM : ";

        Dictionary<string, EventBase> EventList = new();
        
        // adding / removing
        public void CreateEvent(string name)
        {
            if (EventList.ContainsKey(name))
            {
                Debug.LogError($"{_debugFormat}EVENT ALREADY IN LIST!");
                return;
            }

            if (!EventList.TryAdd(name, new(name)))
                Debug.LogError($"{_debugFormat}ERROR ADDING EVENT!");               
        }

        public void RemoveEvent(string name)
        {
            if (!EventList.ContainsKey(name))
            {
                Debug.LogError($"{_debugFormat}EVENT NOT IN LIST!");
                return;
            }

            if (!EventList.Remove(name))
                Debug.LogError($"{_debugFormat}ERROR REMOVING EVENT!");
        }


        public void AddDelegates(string eventName, Delegate del)
        {
            if (!EventList.ContainsKey(eventName))
            {
                Debug.LogWarning($"{_debugFormat}EVENT DOES NOT EXIST!\nCREATING A NEW {eventName} EVENT...");
                CreateEvent(eventName);
            }

            EventList[eventName].AddListeners(del);
        }

        //event handling
        public EventReturn TriggetEvent(string name)
        {
            var evnt = EventList[name];
            var data = evnt.Execute();

            return data;
        }

        //public EventReturn TriggetEvent(string name, EventArgs args)
        //{
        //    var evnt = EventList[name];
        //    var returnData = evnt.Execute();

        //    return returnData;
        //}
    }

    /// <summary>
    /// Contains all the arguments needed to execute the event
    /// </summary>
    public class EventArgs
    {
        public string Name { get; private set; }
        public Dictionary<string,object> Payload;

        public void AddArgs(string name, object data)
        {
            Payload.Add(name, data);
        }

        public EventArgs(string EventName)
        {
            Name = EventName + "_ARGSA";
            Payload = new();
        }
    }

    /// <summary>
    /// Container for all the data being returned from the event
    /// </summary>
    public class EventReturn
    {
        public Dictionary<string,object> Payload = new();

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
        List<Delegate> test;

        public void AddListeners(Delegate del)
        {
            test.Add(del);
        }

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

