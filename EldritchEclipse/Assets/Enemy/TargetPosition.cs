using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPosition : MonoBehaviour
{
    //transform position is the starting position
    //ray cast below
    public Vector3 targetPosition { get; private set; }
    [SerializeField] private float sizeOfSphere;
    [SerializeField] private float distanceOfRayCast;
    [SerializeField] private LayerMask maskTargeting;

    private void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(transform.position, Vector3.down * distanceOfRayCast , Color.yellow);

        if(Physics.Raycast(ray, out var hitinfo, distanceOfRayCast , maskTargeting))
        {
            targetPosition = hitinfo.point;
        }
        else
        {
            Debug.LogError("cant hit anything");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPosition, sizeOfSphere);
    }

}
