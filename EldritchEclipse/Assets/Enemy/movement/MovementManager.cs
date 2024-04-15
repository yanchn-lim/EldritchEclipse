using PGGE.Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Movement
{
    public class MovementManager : MonoBehaviour
    {
        [SerializeField] private Transform leftLegPrefab;
        [SerializeField] private Transform rightLegPrefab;

        //for now it is quadpedal
        private Dictionary<Quadpedal, object> bodyPartLookUp;
        [SerializeField] private Transform rigTransform;
        [Header("dual legs")]
        [SerializeField] private float rotationSpeed = 1f;

        [Header("Moving")]
        [SerializeField] private float walkingPauseTime = 1f;

        [Header("FK Body")]
        [SerializeField] private bool enabledFKBody;
        [SerializeField] private float preferredHeight = 1.1f;
        [SerializeField] private float fkDamping = 1f;

        //the lookup must start at the core
        private FSM movementStateMachine;

        public float RotationSpeed { get => rotationSpeed; }
        public float WalkingPauseTime { get => walkingPauseTime;}
        public float PreferredHeight { get => preferredHeight;}
        public float FkDamping { get => fkDamping;}

        private void Start()
        {
            bodyPartLookUp = new Dictionary<Quadpedal, object>();
            bodyPartLookUp.Add(Quadpedal.HEAD, this);//store the transform
            ImplantComponent();
            ImplantFKBody();
            //DebuggingBodyLookUp();
            movementStateMachine = new FSM();

            movementStateMachine.Add((int)MovementState.WALKING, new WalkingState(
                movementStateMachine,
                (int)MovementState.WALKING,
                (LegMovement)bodyPartLookUp[Quadpedal.UPPERLEFTLEG],
                (LegMovement)bodyPartLookUp[Quadpedal.LOWERLEFTLEG],
                (LegMovement)bodyPartLookUp[Quadpedal.UPPERRIGHTLEG],
                (LegMovement)bodyPartLookUp[Quadpedal.LOWERRIGHTLEG],
                this
                ));

            movementStateMachine.Add((int)MovementState.RUNNING, new RunningState(
                movementStateMachine,
                (int)MovementState.RUNNING,
                (LegMovement)bodyPartLookUp[Quadpedal.UPPERLEFTLEG],
                (LegMovement)bodyPartLookUp[Quadpedal.LOWERLEFTLEG],
                (LegMovement)bodyPartLookUp[Quadpedal.UPPERRIGHTLEG],
                (LegMovement)bodyPartLookUp[Quadpedal.LOWERRIGHTLEG],
                this
                ));
            movementStateMachine.SetCurrentState((int)MovementState.WALKING);
        }

        private void Update()
        {
            movementStateMachine.Update();   
        }

        private void HandleWalking()
        {
            LegMovement upperLeftLeg = (LegMovement)bodyPartLookUp[Quadpedal.UPPERLEFTLEG];
            LegMovement upperRightLeg = (LegMovement)bodyPartLookUp[Quadpedal.UPPERRIGHTLEG];
            LegMovement lowerLeftLeg = (LegMovement)bodyPartLookUp[Quadpedal.LOWERLEFTLEG];
            LegMovement lowerRightLeg = (LegMovement)bodyPartLookUp[Quadpedal.LOWERRIGHTLEG];

        }

        private void ImplantComponent()
        {

            foreach (Transform t in  transform)
            {
                if (t == rigTransform) continue;//ignore the rig transform

                //then it is one of the main body partName
                DualLegs component = t.AddComponent<DualLegs>();    

                //add the dual legs component to the main part

                if (t.name == "LowerBody")
                {
                    //Lower body
                    bodyPartLookUp.Add(Quadpedal.WAIST, component);
                }
                else
                {
                    //Upper body
                    bodyPartLookUp.Add(Quadpedal.SHOULDER, component);
                }
            }

            InstallLegs(true);
            InstallLegs(false);

            //set up the waist and shoulder

            ((DualLegs)bodyPartLookUp[Quadpedal.SHOULDER]).SetUpLegs(
                (LegMovement)bodyPartLookUp[Quadpedal.UPPERLEFTLEG],
                (LegMovement)bodyPartLookUp[Quadpedal.UPPERRIGHTLEG],
                this
                );
            ((DualLegs)bodyPartLookUp[Quadpedal.WAIST]).SetUpLegs(
                (LegMovement)bodyPartLookUp[Quadpedal.LOWERLEFTLEG],
                (LegMovement)bodyPartLookUp[Quadpedal.LOWERRIGHTLEG],
                this
                );

            void InstallLegs(bool upperBody)
            {
                Transform bodyPart;

                if (upperBody)
                {
                    bodyPart = ((DualLegs)bodyPartLookUp[Quadpedal.SHOULDER]).transform;
                }
                else
                {
                    bodyPart = ((DualLegs)bodyPartLookUp[Quadpedal.WAIST]).transform;
                }

                foreach (Transform t in bodyPart)
                {
                    LegMovement component;
                    if (t.name == "LeftLeg")
                    {
                        component = Instantiate(leftLegPrefab, t).GetComponentInChildren<LegMovement>();
                    }
                    else
                    {
                        component = Instantiate(rightLegPrefab, t).GetComponentInChildren<LegMovement>();
                    }
                    
                    //component.name = $"{t.name} moveable";
                    if (component == null) Debug.LogError("There is no leg component for the legs!");

                    component.JointConnection(bodyPart);

                    if(t.name == "LeftLeg")
                    {
                        if(upperBody) bodyPartLookUp.Add(Quadpedal.UPPERLEFTLEG, component);
                        else bodyPartLookUp.Add(Quadpedal.LOWERLEFTLEG, component);
                    }
                    else
                    {
                        if (upperBody) bodyPartLookUp.Add(Quadpedal.UPPERRIGHTLEG, component);
                        else bodyPartLookUp.Add(Quadpedal.LOWERRIGHTLEG, component);
                    }
                }
            }

        }

        private void ImplantFKBody()
        {
            if(!enabledFKBody) return;
            transform.AddComponent<FKBody>().Init(this);
            ((DualLegs)bodyPartLookUp[Quadpedal.SHOULDER]).AddComponent<FKBody>().Init(this);
            ((DualLegs)bodyPartLookUp[Quadpedal.WAIST]).AddComponent<FKBody>().Init(this);
            
        }

        private void DebuggingBodyLookUp()
        {
            Array quadPedalArray = Enum.GetValues(typeof(Quadpedal));
            foreach(Quadpedal partName in quadPedalArray)
            {
                if (bodyPartLookUp.ContainsKey(partName))
                {
                    Transform part = (Transform) bodyPartLookUp[partName];
                    print($"{partName}: {part.name}");
                }
            }
        }

        enum Quadpedal
        {
            HEAD,
            WAIST,
            SHOULDER,
            UPPERLEFTLEG,
            UPPERRIGHTLEG,
            LOWERLEFTLEG,
            LOWERRIGHTLEG,
        }

        enum MovementState
        {
            WALKING,
            RUNNING
        }

        public void ChangeToRunningState()
        {
            movementStateMachine.SetCurrentState((int)MovementState.RUNNING);
        }

        
        public void ChangeToWalkingState()
        {
            movementStateMachine.SetCurrentState((int)MovementState.WALKING);
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 150, 100), "walk"))
            {
                ChangeToWalkingState();
            }

            if (GUI.Button(new Rect(200, 10, 150, 100), "Run"))
            {
                ChangeToRunningState();
            }
        }
    }
}