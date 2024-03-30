using System.Collections;
using UnityEngine;


namespace BodyPartLogic
{
    /// <summary>
    /// script if want to implement bipedal/quadpeds monsters
    /// </summary>
    public class DualLegs : MonoBehaviour
    {
        [SerializeField]private bool leftLegStart;

        [Header("legs")]
        [SerializeField] private LegMovement leftLeg;
        [SerializeField] private LegMovement rightLeg;


        public void RemoveLegs()
        {
            leftLeg.DismentalConnection();
            rightLeg.DismentalConnection();
            leftLeg = null;
            rightLeg = null;
        }

        public void SetUpLegs(LegMovement leftLeg, LegMovement rightLeg)
        {
            this.leftLeg = leftLeg;
            this.rightLeg = rightLeg;

            leftLeg.SetUpConnector(this);
            rightLeg.SetUpConnector(this);
        }

        public void ToggleLeg()
        {
            leftLegStart = !leftLegStart;
        }

        public LegMovement GetOtherLeg(LegMovement currentLeg)
        {
            if(currentLeg == rightLeg)
            {//if the one requesting is the right leg, return left leg
                return leftLeg;
            }
            else 
            { 
                return rightLeg;
            }
        }

        public bool CanMoveLeg(LegMovement currentLeg)
        {
            if(leftLeg == currentLeg)
            {
                return leftLegStart == true;
            }
            else
            {
                return leftLegStart == false;
            }
        }
    }
}