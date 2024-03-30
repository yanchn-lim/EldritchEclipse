using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BodyPartLogic
{
    public class LegMovement : MonoBehaviour
    {
        [SerializeField] private Transform jointConnection;
        [SerializeField] private float distanceOffset;
        [SerializeField] private float distanceOfRayCast = 2f;
        [SerializeField] private LayerMask maskTargeting;

        [Header("Walking")]
        [SerializeField] private float stepDistance = 0.32f;
        [SerializeField] private float stepHeight = 0.4f;
        [SerializeField] private float speedToStep = 0.12f;

        private float elapseTime;
        //for dual legs
        private DualLegs connector;

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

            //find the distance offset
            distanceOffset =  transform.position.x - jointConnection.position.x ;
            
        }

        private void Update()
        {
            transform.position = originalPosition;

            rayCastingPosition = jointConnection.position + jointConnection.right * distanceOffset;

            Ray ray = new Ray(rayCastingPosition, Vector3.down);

            Debug.DrawRay(rayCastingPosition, Vector3.down * distanceOfRayCast, Color.yellow);

            if (Physics.Raycast(ray, out var hitinfo, distanceOfRayCast, maskTargeting))
            {
                dummyTargetPosition = hitinfo.point;
                if (Vector3.Distance(newPosition, hitinfo.point) >= stepDistance)
                {//meet the requirement to move
                    newPosition = dummyTargetPosition;
                    elapseTime = 0f;
                }
            }
            if (connector != null)
            {
                MoveForTwoLegs();
            }
            else
            {
                MoveLeg();
            }
        }

        private void MoveForTwoLegs()
        {
            if (connector.GetOtherLeg(this).isGrounded &&
                        connector.CanMoveLeg(this) &&
                        isGrounded)
            {
                //is both legs are grounded and the connector say that the assign leg can start first
                isGrounded = false;
                connector.ToggleLeg(); //switch such that the next leg move
                                       //can start moving 

            }
            else if (isGrounded == false)
            {
                MoveLeg();
            }
            else
            {
                return; //dont move
            }
        }

        private void MoveLeg()
        {
            //is moving
            if (elapseTime < 1)
            {
                float progress = elapseTime / 1;

                Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, progress);
                footPosition.y += Mathf.Sin(progress * Mathf.PI) * stepHeight;

                originalPosition = footPosition;
                isGrounded = false;
                elapseTime += Time.deltaTime * speedToStep;
            }
            else
            {
                isGrounded = true;
                oldPosition = newPosition;
            }
        }

        //debugging purpose
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

        public void SetUpConnector(DualLegs bodyMover)
        {
            connector = bodyMover;
        }

        public void JointConnection(Transform joint)
        {
            jointConnection = joint;
        }

        public void DismentalConnection()
        {
            
            connector = null;
        }

        private void OnDestroy()
        {
            connector.RemoveLegs();
        }

        
    }
}