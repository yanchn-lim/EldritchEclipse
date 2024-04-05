using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpOrb : MonoBehaviour
{

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
            transform.position += dir * 2f * Time.fixedDeltaTime;
            yield return null;
        }
    }
}
