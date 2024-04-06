using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpOrb : MonoBehaviour
{

    public float xpValue = 5;

    public void FlyToPlayer(Transform player)
    {
        StartCoroutine(FlyTowards(player));
    }

    IEnumerator FlyTowards(Transform player)
    {
        while (true)
        {
            Vector3 dir = player.position - transform.position;
            dir.Normalize();
            transform.position += dir * 1f * Time.fixedDeltaTime;
            
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerStats s = other.GetComponentInParent<PlayerStats>();
            s.GainXP(5);
            Destroy(gameObject);
        }
    }
}
