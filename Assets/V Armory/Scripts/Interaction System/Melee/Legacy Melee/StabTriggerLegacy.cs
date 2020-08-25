using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class StabTriggerLegacy : MonoBehaviour
    {
        public Collider col;
        public List<StabJointLegacy> stabJoints = new List<StabJointLegacy>();

        public delegate void StabEvent(StabTriggerLegacy stabTrigger, Collider collider);
        public StabEvent _OnTriggerEnter;

        public List<StabTriggerLegacy> disableTriggersOnStab;

        private void Start()
        {
            col = GetComponent<Collider>();
        }

        void OnTriggerEnter(Collider collider)
        {
            _OnTriggerEnter(this, collider);
        }
    }
}
