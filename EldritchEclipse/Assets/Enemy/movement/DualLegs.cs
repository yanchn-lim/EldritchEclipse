using System.Collections;
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

        private void Start()
        {
            core = transform.parent;
        }

        private void Update()
        {
            //fix this later (this is where the connection joint follows the legs)

            //Vector3 targetDirectionToFace = (leftLeg.transform.position - rightLeg.transform.position).normalized;
            //targetDirectionToFace = Vector3.Cross(targetDirectionToFace, Vector3.up);
            //float angle = Vector3.Angle(core.forward, targetDirectionToFace);
            //targetDirectionToFace =  Quaternion.AngleAxis(angle * 2 , Vector3.up) * targetDirectionToFace;

            //print($"Target direction {transform.name} {targetDirectionToFace}");
            //Debug.DrawRay(transform.position, targetDirectionToFace * 5, Color.green);


            //transform.rotation = Quaternion.LookRotation(targetDirectionToFace * Time.deltaTime * movelooker.RotationSpeed, Vector3.up);
            
        
        }

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