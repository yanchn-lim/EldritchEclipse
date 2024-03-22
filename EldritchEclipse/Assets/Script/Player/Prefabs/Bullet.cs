using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float maxTime = 5;
    float speed = 10f;

    private void Start()
    {
        StartCoroutine(Timer());
    }

    private void FixedUpdate()
    {
        transform.position += transform.up * Time.fixedDeltaTime * speed;
    }

    IEnumerator Timer()
    {
        float time = 0;
        float tick = 0.01f;
        while(time < maxTime)
        {
            time += tick;

            yield return new WaitForSeconds(tick);
        }

        Destroy(gameObject);
    }
}
