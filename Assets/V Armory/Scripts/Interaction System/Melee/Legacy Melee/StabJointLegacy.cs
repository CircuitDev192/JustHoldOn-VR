using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class StabJointLegacy : MonoBehaviour
    {
        void Start()
        {
            joint = GetComponent<ConfigurableJoint>();
            jointRb = GetComponent<Rigidbody>();

            unstabbedPosition = transform.localPosition;
            restingPosition = unstabbedPosition;
            restingPosition.y += joint.linearLimit.limit;

            bladeLength = joint.linearLimit.limit;
        }

        public TestDummyJoint dummyJoint;

        public TwitchExtension.DotAxis stabAxis;

        public float bladeLength;
        public float resistance;
        public float damp;

        public Vector3 unstabbedPosition;
        public Vector3 restingPosition;

        public Rigidbody stabbedRigidbody;
        public FixedJoint attachedFixedJoint;

        public ConfigurableJoint joint;
        public Rigidbody jointRb;

        public float previousStabJointPosition;

        public float stabTime;
        public float unstabTime;
        public Collider unstabCol;

        public Collider onCollider;
    }
}
