//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using YC;
//using System;
//public class TestEventSystem : MonoBehaviour
//{
//    EventSystem es = EventSystem.Instance;

//    private void Start()
//    {
//        Action a = new(TestMethod);
//        Action<string> a2 = new(TestMethod2);
        
//        es.CreateEvent("TestEvent");
//        es.AddDelegates("TestEvent", a);       
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.E))
//        {
//            es.TriggetEvent("TestEvent");
//        }
//    }

//    void TestMethod()
//    {
//        Debug.Log("TestMethod_1");
//    }

//    void TestMethod2(string t)
//    {
//        Debug.Log($"TestMethod_2 : {t}");
//    }

//    void TestMethod2(string a,string b,string c)
//    {
//        Debug.Log($"TestMethod_2 : {a + b + c}");
//    }

//    string TestMethod3()
//    {
//        Debug.Log("TestMethod_3");
//        return "TestMethod_3";
//    }

//    string TestMethod4(string a)
//    {
//        Debug.Log($"TestMethod_4 : {a}");
//        return "TestMethod_4";
//    }

//    string TestMethod5(string a,string b,string c)
//    {
//        Debug.Log($"TestMethod_5 : {a + b + c}");
//        return "TestMethod_5";
//    }
//}
