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

        private void Start()
        {
            foreach (BodyPartInformation part in bodyParts)
            {
                if(part.child != null)
                {
                    part.child.SetParent(this);

                    //orient the part to the transfrom

                    Transform childTransform = part.child.transform;
                    //make it a child of connection
                    childTransform.parent = part.connection.transform;
                    //adjust the local and rotation of the child body part to fit the connection
                    //resume to connection point
                    childTransform.localPosition = Vector3.zero; 
                    //have no rotation as it based on connection
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
    }
}