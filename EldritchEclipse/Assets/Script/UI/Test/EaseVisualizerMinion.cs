using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseVisualizerMinion : MonoBehaviour
{
    public EasingFunctions.EasingFunction e;
    
    public void Begin()
    {
        StartCoroutine(a());
    }

    IEnumerator a()
    {
        float time = 0;
        Vector3 startPos = transform.localPosition;
        while (time < 1)
        {
            float x = Mathf.Lerp(startPos.x, 10, EasingFunctions.Ease(e, time));
            float y = Mathf.Lerp(startPos.y, 10, time);
            //transform.localPosition = Vector3.Lerp(startPos, new(10, 10, 0), EasingFunctions.Ease(e, time));
            transform.localPosition = new(x, y, 0);
            time += 0.01f / 5f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
