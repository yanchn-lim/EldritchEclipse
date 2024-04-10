using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float fixY;

    private void Update()
    {
        Vector3 vector3 = transform.position;
        vector3.y = fixY;
        transform.position = vector3;
    }
}
