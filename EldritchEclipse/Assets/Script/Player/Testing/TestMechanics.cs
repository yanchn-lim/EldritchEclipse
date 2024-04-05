using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMechanics : MonoBehaviour
{
    PlayerStats stat;

    private void Start()
    {
        stat = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            stat.GainXP(5);
        }
    }
}
