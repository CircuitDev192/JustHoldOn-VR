using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class FlipSight : MonoBehaviour
    {

        InteractionVolume interactionVolume;

        [SerializeField] protected float startRotation;
        [SerializeField] protected float endRotation;

        [SerializeField] protected Transform sight;
        [SerializeField] protected float flipSpeed;

        [SerializeField] bool up;

        void Start()
        {
            interactionVolume = GetComponent<InteractionVolume>();
            interactionVolume._StartInteraction += Flip;
        }

        void Flip()
        {
            up = !up;
            interactionVolume.StopInteraction();
        }

        void FixedUpdate()
        {
            float greatest = startRotation > endRotation ? startRotation : endRotation;
            float least = startRotation > endRotation ? endRotation : startRotation;

            Vector3 tempRot = sight.localEulerAngles;

            float tempSpeed = up ? flipSpeed : -flipSpeed;

            tempRot.x = Mathf.Clamp(tempRot.x + tempSpeed * Time.deltaTime, least, greatest);

            sight.localEulerAngles = tempRot;
        }
    }
}