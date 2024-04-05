using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestXP : MonoBehaviour
{
    public SphereCollider s;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + s.center, s.radius);
    }


}
