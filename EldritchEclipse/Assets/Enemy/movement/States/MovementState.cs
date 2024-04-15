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
            int id,
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

        public WalkingState(FSM fsm, int id, LegMovement LUL, LegMovement LLL, LegMovement RUL, LegMovement RLL, MovementManager movementManager) : base(fsm, id, LUL, LLL, RUL, RLL, movementManager) { }

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

            while (elapseTime < movementManager.WalkingPauseTime)
            {
                elapseTime += Time.deltaTime;
                return;
            }

            if (LeftLegStart)
            {
                if (rightUpperLeg.IsGrounded && leftLowerLeg.IsGrounded)
                    MoveLeftFrontLeg();
            }
            else
            {
                if (leftUpperLeg.IsGrounded && rightLowerLeg.IsGrounded)
                {
                    MoveRightFrontLeg();
                }
            }
        }

        private void MoveLeftFrontLeg()
        {
            if (leftUpperLeg.CanMove || rightLowerLeg.CanMove) elapseTime = 0f;

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

    public class RunningState : MovementState
    {
        float elapseTime;
        bool frontLegStart;
        public RunningState(FSM fsm, int id, LegMovement LUL, LegMovement LLL, LegMovement RUL, LegMovement RLL, MovementManager movementManager) : base(fsm, id, LUL, LLL, RUL, RLL, movementManager)
        {
        }

        public override void Enter()
        {
            elapseTime = 0f;
        }

        public override void Update()
        {
            Debug.Log($"LUL: CanMove {leftUpperLeg.CanMove}, IsGround {leftUpperLeg.IsGrounded} \n" +
                $"LLL: CanMove {leftLowerLeg.CanMove}, , IsGround {leftLowerLeg.IsGrounded} \n" +
                $"RUL: CanMove {rightUpperLeg.CanMove}, , IsGround {rightUpperLeg.IsGrounded} \n" +
                $"RLL: CanMove {rightLowerLeg.CanMove}, , IsGround {rightLowerLeg.IsGrounded} \n" +
                $"Front leg start {frontLegStart}");

            while (elapseTime < movementManager.WalkingPauseTime)
            {
                elapseTime += Time.deltaTime;
                return;
            }

            if (frontLegStart)
            {
                if (leftLowerLeg.IsGrounded && rightLowerLeg.IsGrounded)
                    MoveFrontLeg();
            }
            else
            {
                if (leftUpperLeg.IsGrounded && rightUpperLeg.IsGrounded)
                    MoveLowerLeg();
            }
        }

        private void MoveLowerLeg()
        {
            if (leftLowerLeg.CanMove && rightLowerLeg.CanMove)
            {
                //next leg to move is the upper half
                frontLegStart = true;
                elapseTime = 0f;
            }
            if (leftLowerLeg.CanMove)
            {
                leftLowerLeg.MoveLeg();
            }
            if (rightLowerLeg.CanMove)
            {
                rightLowerLeg.MoveLeg();
            }
        }

        private void MoveFrontLeg()
        {
            if (leftUpperLeg.CanMove && rightUpperLeg.CanMove)
            {
                //next leg to move is the lower half
                frontLegStart = false;
                elapseTime = 0f;
            }

            if (rightUpperLeg.CanMove)
            {
                rightUpperLeg.MoveLeg();
            }
            if (leftUpperLeg.CanMove)
            {
                leftUpperLeg.MoveLeg();
            }
        }
    }
}