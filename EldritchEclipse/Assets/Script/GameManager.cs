using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    float timeTick;
    float maxTime = 1200;
    public static float Timer;

    private void Start()
    {
        GameStart();
    }

    void GameStart()
    {
        StartTimer();
    }

    void StartTimer()
    {
        Timer = maxTime;
        StartCoroutine(GameTimer());
    }

    IEnumerator GameTimer()
    {
        while(Timer > 0)
        {
            Timer -= timeTick;
            yield return new WaitForSeconds(timeTick);
        }

        Debug.Log("OUT OF TIME");
    }
}
