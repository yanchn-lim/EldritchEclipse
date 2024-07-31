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

    #region TIME
    void StartTimer()
    {
        GameVariables.ElapsedTime = 0;
        GameVariables.RemainingTime = GameVariables.GameTime;
        StartCoroutine(GameTimer());
    }

    IEnumerator GameTimer()
    {
        while(GameVariables.RemainingTime > 0)
        {
            GameVariables.RemainingTime -= GameVariables.TimeTick;
            GameVariables.ElapsedTime += GameVariables.TimeTick;
            yield return new WaitForSeconds(GameVariables.TimeTick);
        }

        Debug.Log("OUT OF TIME");
    }

    #endregion
}
