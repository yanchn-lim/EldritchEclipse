using System.Collections;
using System.Net;
using Unity.Mathematics;
using UnityEngine;


namespace Movement
{
    /// <summary>
    /// script if want to implement bipedal/quadpeds monsters
    /// </summary>
    public class DualLegs : MonoBehaviour
    {
        [Header("legs")]
        [SerializeField] private LegMovement leftLeg;
        [SerializeField] private LegMovement rightLeg;
        private MovementManager movelooker;
        private Transform core;

        [Header("debugging")]
        [SerializeField] private Vector3 rotation = new Vector3(0,-90,0);
        private void Start()
        {
            core = transform.parent;
        }

        private void Update()
        {
            Vector3 rightVector = (leftLeg.transform.position - rightLeg.transform.position).normalized;
            Vector3 forwardVector = Quaternion.Euler(0, -90, 0) * rightVector;
            Vector3 upVector = Vector3.Cross(forwardVector, rightVector);

            Quaternion targetRotation = Quaternion.LookRotation(core.forward, upVector);

            transform.rotation = Quaternion.Slerp(transform.rotation,
                        targetRotation,
                        Time.deltaTime * movelooker.RotationSpeed);
        }


        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = Color.red;
        //    Vector3 midpoint = (leftLeg.transform.position + rightLeg.transform.position) / 2;
        //    Gizmos.DrawSphere(midpoint, 0.05f);
        //    Debug.DrawLine(leftLeg.transform.position, rightLeg.transform.position, Color.red);

        //    Vector3 rightVector = (leftLeg.transform.position - rightLeg.transform.position).normalized;
        //    Debug.DrawRay(midpoint, rightVector, Color.red);
        //    Vector3 forwardVector = Quaternion.Euler(rotation) * rightVector;
        //    Debug.DrawRay(midpoint, forwardVector, Color.blue);
        //    Vector3 upVector = Vector3.Cross(forwardVector,rightVector);
        //    Debug.DrawRay(midpoint, upVector, Color.green);

        //}
        public void SetUpLegs(
            LegMovement leftLeg,
            LegMovement rightLeg,
            MovementManager overLooker)
        {
            this.leftLeg = leftLeg;
            this.rightLeg = rightLeg;
            movelooker = overLooker;
        }

    }
}