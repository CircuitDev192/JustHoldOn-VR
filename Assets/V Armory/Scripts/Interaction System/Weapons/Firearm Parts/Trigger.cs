using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Trigger : MonoBehaviour
    {
        public delegate void TriggerEvent();

        public TriggerEvent _TriggerPulled;
        public TriggerEvent _TriggerHeld;
        public TriggerEvent _TriggerReleased;

        protected bool releasedTrigger;
        public bool ReleasedTrigger { get { return releasedTrigger; } set { releasedTrigger = value; } }

        [SerializeField] [Range(0, 1)] protected float triggerRelease = 0.2f;
        [SerializeField] [Range(0, 1)] protected float triggerPull = 0.9f;

        [SerializeField] [Range(0, 1)] protected float doubleActionTriggerRelease = 0.2f;
        [SerializeField] [Range(0, 1)] protected float doubleActionTriggerPull = 0.9f;

        public float TriggerAxis(float input)
        {
            float tempTriggerAxis = releasedTrigger ? input : 0;

            tempTriggerAxis = TwitchExtension.NormalizeInput(0, triggerPull - triggerRelease, tempTriggerAxis - triggerRelease);

            return tempTriggerAxis;
        }

        protected enum PoseType
        {
            position,
            rotation
        }

        [SerializeField] protected PoseType poseType = PoseType.rotation;

        protected enum Axis
        {
            x,
            y,
            z,
            none
        }

        [SerializeField] protected Axis poseAxis;
        [SerializeField] protected float poseInputScale;
        Vector3 initialPose;

        [SerializeField] protected Hammer hammer;

        void Start()
        {
            initialPose = poseType == PoseType.position ? transform.localPosition : transform.localEulerAngles;
        }

        public virtual void PoseTrigger(float input)
        {
            Vector3 desiredVector = initialPose;

            float hammerDelta = hammer ? hammer.touchPadDelta * poseInputScale : 0;

            if (poseInputScale != 0)
            {
                switch (poseAxis)
                {
                    case Axis.x:
                        desiredVector.x = (input * poseInputScale) + initialPose.x - hammerDelta;
                        break;
                    case Axis.y:
                        desiredVector.y = (-input * poseInputScale) + initialPose.y + hammerDelta;
                        break;
                    case Axis.z:
                        desiredVector.z = (input * poseInputScale) + initialPose.z - hammerDelta;
                        break;
                    case Axis.none:

                        break;
                }
            }

            if (poseType == PoseType.position)
                transform.localPosition = desiredVector;
            else
                transform.localEulerAngles = desiredVector;

            //Do not pull back hammer if the gun is on safety

            float tempTriggerPull = hammer ? (hammer.Cocked ? doubleActionTriggerPull : triggerPull) : triggerPull;

            if (input >= tempTriggerPull)
            {
                if (releasedTrigger)
                {
                    _TriggerPulled();
                    releasedTrigger = false;
                }

                if (_TriggerHeld != null)
                    _TriggerHeld();
            }

            float tempTriggerRelease = hammer ? (hammer.Cocked ? doubleActionTriggerRelease : triggerRelease) : triggerRelease;

            if (input <= tempTriggerRelease && !releasedTrigger)
            {
                if(_TriggerReleased != null)
                    _TriggerReleased();

                releasedTrigger = true;
            }
        }
    }
}