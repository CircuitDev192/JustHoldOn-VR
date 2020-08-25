using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Detonator : Item
    {
        [SerializeField] protected List<Explosive> explosives = new List<Explosive>();
        [SerializeField] protected Trigger trigger;

        public delegate void DetonateEvent();
        public DetonateEvent _Detonate;

        protected override void Start()
        {
            base.Start();

            if (!trigger) trigger = GetComponentInChildren<Trigger>();

            if (trigger)
            {
                trigger._TriggerPulled += Detonate;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!PrimaryHand)
                return;

            if (trigger) trigger.PoseTrigger(PrimaryHand.TriggerRotation);
        }

        void Detonate()
        {
            if (_Detonate != null)
                _Detonate();
        }
    }
}
