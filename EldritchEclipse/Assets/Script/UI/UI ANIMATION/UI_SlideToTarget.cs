using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SlideToTarget : MonoBehaviour
{
    float tick = GameVariables.TimeTick;
    bool reachedTarget;
    Vector3 originalPos;

    [SerializeField,Range(0.01f,5f)]
    float speed;

    void OnEnable()
    {
        originalPos = transform.localPosition;
        StartCoroutine(Animation());
    }

    IEnumerator Animation()
    {
        transform.localPosition = new(-2000, originalPos.y, originalPos.z);
        float time = 0;
        yield return new WaitForSeconds(1f);

        while (time <= 1)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, time);
            time += tick * speed;
            yield return new WaitForSeconds(tick);
        }
        transform.localPosition = originalPos;
        reachedTarget = true;      
    }
}
