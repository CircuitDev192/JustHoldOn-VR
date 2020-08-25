using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Drawer : Item
    {
        [SerializeField] protected ConfigurableJoint joint;
        [SerializeField] protected float openLimit;
        [SerializeField] protected float closedLimit;

        [SerializeField] protected Vector2 freeSwingDrag;
        [SerializeField] protected Vector2 heldDrag;

        public Transform InitialAttachPoint;

        protected override void Start()
        {
            base.Start();
            joint = GetComponent<ConfigurableJoint>();
        }

        void SetLimit(float limit)
        {
            if (joint.linearLimit.limit == limit)
                return;

            SoftJointLimit newLimit = new SoftJointLimit();
            newLimit.limit = limit;
            newLimit.bounciness = joint.linearLimit.bounciness;
            joint.linearLimit = newLimit;
        }

        public override void Pose()
        {
            if (PrimaryHand && rb)
            {
                Vector3 positionDelta = (PrimaryHand.transform.position - InitialAttachPoint.position) * setPositionSpeed;
                rb.AddForceAtPosition(positionDelta, InitialAttachPoint.position, ForceMode.VelocityChange);
            }
        }

        protected override void PrimaryDrop()
        {
            transform.SetParent(null, true);

            if (rb)
            {
                rb.drag = freeSwingDrag.x;
                rb.angularDrag = freeSwingDrag.y;
            }

            SetPhysics(onDetach);

            if (InitialAttachPoint != null)
                Destroy(InitialAttachPoint.gameObject);
        }

        protected override void PrimaryGrasp()
        {
            if (!PrimaryHand)
                return;

            InitialAttachPoint = new GameObject(string.Format("[{0}] InitialAttachPoint", this.gameObject.name)).transform;
            InitialAttachPoint.position = PrimaryHand.transform.position;
            InitialAttachPoint.rotation = PrimaryHand.transform.rotation;
            InitialAttachPoint.localScale = Vector3.one * 0.25f;
            InitialAttachPoint.parent = this.transform;

            rb.drag = heldDrag.x;
            rb.angularDrag = heldDrag.y;
            SetPhysics(onAttach);

            SetLimit(openLimit);

        }
    }
}