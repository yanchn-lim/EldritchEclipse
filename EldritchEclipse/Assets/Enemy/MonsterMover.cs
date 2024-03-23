using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterMover : MonoBehaviour
{
    [SerializeField] private Transform body;

    [Tooltip("quadruped legs")]
    [SerializeField] private LegPiece leftFrontLegPiece;
    [SerializeField] private LegPiece rightFrontLegPiece;
    [SerializeField] private LegPiece leftHindLegPiece;
    [SerializeField] private LegPiece rightHindLegPiece;
    
    [SerializeField] private float durationToMoveLeg;
    [SerializeField] private float distanceToTriggerMove;
    [SerializeField] private LegMovingState currentLegState;

    private List<LegPiece> legUsed; //can only move two legs at a time
    private List<LegPiece> legNotUsed; //depends on the condition


    private void Start()
    {
        InitLegs();
        SettingLegMovement();
    }

    private void Update()
    {
        
        RootLegs();
        //if all legs are not used or if the leg used are not moving then do moving again
        if( legNotUsed.Count == 4 || legUsed.All(leg => leg.isGrounded))
        {//if all leg is ground for the leg being used
            SettingLegMovement();
        }
    }

    private void RootLegs()
    {
        foreach(LegPiece piece in legNotUsed)
        {
            piece.leg.position = piece.originalPosition;
        }
    }

    #region ignore this first

    private void SettingLegMovement()
    {
        legNotUsed = new List<LegPiece>();
        legUsed = new List<LegPiece>();
        switch (currentLegState)
        {
            case (LegMovingState.RightLeg):
                legUsed.Add(leftFrontLegPiece);
                legUsed.Add(rightHindLegPiece);

                legNotUsed.Add(leftHindLegPiece);
                legNotUsed.Add(rightFrontLegPiece);

                //make sure that the leg moving is the right leg
                currentLegState = LegMovingState.LeftLeg;
                break;
            case (LegMovingState.LeftLeg):
                legUsed.Add( rightFrontLegPiece);
                legUsed.Add(leftHindLegPiece);

                legNotUsed.Add(rightHindLegPiece);
                legNotUsed.Add(leftFrontLegPiece);

                //make sure that the next leg would be the left leg
                currentLegState = LegMovingState.RightLeg;
                break;
        }
        
        for(int i = 0; i < legUsed.Count; i++)        
        {
            var piece = legUsed[i];
            if (piece.distance >= distanceToTriggerMove)
            {//if can move then do the coroutine to move the leg
                StartCoroutine(MoveLeg(piece));
            }
            else
            {
                //If cant move then dont move
                legNotUsed.Add(piece);
            }
        }

    }

    private void InitLegs()
    {
        leftFrontLegPiece.originalPosition = leftFrontLegPiece.leg.position;
        rightFrontLegPiece.originalPosition = rightFrontLegPiece.leg.position;
        leftHindLegPiece.originalPosition = leftHindLegPiece.leg.position;
        rightHindLegPiece.originalPosition = rightHindLegPiece.leg.position;

        leftFrontLegPiece.isGrounded = true;
        rightFrontLegPiece.isGrounded = true;
        leftHindLegPiece.isGrounded = true;
        rightHindLegPiece.isGrounded = true;
    }

    private enum LegMovingState
    {
        LeftLeg,
        RightLeg
    }

    private IEnumerator MoveLeg(LegPiece  leg) //cant store a ref type
    {
        print($"moving leg {leg.leg.name}");
        leg.isGrounded = false; //tell that it is not moving

        float elapseTime = 0f;
        Transform legTransform = leg.leg;
        Vector3 targetPosition = leg.newPosition.targetPosition;
        
        while (elapseTime < durationToMoveLeg) 
        {
            //do the leg moving here
            legTransform.position = Vector3.Lerp(leg.originalPosition,
                targetPosition,
                elapseTime / durationToMoveLeg
                );

            elapseTime += Time.deltaTime;
            yield return null;
        }
        leg.originalPosition = targetPosition;
        leg.isGrounded = true; //now it is grounded

        print($"Finish moving leg {leg.leg.name}");
    }

    #endregion
}

[Serializable]
public class LegPiece
{
    public Transform leg;
    public bool isGrounded;
    public TargetPosition newPosition;
    [HideInInspector] public Vector3 originalPosition;
    
    public float distance
    {
        get
        {
            return Vector3.Distance(leg.position, newPosition.targetPosition);
        }
    }

}