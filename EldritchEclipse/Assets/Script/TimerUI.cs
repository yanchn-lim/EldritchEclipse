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
        timerText.text = $"{min} : {sec}";    
    }
}
