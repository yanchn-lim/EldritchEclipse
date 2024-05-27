using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPDetection : MonoBehaviour
{
    SphereCollider detection;
    float baseRange = 5;
    float rangeMultiplier = 1;

    private void Start()
    {
        detection = GetComponent<SphereCollider>();
    }

    void ChangeRange()
    {
        detection.radius = baseRange * rangeMultiplier;
    }

    private void OnTriggerEnter(Collider other)
    {    
        if (other.GetComponent<XpOrb>())
        {
            other.GetComponent<XpOrb>().FlyToPlayer(transform);
        }
    }
}
