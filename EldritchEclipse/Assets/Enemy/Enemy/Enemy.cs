using Movement;
using Modular;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(BodyPart))]
    public class Enemy : MonoBehaviour
    {
        //when attach, it will naturally assume that it is a JointConnection
        //private BodyPart bodyParts;

        //[Header("experiment")]
        //[SerializeField] private BodyPart legsPrefab;

        //private void Start()
        //{
        //    bodyParts = GetComponent<BodyPart>();
        //    foreach (var part in bodyParts.BodyParts)
        //    {
        //        if(part.type == BodyPartType.DUALJOINTS)
        //        {
        //            //create the legs
        //            CreateLegs(part.child.GetComponent<DualLegs>(),part.child);
        //        }
        //    }
        //}

        //private void CreateLegs(DualLegs dualLegs , BodyPart connection)
        //{
        //    //start by adding the left leg followed by the right leg
        //    List<LegMovement> listLegMovements = new List<LegMovement>();
        //    foreach(var legJoint in connection.BodyParts)
        //    {
        //        var leg = Instantiate(legsPrefab, legJoint.connection);
        //        legJoint.child = leg.GetComponent<BodyPart>();
        //        var legComponent = leg.GetComponentInChildren<LegMovement>();
        //        legComponent.JointConnection(connection.transform);
        //        listLegMovements.Add(legComponent);
        //    }
        //    dualLegs.SetUpLegs(listLegMovements[0] , listLegMovements[1]);

        //}

    }
}