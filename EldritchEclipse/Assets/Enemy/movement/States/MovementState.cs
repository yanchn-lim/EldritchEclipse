using PGGE.Patterns;
using System.Collections;
using UnityEngine;

namespace Movement
{
    public class MovementState : FSMState
    {
        protected LegMovement leftUpperLeg;
        protected LegMovement leftLowerLeg;
        protected LegMovement rightUpperLeg;
        protected LegMovement rightLowerLeg;
        protected MovementManager movementManager;
        public MovementState(FSM fsm, 
            int id , 
            LegMovement LUL,
            LegMovement LLL,
            LegMovement RUL,
            LegMovement RLL,
            MovementManager movementManager
            ) : base(fsm, id)
        {
            leftUpperLeg = LUL;
            leftLowerLeg = LLL;
            rightUpperLeg = RUL;
            rightLowerLeg = RLL;
            this.movementManager = movementManager;
        }
    }

    public class WalkingState : MovementState
    {
        private bool LeftLegStart;
        private float elapseTime;

        public WalkingState(FSM fsm, int id, LegMovement LUL, LegMovement LLL, LegMovement RUL, LegMovement RLL, MovementManager movementManager) : base(fsm, id, LUL, LLL, RUL, RLL, movementManager)
        {
        }

        public override void Enter()
        {
            elapseTime = 0;
        }

        public override void Update()
        {
            //Debug.Log($"LUL: {leftUpperLeg.CanMove} \n" +
            //    $"LLL: {leftLowerLeg.CanMove} \n" +
            //    $"RUL: {rightUpperLeg.CanMove} \n" +
            //    $"RLL: {rightLowerLeg.CanMove} \n" +
            //    $"lEFT LEG START {LeftLegStart}");

            while( elapseTime < movementManager.WalkingPauseTime)
            {
                elapseTime += Time.deltaTime;
                return;
            }

            if( LeftLegStart)
            {
                if (rightUpperLeg.IsGrounded && leftLowerLeg.IsGrounded)
                    MoveLeftFrontLeg();
            }
            else
            {
                if(leftUpperLeg.IsGrounded && rightLowerLeg.IsGrounded)
                {
                    MoveRightFrontLeg();
                }
            }
        }

        private void MoveLeftFrontLeg()
        {
            if(leftUpperLeg.CanMove || rightLowerLeg.CanMove) elapseTime = 0f; 

            if (leftUpperLeg.CanMove && rightLowerLeg.CanMove)
            {
                LeftLegStart = false;
            }
            if (leftUpperLeg.CanMove)
            {
                leftUpperLeg.MoveLeg();
            }
            if (rightLowerLeg.CanMove)
            {
                rightLowerLeg.MoveLeg();
            }
        }

        private void MoveRightFrontLeg()
        {
            if (rightUpperLeg.CanMove || leftLowerLeg.CanMove) elapseTime = 0f;

            if (leftUpperLeg.CanMove && rightLowerLeg.CanMove)
            {
                //next leg to move is the left leg
                LeftLegStart = true;
            }
            if (rightUpperLeg.CanMove)
            {
                rightUpperLeg.MoveLeg();
            }
            if (leftLowerLeg.CanMove)
            {
                leftLowerLeg.MoveLeg();
            }
        }
    }
}