using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Modular
{
    public class BodyPart : MonoBehaviour
    {
        public BodyPart parent { get; private set; }

        [Tooltip("Body part")]
        [SerializeField] private BodyPartInformation[] bodyParts;
        public BodyPartInformation[] BodyParts { get => bodyParts; }

        private void Start()
        {
            foreach (BodyPartInformation part in BodyParts)
            {
                if (part.child.transform == part.connection.transform) continue;//cyclic
                if(part.child != null)
                {
                    part.child.SetParent(this);

                    //orient the bodyParts to the transfrom

                    Transform childTransform = part.child.transform;
                    //make it a child of JointConnection
                    childTransform.parent = part.connection.transform;
                    //adjust the local and rotation of the child JointConnection bodyParts to fit the JointConnection
                    //resume to JointConnection point
                    childTransform.localPosition = Vector3.zero; 
                    //have no rotation as it based on JointConnection
                    childTransform.localRotation = Quaternion.identity;
                    childTransform.localScale = Vector3.one;
                }
            }
        }

        public void SetParent(BodyPart parent)
        {
            this.parent = parent;
        }
    }

    [Serializable]
    public class BodyPartInformation
    {
        public Transform connection;
        public BodyPart child;
        public BodyPartType type;
    }
}