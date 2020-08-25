using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class IED_Knob : MonoBehaviour
    {
        protected InteractionVolume interactionVolume;

        protected bool flipped;

        [SerializeField] protected RotateDirection rotateDirection;
        [SerializeField] protected bool inverseRotate;

        protected enum RotateDirection
        {
            up,
            forward,
            right
        }

        void Start()
        {
            interactionVolume = GetComponent<InteractionVolume>();
            interactionVolume._StartInteraction += FlipKnob;
        }

        void FixedUpdate()
        {
            int desiredAngle = inverseRotate ? -90 : 90;

            transform.localRotation = Quaternion.Lerp(transform.localRotation, 
                                                      Quaternion.Euler(new Vector3(flipped && rotateDirection == RotateDirection.right ? desiredAngle : 0, 
                                                                                   flipped && rotateDirection == RotateDirection.up ? desiredAngle : 0, 
                                                                                   flipped && rotateDirection == RotateDirection.forward ? desiredAngle : 0)),
                                                      30f * Time.fixedDeltaTime);
        }

        void FlipKnob()
        {
            flipped = !flipped;
            interactionVolume.StopInteraction();
        }
    }
}