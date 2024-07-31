using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamagePopUpAnimation : MonoBehaviour
{
    public void BeginAnimation(float damage)
    {
        StartCoroutine(Animate(damage));

    }


    IEnumerator Animate(float dmg)
    {
        float timer = 0;
        float time = 3f;
        var tmp = GetComponent<TMP_Text>();
        Color c = tmp.color;
        tmp.text = dmg.ToString();
        while(timer < 3f)
        {
            Vector3 pos = transform.localPosition;
            pos.y += 0.04f;
            transform.localPosition = pos;
            c.a = Mathf.Lerp(1,0, timer/time);
            tmp.color = c;
            timer += 0.02f;
            yield return new WaitForSeconds(0.02f);
        }

        Destroy(gameObject);
    }
}
