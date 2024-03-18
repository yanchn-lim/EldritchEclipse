using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text timerText;

    // Update is called once per frame
    void Update()
    {
        float time = GameManager.Timer;
        int min = Mathf.FloorToInt(time / 60);
        int sec = Mathf.FloorToInt(time % 60);

        string minString = min >= 10 ? min.ToString() : $"0{min}";
        string secString = sec >= 10 ? sec.ToString() : $"0{sec}";
        timerText.text = $"{minString} : {secString}";    
    }
}
