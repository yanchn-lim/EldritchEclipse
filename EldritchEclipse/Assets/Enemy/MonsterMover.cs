using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterMover : MonoBehaviour
{
    [SerializeField] private Transform body;

    [Tooltip("quadruped legs")]
    [SerializeField] private Transform leftFrontLeg;
    [SerializeField] private Transform rightFrontLeg;
    [SerializeField] private Transform leftHindLeg;
    [SerializeField] private Transform rightHindLeg;
    [SerializeField] private float durationToMoveLeg;
    [SerializeField] private LegMovingState currentLegState;
    [Tooltip("Clamp the value of the movement to make sure that the leg movement is within the certain range")]
    [SerializeField] private float movementClampValue;
    [Tooltip("Testing")]
    [SerializeField] private Transform targetPosition;

    private LegPiece leftFrontLegPiece;
    private LegPiece rightFrontLegPiece;
    private LegPiece leftHindLegPiece;
    private LegPiece rightHindLegPiece;

    private LegPiece[] legUsed = new LegPiece[2];
    private LegPiece[] legNotUsed = new LegPiece[2];

    private Vector3 bodyTargetDirection;

    private void Start()
    {
        InitLegs();
        SettingLegMovement();
    }

    private void Update()
    {
        bool canSwitchLeg = legUsed[0].isGrounded && legUsed[1].isGrounded;
        if(canSwitchLeg)
        {
            SettingLegMovement();
        }
        else
        {//root the two leg to world position
            RootLegs();
        }
    }

    private void RootLegs()
    {
        for(int i = 0 ; i < legNotUsed.Length; i++)
        {
            //make sure it is rooted in world space;
            legNotUsed[i].leg.position = legNotUsed[i].originalPosition;
        }
    }

    private void SettingLegMovement()
    {
        switch (currentLegState)
        {
            case (LegMovingState.LeftLeg):
                legUsed[0] = leftFrontLegPiece;
                legUsed[1] = rightHindLegPiece;

                legNotUsed[0] = leftHindLegPiece;
                legNotUsed[1] = rightHindLegPiece;

                //make sure that the leg moving is the right leg
                currentLegState = LegMovingState.RightLeg;
                break;
            case (LegMovingState.RightLeg):
                legUsed[0] = rightFrontLegPiece;
                legUsed[1] = leftHindLegPiece;

                legNotUsed[0] = rightHindLegPiece;
                legNotUsed[1] = leftFrontLegPiece;

                //make sure that the next leg would be the left leg
                currentLegState = LegMovingState.LeftLeg;
                break;
        }

        //prepare them to move
        legUsed[0].isGrounded = false;
        legUsed[1].isGrounded = false;

        //calculate the new leg position



        Vector2 targetPosition = new Vector2(this.targetPosition.position.x, this.targetPosition.position.z);
        //raycast the leg to find new position
        for(int i = 0 ; i < legUsed.Length; i++)
        {
            var leg = legUsed[i];
            //Vector3 targetPoint = targetPosition.position - leg.originalPosition;
            Vector2 originalPosition = new Vector2(leg.originalPosition.x, leg.originalPosition.z);
            //targetPoint = Vector3.ClampMagnitude(targetPoint, movementClampValue) + leg.originalPosition;
            Vector2 newPointPosition = Vector2.ClampMagnitude(((targetPosition - originalPosition))
                , movementClampValue) //get the target direction and clamp it
                + originalPosition;//then add the original position to get new position

            //do raycast
            Ray ray = new Ray(newPointPosition, Vector3.down);
            Debug.DrawRay(newPointPosition, Vector3.down * 10f, Color.yellow, 2f);
            //10f is just a random value so go ahead and change that

            Vector3 newPostion = Vector3.zero;

            if (Physics.Raycast(ray,out var hit, 10f)) 
            {
                newPostion = hit.point;
            }
            else
            {
                Debug.LogError("not target!!! please make sure that is a ground for the raycast to hit!");
            }

            //target direction is where the leg should be moving 
            //have not account for edge case so add that in later

            //set new position for it to move to
            legUsed[i].newPosition = newPostion;
        }

        //find the target direction of the body;

        FindBodyTargetDirection();
        //start coroutine to begin movement

        StartCoroutine(MoveLegs());
    }

    private void FindBodyTargetDirection()
    {
        Vector3 targetDirction = targetPosition.position - body.position;
        targetDirction.y = body.position.y;
        targetDirction = Vector3.ClampMagnitude(targetDirction, movementClampValue);

        bodyTargetDirection = targetDirction + body.position;
    }

    private void InitLegs()
    {
        leftFrontLegPiece = new LegPiece(leftFrontLeg, true, leftFrontLeg.position);
        rightFrontLegPiece = new LegPiece(rightFrontLeg, true, rightFrontLeg.position);
        leftHindLegPiece = new LegPiece(leftHindLeg, true, leftHindLeg.position);
        rightHindLegPiece = new LegPiece(rightHindLeg, true, rightHindLeg.position);
    }

    private enum LegMovingState
    {
        LeftLeg,
        RightLeg
    }

    private IEnumerator MoveLegs()
    {
        float elapseTime = 0f;
        var originalBodyPosition = body.position;
        while(elapseTime < durationToMoveLeg)
        {
            float progress = elapseTime / durationToMoveLeg;
            for(int i = 0; i < legUsed.Length;i++)
            {
                var legData = legUsed[i];
                //move the leg
                legData.leg.position = Vector3.Lerp(legData.originalPosition,
                    legData.newPosition , 
                    progress);
            }

            body.position = Vector3.Lerp(originalBodyPosition, bodyTargetDirection, progress);
            //move the body as well

            elapseTime += Time.deltaTime;
            yield return null;
        }

        body.position = bodyTargetDirection;

        //over here they have already reach the new position;
        for (int i = 0; i < legUsed.Length; i++)
        {
            legUsed[i].leg.position = legUsed[i].newPosition;

            //bad way of doing this because we dont know if the monster is ground or not 
            legUsed[i].isGrounded = true;
        }

    }
}

public struct LegPiece
{
    public Transform leg;
    public bool isGrounded;
    public Vector3 newPosition;
    public Vector3 originalPosition;

    public LegPiece(Transform leg, bool isGrounded, Vector3 newPosition, Vector3 originalPosition)
    {
        this.leg = leg;
        this.isGrounded = isGrounded;
        this.newPosition = newPosition;
        this.originalPosition = originalPosition;
    }

    public LegPiece(Transform leg, bool isGrounded, Vector3 originalPosition) : this()
    {
        this.leg = leg;
        this.isGrounded = isGrounded;
        this.originalPosition = originalPosition;
    }
}