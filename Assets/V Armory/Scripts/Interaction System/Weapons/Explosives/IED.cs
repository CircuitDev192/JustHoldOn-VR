using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class IED : ExplosiveItem
    {
        [SerializeField] protected InteractionVolume armingKnob;
        [SerializeField] protected float forceThreshold;

        protected override void Start()
        {
            base.Start();
            armingKnob._StartInteraction += DelayedArm;
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.relativeVelocity.magnitude > forceThreshold && armed)
                Explode();
        }
    }
}