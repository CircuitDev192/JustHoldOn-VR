using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Hinge : MonoBehaviour
    {

        [SerializeField] protected InteractionVolume interactionVolume;

        [SerializeField] protected bool inversePull;

        [SerializeField] protected TwitchExtension.Axis axis;

        [SerializeField] protected float pullDistance;
        [SerializeField] protected float maxAngle;

        [ReadOnly] [SerializeField] float initialLocalHeight;
        [ReadOnly] [SerializeField] float offset = 1;
        [SerializeField] protected Hand hingeHand;

        public virtual Hand HingeHand
        {
            get { return hingeHand; }

            set
            {
                hingeHand = value;

                if (!hingeHand)
                    return;

                initialLocalHeight = transform.parent.InverseTransformDirection(hingeHand.transform.position - transform.parent.position).y;

                switch (axis)
                {
                    case TwitchExtension.Axis.x:
                        offset = TwitchExtension.NormalizeInput(0, maxAngle, transform.localEulerAngles.x);
                        break;
                    case TwitchExtension.Axis.y:
                        offset = TwitchExtension.NormalizeInput(0, maxAngle, transform.localEulerAngles.y);
                        break;
                    case TwitchExtension.Axis.z:
                        offset = TwitchExtension.NormalizeInput(0, maxAngle, transform.localEulerAngles.z);
                        break;
                }

                if (inversePull)
                    offset = 1 - offset;

                offset *= -pullDistance;
            }
        }

        protected virtual void GrabHinge() { HingeHand = interactionVolume.Hand; }

        protected virtual void RemoveHingeHand() { HingeHand = null; }

        protected virtual void Start()
        {
            interactionVolume._StartInteraction += GrabHinge;
            interactionVolume._EndInteraction += RemoveHingeHand;
        }

        protected virtual void Update()
        {
            PullHinge();
        }

        public void ForceStop() { interactionVolume.StopInteraction(); }
        public void ForceStart(Hand hand) { interactionVolume.ForceStartInteraction(hand); }

        protected void PullHinge()
        {
            if (!HingeHand)
                return;

            switch (axis)
            {
                case TwitchExtension.Axis.x:
                    transform.localEulerAngles = new Vector3(Pull(), transform.localEulerAngles.y, transform.localEulerAngles.z);
                    break;
                case TwitchExtension.Axis.y:
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Pull(), transform.localEulerAngles.z);
                    break;
                case TwitchExtension.Axis.z:
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Pull());
                    break;
            }
        }

        protected float Pull()
        {
            if (!HingeHand)
                switch (axis)
                {
                    case TwitchExtension.Axis.x:
                        return transform.localEulerAngles.x;
                    case TwitchExtension.Axis.y:
                        return transform.localEulerAngles.y;
                    case TwitchExtension.Axis.z:
                        return transform.localEulerAngles.z;
                }

            float tempOffset = initialLocalHeight + offset;
            float min = tempOffset <= initialLocalHeight ? tempOffset : tempOffset - pullDistance;
            float max = tempOffset > initialLocalHeight ? tempOffset : tempOffset + pullDistance;

            float rotationNormal = TwitchExtension.NormalizeInput(min, max,
                                                                  transform.parent.InverseTransformDirection(HingeHand.transform.position - transform.parent.position).y);

            if (inversePull)
                rotationNormal = 1 - rotationNormal;

            return rotationNormal * maxAngle;
        }

        [SerializeField] protected List<AudioClip> openSounds = new List<AudioClip>();
        [SerializeField] protected List<AudioClip> closeSounds = new List<AudioClip>();


        protected virtual void OpenSFX() { TwitchExtension.PlayRandomAudioClip(openSounds, transform.position); }

        protected virtual void CloseSFX() { TwitchExtension.PlayRandomAudioClip(closeSounds, transform.position); }
    }
}