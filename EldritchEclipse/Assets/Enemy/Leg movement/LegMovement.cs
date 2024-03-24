using Assets.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegMovement : MonoBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField] private float distanceOffset;
    [SerializeField] private float distanceOfRayCast;
    [SerializeField] private LayerMask maskTargeting;

    [Header("Walking")]
    [SerializeField] private float stepDistance;
    [SerializeField] private float stepHeight;
    [SerializeField] private float timeToStep;
    private float elapseTime;


    [Header("leg reference")]
    [SerializeField] private bool isLeft;
    [SerializeField] private BodyMover connector;
    [SerializeField] private LegMovement otherLeg;
    public bool isGrounded { get; private set; }

    [Header("Debugging")]
    [SerializeField] private float sizeOfSphere;
    private Vector3 rayCastingPosition;
    private Vector3 dummyTargetPosition;

    private Vector3 oldPosition;
    private Vector3 originalPosition;
    private Vector3 newPosition;

    private void Start()
    {
        oldPosition = transform.position;
        originalPosition = transform.position;
        newPosition = transform.position;
        elapseTime = float.PositiveInfinity;
        isGrounded = true;
    }

    private void Update()
    {
        transform.position = originalPosition;

        rayCastingPosition = body.position + (body.right * distanceOffset);

        Ray ray = new Ray(rayCastingPosition, Vector3.down);

        Debug.DrawRay(rayCastingPosition, Vector3.down * distanceOfRayCast, Color.yellow);

        if (Physics.Raycast(ray, out var hitinfo, distanceOfRayCast, maskTargeting))
        {
            dummyTargetPosition = hitinfo.point;
            if (Vector3.Distance(newPosition, hitinfo.point) >= stepDistance )
            {//meet the requirement to move
                newPosition = dummyTargetPosition;
                elapseTime = 0f;
            }
        }

        if(otherLeg.isGrounded && 
            (connector.leftLegStart == isLeft) &&
            isGrounded)
        {
            //is both legs are grounded and the connector say that the assign leg can start first
            isGrounded = false;
            connector.ToggleLeg(); //switch such that the next leg move
            //can start moving 
            
        }
        else if (isGrounded == false)
        {
            //is moving
            if (elapseTime < timeToStep)
            {
                float progress = elapseTime / timeToStep;

                Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, progress);
                footPosition.y += Mathf.Sin(progress * Mathf.PI) * stepHeight;

                originalPosition = footPosition;
                isGrounded = false;
                elapseTime += Time.deltaTime;
            }
            else
            {
                isGrounded = true;
                oldPosition = newPosition;
            }
        }
        else
        {
            return; //dont move
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rayCastingPosition, sizeOfSphere);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(dummyTargetPosition, sizeOfSphere);

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(originalPosition, sizeOfSphere);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(newPosition, sizeOfSphere);
    }


}
