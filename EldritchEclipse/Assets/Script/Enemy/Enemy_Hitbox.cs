using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Hitbox : MonoBehaviour
{
    EnemyHandler handler;

    public void Initialize(EnemyHandler h)
    {
        handler = h;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player_Projectile"))
        {
            handler.GetHit();
        }
    }
}
