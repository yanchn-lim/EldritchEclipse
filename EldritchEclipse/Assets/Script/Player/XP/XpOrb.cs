using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpOrb : MonoBehaviour
{
    public float xpValue;
    public float flySpeed = 1;

    public void ChangeValue(float val)
    {
        xpValue = val;
    }

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
            EventSystem.Player.TriggerEvent(PlayerEvents.PLAYER_XP_GAIN, xpValue);
            Destroy(gameObject);
        }
    }
}
