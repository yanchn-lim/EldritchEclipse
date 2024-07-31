using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Hitbox : MonoBehaviour
{
    EnemyHandler handler;
    EventManager<EnemyEvents> em_e = EventSystem.Enemy;
    public void Initialize(EnemyHandler h)
    {
        handler = h;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player_Projectile"))
        {
            float dmg = em_e.TriggerEvent<float>(EnemyEvents.TAKE_DAMAGE);
            handler.GetHit(dmg);
        }
    }
}
