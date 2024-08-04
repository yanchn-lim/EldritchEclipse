using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TestAsync : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await t1();
        await t2();
    }

    async Task t1()
    {
        await Task.Delay(1000);
        Debug.Log("t1");
    }
    
    async Task t2()
    {
        await Task.Delay(2000);
        Debug.Log("t2");
    }

}
