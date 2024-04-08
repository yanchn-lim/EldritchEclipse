using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    EnemyStat stat;
    Transform player;
    float moveSpeed = 3;
    Vector3 dir;
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //move towards player
        dir = transform.position - player.position;
        dir.y = 0;
        dir.Normalize();
    }

    private void FixedUpdate()
    {
        transform.position += dir * moveSpeed * Time.fixedDeltaTime;
    }
}
