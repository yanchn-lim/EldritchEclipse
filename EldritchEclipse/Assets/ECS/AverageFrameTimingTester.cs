using System.Collections;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class AverageFrameTimingTester : MonoBehaviour
{
    public int frameCount = 100;
    public int testCount = 5;
    int counter = 0;
    float t = 0;
    int f = 0;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightBracket)){
            StartCoroutine(Timer());
        }
    }

    IEnumerator Timer()
    {
        print("Timer started...");

        for (int i = 0; i < testCount; i++)
        {
            float total = 0;
            for (int j = 0; j < frameCount; j++)
            {
                total += Time.deltaTime;
                yield return null;
            }
            float frameTime = total / frameCount * 1000;
            t += frameTime;
            int fps = (int)(1 / (total / frameCount));
            f += fps;
            counter++;
            print($"Frame Timing : {frameTime}ms \nFPS : {fps}\nAverage Timing : {t / counter}ms : {f / counter}fps / {counter} Tests");
        }

        print($"End of {counter} Tests\nAverage Timing : {t / counter}ms / {f / counter}fps");
    }
}
