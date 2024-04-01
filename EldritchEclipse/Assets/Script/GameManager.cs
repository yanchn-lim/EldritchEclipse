using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        GameStart();
    }

    private void Update()
    {
        InputHandler.Update();
    }

    void GameStart()
    {
        StartTimer();
    }

    void StartTimer()
    {
        GameVariables.CurrentTime = GameVariables.GameTime;
        StartCoroutine(GameTimer());
    }

    IEnumerator GameTimer()
    {
        while(GameVariables.CurrentTime > 0)
        {
            GameVariables.CurrentTime -= GameVariables.TimeTick;
            yield return new WaitForSeconds(GameVariables.TimeTick);
        }

        Debug.Log("OUT OF TIME");
    }
}
