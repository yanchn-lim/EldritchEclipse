using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [SerializeField]
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
        dir = player.position - transform.position;
        dir.y = 0;
        dir.Normalize();
        stat.Test();
    }

    private void FixedUpdate()
    {
        transform.position += dir * moveSpeed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player_Projectile"))
        {
            Debug.Log("AM HITTED");
        }
    }
}
