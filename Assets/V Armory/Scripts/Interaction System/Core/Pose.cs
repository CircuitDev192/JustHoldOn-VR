using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Pose : MonoBehaviour
    {
       /* protected Rigidbody rb;
        protected Collider col;

        public Rigidbody Rb { get { return rb; } }
        public Collider Col { get { return col; } }

        [SerializeField] protected Slot slot;
        public Slot Slot { get { return slot; } }

        [SerializeField] protected InteractionVolume primaryGrip;
        [SerializeField] protected InteractionVolume secondaryGrip;

        public InteractionVolume PrimaryGrip { get { return primaryGrip; } }
        protected InteractionVolume SecondaryGrip { get { return secondaryGrip; } }

        protected Hand PrimaryHand { get { return primaryGrip ? primaryGrip.Hand : null; } }
        protected Hand SecondaryHand { get { return secondaryGrip ? secondaryGrip.Hand : null; } }

        [SerializeField] protected Vector3 positionOffset; //Use this to fix the position of this object when being held
        [SerializeField] protected Vector3 rotationOffset; //Use this to fix the rotation of this object when being held

        //[SerializeField] protected Vector3 secondaryPositionOffset; //Use this to fix the position when holding this object with only the secondary hand
        //public Vector3 SecondaryPositionOffset { get { return secondaryPositionOffset; } }

        [SerializeField] protected Vector3 alignedRotationOffset;

        public Vector3 PositionOffset { get { return positionOffset; } set { positionOffset = value; } }
        public Vector3 RotationOffset { get { return rotationOffset; } set { rotationOffset = value; } }

        [System.Serializable]
        public struct PhysicsSettings
        {
            public bool isKinematic;
            public bool useGravity;
            public bool colliderEnabled;
            public bool isTrigger;
            public bool parent;
            public LayerMask layer;
        }

        [SerializeField] protected PhysicsSettings onAttach = new PhysicsSettings { isKinematic = true, useGravity = false, colliderEnabled = true, isTrigger = false, parent = true };
        [SerializeField] protected PhysicsSettings onDetach = new PhysicsSettings { isKinematic = false, useGravity = true, colliderEnabled = true, isTrigger = false, parent = false };

        public PhysicsSettings OnAttach { get { return onAttach; } }
        public PhysicsSettings OnDetach { get { return onDetach; } }

        public PhysicsSettings CurrentPhysicsProperties
        { get { return new PhysicsSettings { isKinematic = item.Rb.isKinematic, useGravity = item.Rb.useGravity, colliderEnabled = item.Col.enabled, isTrigger = item.Col.isTrigger, parent = false }; } }

        protected float positionGrabTime;
        protected float rotationGrabTime;

        protected Vector3 grabLocalPosition;
        protected Quaternion grabLocalRotation;
        protected Vector3 grabPosition;
        protected Quaternion grabRotation;

        [SerializeField] protected float setPositionSpeed = 1;
        [SerializeField] protected float setRotationSpeed = 1;
        [SerializeField] protected AnimationCurve setPositionCurve;
        [SerializeField] protected AnimationCurve setRotationCurve;

        [SerializeField] protected ConfigurableJoint configurableJoint;
        protected ConfigurableJoint physicsJoint;

        [SerializeField] protected bool moveOffsetOnAttach;

        public delegate void PoseEvent();

        public PoseEvent preGrabPrimary;
        public PoseEvent postGrabPrimary;
        public PoseEvent preGrabSecondary;
        public PoseEvent postGrabSecondary;
        public PoseEvent preTotalDrop;
        public PoseEvent postTotalDrop;

        public virtual void PrimaryGrab()
        {
            if(preGrabPrimary != null) preGrabPrimary();

            PrimaryHand.StoredItem = item;

            SetInitialGrabRotationAndPosition(PrimaryHand, moveOffsetOnAttach, onAttach.parent, OnAttach);

            if (SecondaryHand)
                PrimaryGrabWithSecondary();

            if (postGrabPrimary != null) postGrabPrimary();
        }

        public virtual void PrimaryGrabWithSecondary()
        {
            
        }

        public virtual void PrimaryDrop()
        {
            DetachSlotLight(PrimaryHand);
        }

        public virtual void PrimaryDropWithSecondary()
        {

        }

        public virtual void SecondaryGrab()
        {

        }

        public virtual void SecondaryGrabWithPrimary()
        {

        }

        public virtual void SecondaryDrop()
        {

        }

        public virtual void SecondaryDropWithPrimary()
        {

        }

        public virtual void TotalDrop()
        {

        }

        public virtual void DetachSlotLight(Slot slot)
        {
            if (slot)
                slot.UnstoreItemLight(item);

            this.slot = null;
        }

        protected virtual void SetInitialGrabRotationAndPosition(Slot slot, bool orientate, bool setParent, PhysicsSettings setPhysics)
        {
            SetInitialGrabRotationAndPosition(slot, orientate, setParent, setPhysics, -PositionOffset);
        }

        protected virtual void SetInitialGrabRotationAndPosition(Slot slotOffset, bool orientate, bool setParent, PhysicsSettings setPhysics, Vector3 positionOffset)
        {
            Transform grip = slotOffset.Offset;

            SetPhysics(setPhysics);

            if (orientate)
            {
                grip.position = transform.TransformPoint(-positionOffset);
                grip.rotation = transform.rotation;
            }

            if (setParent)
                transform.SetParent(grip, true);

            positionGrabTime = Time.time;
            rotationGrabTime = Time.time;

            grabRotation = grip.rotation;
            grabPosition = grip.localPosition;

            grabLocalPosition = grip.localPosition;
            grabLocalRotation = grip.localRotation;

            SetPhysicsJoint(slotOffset.OffsetRigidbody);
        }

        protected virtual void ResetOffset(Hand hand)
        {
            hand.Offset.transform.localPosition = Vector3.zero;
            hand.Offset.transform.localRotation = Quaternion.identity;
        }

        public void SetPhysics(PhysicsSettings settings)
        {
            TwitchExtension.SetItemLayerRecursivly(gameObject, settings.layer);

            if (!rb)
                return;

            if (rb.isKinematic != settings.isKinematic)
                rb.isKinematic = settings.isKinematic;
            if (rb.isKinematic != settings.useGravity)
                rb.useGravity = settings.useGravity;

            if (!col)
                return;

            if (col.enabled != settings.colliderEnabled)
                col.enabled = settings.colliderEnabled;
            if (col.isTrigger != settings.isTrigger)
                col.isTrigger = settings.isTrigger;
        }

        public void SetPhysics(bool kinematic, bool gravity, bool collider)
        {
            if (!rb)
                return;

            if (rb.isKinematic != kinematic)
                rb.isKinematic = kinematic;
            if (rb.isKinematic != gravity)
                rb.useGravity = gravity;

            if (!col)
                return;

            if (col.enabled != collider)
                col.enabled = collider;
        }

        public void SetPhysics(bool kinematic, bool gravity, bool collider, bool isTrigger)
        {
            SetPhysics(kinematic, gravity, collider);

            if (!col)
                return;

            if (col.isTrigger != isTrigger)
                col.isTrigger = isTrigger;
        }

        public void SetKinematic()
        {
            SetKinematic(true);
        }

        public void SetKinematic(bool kinematic)
        {
            if (!rb)
                return;

            if (rb.isKinematic != kinematic)
                rb.isKinematic = kinematic;
        }

        public void SetPhysicsJoint(Rigidbody rb)
        {
            if (configurableJoint)
                if (rb)
                    if (!physicsJoint)
                        physicsJoint = gameObject.AddComponent<ConfigurableJoint>();

            if (physicsJoint)
            {
                SetKinematic(false);
                physicsJoint.xDrive = configurableJoint.xDrive;
                physicsJoint.yDrive = configurableJoint.yDrive;
                physicsJoint.zDrive = configurableJoint.zDrive;
                physicsJoint.angularYZDrive = configurableJoint.angularYZDrive;
                physicsJoint.angularXDrive = configurableJoint.angularXDrive;
                physicsJoint.connectedBody = rb;
            }
        }
        */
    }
}
