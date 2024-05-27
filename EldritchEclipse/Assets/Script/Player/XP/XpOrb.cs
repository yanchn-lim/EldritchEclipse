using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpOrb : MonoBehaviour
{
    public float xpValue = 5;
    public float flySpeed = 1;
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
            transform.position += dir * flySpeed * Time.fixedDeltaTime;
            
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            EventManager.Instance.TriggerEvent(Event.PLAYER_XP_GAIN, xpValue);
            Destroy(gameObject);
        }
    }
}
