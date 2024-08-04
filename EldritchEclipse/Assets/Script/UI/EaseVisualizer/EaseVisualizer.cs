using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EasingFunctions;

public class EaseVisualizer : MonoBehaviour
{
    public GameObject p;
    int t = 31;
    List<GameObject> l = new();

    private void OnEnable()
    {
        StartCoroutine(a());
    }

    private void OnDisable()
    {
        foreach (var item in l)
        {
            Destroy(item);
        }

        l.Clear();
    }

    IEnumerator a()
    {
        for (int i = 0; i < t; i++)
        {
            EasingFunction f = (EasingFunction)i;
            var g = Instantiate(p, transform);
            l.Add(g);
            g.transform.localPosition = Vector3.zero;
            var gt = g.GetComponent<EaseVisualizerMinion>();
            gt.e = f;
            gt.Begin();
            yield return new WaitForSeconds(1f);
        }
    }

}
