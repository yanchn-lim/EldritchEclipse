using System.Collections;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class AverageFrameTimingTester : MonoBehaviour
{
    public int frameCount = 100;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightBracket)){
            StartCoroutine(Timer());
        }
    }

    IEnumerator Timer(){
        float total = 0;
        print("Timer started...");
        for (int i = 0; i < frameCount; i++)
        {
            total += Time.deltaTime;
            yield return null;
        }

        print($"Frame Timing : {total / frameCount * 1000}ms \nFPS : {1 / (total / frameCount)}");
    }
}
