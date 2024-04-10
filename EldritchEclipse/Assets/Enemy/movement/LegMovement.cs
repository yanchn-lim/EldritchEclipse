using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class LegMovement : MonoBehaviour
    {
        [SerializeField] private Transform jointConnection;
        private Transform core;
        [SerializeField] private float horizontalDistanceOffset;
        [SerializeField] private float verticalDistanceOffset;
        [SerializeField] private float distanceOfRayCast = 2f;
        [SerializeField] private LayerMask maskTargeting;

        [Header("Walking")]
        [SerializeField] private float stepDistance = 0.32f;
        [SerializeField] private float stepHeight = 0.4f;
        [SerializeField] private float speedToStep = 0.12f;

        [Header("AngleRotation")]
        [SerializeField] private float ankleRotationSpeed = 1;
        //for dual legs

        //will determine if the legs can be move
        public bool IsGrounded { get; private set; }
        public bool CanMove { get; private set; }
        private Vector3 newPosition;

        [Header("Debugging")]
        [SerializeField] private float sizeOfSphere;
        private Vector3 rayCastingPosition;
        private Vector3 dummyTargetPosition;
        private Vector3 CurrentPosition;
        private string legName;
        

        private void Start()
        {
            CurrentPosition = transform.position;
            newPosition = transform.position;
            IsGrounded = true;
            //find the distance offset
            horizontalDistanceOffset = transform.position.x - jointConnection.position.x;
            core = jointConnection.parent;
            legName = transform.parent.parent.parent.parent.parent.name+ " " + transform.parent.parent.parent.name;
        }

        private void Update()
        {
            transform.position = CurrentPosition;

            //if (IsGrounded && transform.position != CurrentPosition)
            //{
            //    Debug.LogError($"{legName} position is not right.\n " +
            //        $"Original Position: {CurrentPosition}\n" +
            //        $"Transform position: {transform.position}");
            //}

            //constantly update the where the ray is
            //                   The Starting Point            How much right is added 
            rayCastingPosition = jointConnection.position +
                (core.right * horizontalDistanceOffset) +
                (jointConnection.forward * verticalDistanceOffset);

            Ray ray = new Ray(rayCastingPosition, Vector3.down);

            Debug.DrawRay(rayCastingPosition, Vector3.down * distanceOfRayCast, Color.yellow);
            if (Physics.Raycast(ray, out var hitinfo, distanceOfRayCast, maskTargeting))
            {
                dummyTargetPosition = hitinfo.point;
                //check the distance and see if the new position can be landed
                if (Vector3.Distance(transform.position, hitinfo.point) >= stepDistance)
                {
                    if (IsGrounded)
                    {
                        CanMove = true;
                        newPosition = dummyTargetPosition;
                        return;
                    }
                }
            }
            //cannot move at all
            CanMove = false;


            //for the ankle rotation
            AnkleAdjustment();

            void AnkleAdjustment()
            {
                if (!IsGrounded) return;
                Ray ray = new Ray(transform.position, Vector3.down);
                if (Physics.Raycast(ray, out var hit, distanceOfRayCast, maskTargeting))
                {
                    //find the angle to rotate at
                    //Quaternion targetRotation = Quaternion.LookRotation(hit.normal, transform.forward);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                    //    ankleRotationSpeed * Time.deltaTime);
                    Debug.DrawRay(transform.position, hit.normal * 2, Color.black);
                    if(hit.normal != Vector3.up)
                    {
                        transform.up = Vector3.Slerp(transform.up, hit.normal, ankleRotationSpeed * Time.deltaTime);
                    }
                    else
                    {
                        transform.rotation = Quaternion.identity;
                    }

                }
            }

        }


        public void MoveLeg()
        {
            StopAllCoroutines();
            StartCoroutine(MoveLegCoroutine());
        }

        //for for instructing the leg to move
        private IEnumerator MoveLegCoroutine()
        {
            IsGrounded = false; //it is now moving so the entire leg component cant be move
            float elapseTime = 0f;
            Vector3 targetSpot = newPosition;
            Vector3 oldPosition = CurrentPosition;
            while (elapseTime < 1)
            {
                float progress = elapseTime / 1;
                Vector3 footPosition = Vector3.Lerp(oldPosition, targetSpot, progress);
                footPosition.y += Mathf.Sin(progress * Mathf.PI) * stepHeight;
                CurrentPosition = footPosition;
                elapseTime += Time.deltaTime * speedToStep;
                yield return null;
            }
            CurrentPosition = targetSpot;
            IsGrounded = true;// now the leg is ready to take the next step
        }

        //debugging purpose
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rayCastingPosition, sizeOfSphere);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(dummyTargetPosition, sizeOfSphere);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(newPosition, sizeOfSphere);
        }

        //public void SetUpConnector(DualLegs bodyMover)
        //{
        //    connector = bodyMover;
        //}

        public void JointConnection(Transform joint)
        {
            jointConnection = joint;
        }

    }
}