using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float fixY;

    private void Update()
    {
        if(Physics.Raycast(transform.position, Vector3.down,out var hit))
        {
            var newPos = transform.position;
            newPos.y = hit.point.y + fixY;
            transform.position = newPos;
        }
    }
}
