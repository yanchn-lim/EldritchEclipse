using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [SerializeField]
    EnemyStat_SO stat_SO;
    Transform player;
    float moveSpeed = 3;
    Vector3 dir;

    #region MONOBEHAVIOUR
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
            GetHit();
        }
    }
    #endregion

    void InitializeEnemy()
    {

    }

    void GetHit()
    {
    }
}
