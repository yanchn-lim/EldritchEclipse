using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    public Transform player;
    public Vector3 dir;

    public void Initialize(Transform p)
    {
        player = p;
    }

    public void MoveTowardsPlayer(float moveSpeed)
    {
        dir = player.position - transform.position;
        dir.y = 0;
        dir.Normalize();

        transform.position += dir * moveSpeed * Time.fixedDeltaTime;
    }
}
