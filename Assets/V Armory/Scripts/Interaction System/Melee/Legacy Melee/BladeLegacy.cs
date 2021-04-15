using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class BladeLegacy : Item
    {
        public List<StabJointLegacy> stabJoints = new List<StabJointLegacy>();

        public bool useFixedJoint;
        public bool setAttachedAsChild;

        public float stabVelocityThreshold;
        public float stabAngleThreshold;

        public float unstabVelocity;

        [SerializeField] protected bool useLegacyPosing;

        public float heldAndStabbedMassScale = 2;
        public float heldAndStabbedConnectedMassScale = 1;

        public float heldStabSlideMassScale = 20;
        public float heldStabSlideConnectedBodyMassScale = 0.1f;

        public float droppedStabSlideMassScale = 1;
        public float droppedStabSlideConnectedBodyMassScale = 1;

        public float droppedMass;
        public float droppedDrag;
        public float droppedAngularDrag;

        public float heldMass;
        public float heldDrag;
        public float heldAngularDrag;

        //public AnimationCurve forceOverDistance;
        //public AnimationCurve dragOverDistance;

        //public AnimationCurve angularForceOverAngleDiff;
        //public AnimationCurve angularDragOverAngleDiff;

        //public AnimationCurve angularForceOverDistance;

        protected override void Start()
        {
            base.Start();
            rb.maxAngularVelocity = Mathf.Infinity;
            rb.centerOfMass = Vector3.zero;

            StabTriggerLegacy[] stabTriggers = GetComponentsInChildren<StabTriggerLegacy>();

            foreach (StabTriggerLegacy stabTrigger in stabTriggers)
            {
                stabTrigger._OnTriggerEnter += TriggerStab;
            }

            Invoke("Late", 0.5f);
        }

        void Late()
        {
            StabJointLegacy[] tempStabJoints = GetComponentsInChildren<StabJointLegacy>();

            foreach (StabJointLegacy stabJoint in tempStabJoints)
            {
                stabJoints.Add(stabJoint);
            }
        }

        protected override void Update()
        {
            base.Update();

            foreach (StabJointLegacy stabJoint in stabJoints)
            {
                if (stabJoint.stabbedRigidbody)
                {
                    if (Time.time - stabJoint.stabTime > 0.1f)
                        SetUnstabTime(stabJoint);

                    if (Time.time - stabJoint.stabTime > 0.25f)
                    {
                         Resistance(stabJoint);
                    }
                    else if (Time.time - stabJoint.stabTime > 0.5f)
                    {
                         Resistance(stabJoint);
                    }
                    else
                    {
                         Resistance(stabJoint);
                    }

                    if (UnstabConditions(stabJoint))
                        Unstab(stabJoint);
                }

                stabJoint.previousStabJointPosition = stabJoint.transform.localPosition.y;
            }
        }

        protected virtual bool UnstabConditions(StabJointLegacy stabJoint)
        {
            Vector3 colliderVelocity = stabJoint.stabbedRigidbody ? stabJoint.stabbedRigidbody.velocity : Vector3.zero;
            Vector3 characterControllerVelocity = PrimaryHand ? PrimaryHand.CharControllerVelocity : Vector3.zero;

            if (Time.time - stabJoint.stabTime > 1f)
                if (stabJoint.unstabTime > 0.1f)
                    //if (stabJoint.jointRb.velocity.magnitude >= unstabVelocity && 
                    //   Vector3.Dot((velocityHistory._ReleaseVelocity.normalized + colliderVelocity.normalized + characterControllerVelocity.normalized).normalized, stabJoint.transform.up) < -0.25f)
                    {
                        return true;
                    }
            
            return false;
        }

        protected virtual void SetUnstabTime(StabJointLegacy stabJoint)
        {
            float stabPosition = TwitchExtension.ReturnLocalPosition(stabJoint.stabAxis, stabJoint.transform);

            if (stabPosition >= stabJoint.bladeLength * 1.95f && stabJoint.unstabTime == 0)
            {
                stabJoint.unstabTime = Time.time;
            }

            if (stabPosition < stabJoint.bladeLength * 1f && stabJoint.unstabTime != 0)
            {
                stabJoint.unstabTime = 0;
            }
        }

        public AnimationCurve resistanceOverStabDepth;

        protected virtual void Resistance(StabJointLegacy stabJoint)
        {
            float stabPosition = 1 - (TwitchExtension.ReturnLocalPosition(stabJoint.stabAxis, stabJoint.transform) / (stabJoint.bladeLength * 2));

            int polarity = stabJoint.previousStabJointPosition > stabJoint.transform.localPosition.y ? 1 : -1;

            bool disableResistance = stabJoint.dummyJoint ? stabJoint.dummyJoint.Iv.Hand && !stabJoint.dummyJoint.DummyJoint.connectedBody : false;

            if(!disableResistance)
                stabJoint.jointRb.AddForce(Mathf.Abs(stabJoint.previousStabJointPosition - stabJoint.transform.localPosition.y) * stabJoint.resistance * Time.deltaTime * 100 * polarity * stabJoint.transform.up, ForceMode.Impulse);
            else
                stabJoint.jointRb.AddForce(Mathf.Abs(stabJoint.previousStabJointPosition - stabJoint.transform.localPosition.y) * stabJoint.resistance * Time.deltaTime * 50 * stabJoint.joint.connectedMassScale * polarity * stabJoint.transform.up, ForceMode.Impulse);

            stabJoint.joint.yDrive = SetJointDrive(stabJoint.joint.yDrive, 0, stabJoint.damp, Mathf.Infinity);
        }

        protected override void SetOffsetPhysicsForPosing(Hand hand)
        {
            if (VelocityPoseType())
            {
                transform.parent = hand.transform.root;
                rb.isKinematic = false;
                rb.useGravity = false;
            }
        }

        protected override void PrimaryGrasp()
        {
            base.PrimaryGrasp();

            transform.parent = PrimaryHand.transform.root;

            rb.drag = heldDrag;
            rb.angularDrag = heldAngularDrag;
            rb.useGravity = false;

            foreach (StabJointLegacy stabJoint in stabJoints)
            {
                stabJoint.joint.massScale = stabJoint.stabbedRigidbody ? heldAndStabbedMassScale : heldStabSlideMassScale;
                stabJoint.joint.connectedMassScale = stabJoint.stabbedRigidbody ? heldAndStabbedConnectedMassScale : heldStabSlideConnectedBodyMassScale;
            }
        }

        protected override void PrimaryDrop()
        {
            base.PrimaryDrop();

            rb.drag = droppedDrag;
            rb.angularDrag = droppedAngularDrag;
            rb.useGravity = true;

            foreach (StabJointLegacy stabJoint in stabJoints)
            {
                if (stabJoint.stabbedRigidbody)
                    return;

                stabJoint.joint.massScale = droppedStabSlideMassScale;
                stabJoint.joint.connectedMassScale = droppedStabSlideConnectedBodyMassScale;
            }
        }

        StabJointLegacy FreeStabJoint(List<StabJointLegacy> stabJoints)
        {
            foreach (StabJointLegacy stabJoint in stabJoints)
            {
                if (!stabJoint.stabbedRigidbody)
                    return stabJoint;
            }

            return null;
        }

        StabJointLegacy FreeStabJoint(StabJointLegacy[] stabJoints)
        {
            foreach (StabJointLegacy stabJoint in stabJoints)
            {
                if (!stabJoint.stabbedRigidbody)
                    return stabJoint;
            }

            return null;
        }

        StabJointLegacy FreeStabJoint(StabJointGroup stabJoints)
        {
            foreach (StabJointLegacy stabJoint in stabJoints.stabJoints)
            {
                if (!stabJoint.stabbedRigidbody)
                    return stabJoint;
            }

            return null;
        }

        public List<StabTriggerLegacy> stabTriggers = new List<StabTriggerLegacy>();

        [System.Serializable]
        public struct StabJointGroup
        {
            public List<StabJointLegacy> stabJoints;
        }

        TestDummyJoint cutDummyJoint;

        private void OnCollisionEnter(Collision collision)
        {
            TestDummyJoint dummyJoint = collision.collider.GetComponent<TestDummyJoint>();

            if (dummyJoint)
            {
                foreach (StabJointLegacy stabJoint in stabJoints)
                {
                    if (stabJoint.unstabCol)
                        //if (stabJoint.unstabCol == dummyJoint.DummyCollider)
                            return;
                }

                if (collision.relativeVelocity.magnitude >= dummyJoint.CutVelocity)
                {
                    if (Vector3.Distance(collision.contacts[0].point, dummyJoint.transform.position) <= dummyJoint.CutDistance)
                    {
                        Vector3 cutVector = dummyJoint.transform.position - dummyJoint.transform.TransformPoint(dummyJoint.CutVector);

                        if (Vector3.Dot(collision.relativeVelocity.normalized, cutVector) < dummyJoint.CutDot
                            && Vector3.Dot(collision.relativeVelocity.normalized, cutVector) > -dummyJoint.CutDot)
                        {
                            if ((Vector3.Dot(transform.right, cutVector) < dummyJoint.CutDot
                            && Vector3.Dot(transform.right, cutVector) > -dummyJoint.CutDot)
                                ||
                                (Vector3.Dot(-transform.right, cutVector) < dummyJoint.CutDot
                            && Vector3.Dot(-transform.right, cutVector) > -dummyJoint.CutDot))
                            {
                                dummyJoint.DisconnectJoint();
                                cutDummyJoint = dummyJoint;
                                StartCoroutine(NullCutDummy());
                                return;
                            }
                        }
                    }
                }
            }
        }

        void TriggerStab(StabTriggerLegacy stabTrigger, Collider collider)
        {
            List<StabJointLegacy> stabJoints = stabTrigger.stabJoints;

            if (collider.isTrigger) return;

            if (!collider.attachedRigidbody) return;

            if (collider.attachedRigidbody.isKinematic) return;

            StabJointLegacy stabJoint = null;

            foreach (StabJointLegacy joint in this.stabJoints)
            {
                if (collider.attachedRigidbody == joint.stabbedRigidbody)
                    return;
            }

            stabJoint = FreeStabJoint(stabJoints);

            if (!stabJoint) return;

            if (cutDummyJoint ? cutDummyJoint.DummyCollider != collider : true) // Avoids stabbing a cut object
                StartCoroutine(Stab(collider, stabJoint, stabTrigger));
        }

        IEnumerator NullCutDummy()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            cutDummyJoint = null;
        }

        IEnumerator Stab(Collider collider, StabJointLegacy stabJoint, StabTriggerLegacy stabTrigger)
        {
            Debug.Log("Stab");

            Physics.IgnoreCollision(collider, Col);

            foreach (StabTriggerLegacy tempStabTrigger in stabTriggers)
                Physics.IgnoreCollision(collider, tempStabTrigger.col);

            if (cutDummyJoint) //The collider being stabbed was just cut and should not be stabbed
            {
                Physics.IgnoreCollision(collider, Col, false);

                foreach (StabTriggerLegacy tempStabTrigger in stabTriggers)
                    Physics.IgnoreCollision(collider, tempStabTrigger.col, false);

                yield break;
            }

            Vector3 colliderVelocity = collider.attachedRigidbody ? -collider.attachedRigidbody.velocity : Vector3.zero;
            Vector3 characterControllerVelocity = PrimaryHand ? PrimaryHand.CharControllerVelocity : Vector3.zero;

            if (Vector3.Dot((rb.velocity.normalized + characterControllerVelocity.normalized).normalized, stabJoint.joint.transform.up) < stabAngleThreshold) //Angle of approach is too wide //velocityHistory._ReleaseVelocity.normalized
            {
                Physics.IgnoreCollision(collider, Col, false);

                foreach (StabTriggerLegacy tempStabTrigger in stabTriggers)
                    Physics.IgnoreCollision(collider, tempStabTrigger.col, false);

                yield break;
            }

            colliderVelocity += rb.velocity;
            colliderVelocity += characterControllerVelocity;

            if (colliderVelocity.magnitude < stabVelocityThreshold) // Velocity is too low
            {
                Physics.IgnoreCollision(collider, Col, false);

                foreach (StabTriggerLegacy tempStabTrigger in stabTriggers)
                    Physics.IgnoreCollision(collider, tempStabTrigger.col, false);

                yield break;
            }

            stabJoint.unstabCol = collider;

            JointDrive unstabbedDrive = new JointDrive();
            stabJoint.joint.yDrive = unstabbedDrive;

            stabJoint.stabbedRigidbody = collider.attachedRigidbody;

            if (setAttachedAsChild)
                stabJoint.stabbedRigidbody.transform.parent = stabJoint.joint.transform;

            if (useFixedJoint)
            {
                stabJoint.attachedFixedJoint = stabJoint.stabbedRigidbody.gameObject.AddComponent<FixedJoint>();
                stabJoint.attachedFixedJoint.connectedBody = stabJoint.jointRb;
            }
            else
            {
                stabJoint.stabbedRigidbody.isKinematic = true;
            }

            stabJoint.stabTime = Time.time;

            stabJoint.stabbedRigidbody.useGravity = false;
            stabJoint.stabbedRigidbody.velocity = Vector3.zero;

            stabJoint.joint.massScale = heldAndStabbedMassScale;
            stabJoint.joint.connectedMassScale = heldAndStabbedConnectedMassScale;

            stabJoint.dummyJoint = stabJoint.stabbedRigidbody.GetComponent<TestDummyJoint>();

            foreach (StabTriggerLegacy disableTrigger in stabTrigger.disableTriggersOnStab)
                disableTrigger.gameObject.SetActive(false);

            //disable other stab triggers
        }

        void Unstab(StabJointLegacy stabJoint)
        {
            Debug.Log("Unstab");

            if(stabJoint.attachedFixedJoint)
                Destroy(stabJoint.attachedFixedJoint);

            if(setAttachedAsChild)
                stabJoint.stabbedRigidbody.transform.parent = null;

            stabJoint.stabbedRigidbody.useGravity = true;

            stabJoint.joint.yDrive = SetJointDrive(stabJoint.joint.yDrive, 5000, 100, 5000);

            stabJoint.stabbedRigidbody = null;
            
            Physics.IgnoreCollision(stabJoint.unstabCol, Col, false);

            foreach (StabTriggerLegacy stabTrigger in stabTriggers)
                Physics.IgnoreCollision(stabJoint.unstabCol, stabTrigger.col, false);

            stabJoint.unstabCol = null;

            stabJoint.joint.targetPosition = new Vector3(0, -1, 0);
            stabJoint.stabTime = 0;
            stabJoint.unstabTime = 0;

            stabJoint.joint.massScale = heldStabSlideMassScale;
            stabJoint.joint.connectedMassScale = heldStabSlideConnectedBodyMassScale;

            foreach (StabTriggerLegacy stabTrigger in stabTriggers)
            {
                if(stabTrigger.stabJoints.Contains(stabJoint))
                    foreach (StabTriggerLegacy disableTrigger in stabTrigger.disableTriggersOnStab)
                        disableTrigger.gameObject.SetActive(true);
            }
        }

        public override void Pose()
        {
            //if (useLegacyPosing || (slot && !PrimaryHand))
            //    base.Pose();

            if (PrimaryHand)
            {
                //rb.drag = dragOverDistance.Evaluate(Vector3.Distance(PrimaryHand.Offset.parent.position, transform.position)) * heldDrag;
                //rb.angularDrag = angularDragOverAngleDiff.Evaluate(Vector3.Angle(transform.rotation.eulerAngles, PrimaryHand.Offset.parent.rotation.eulerAngles)) * heldAngularDrag;

                Quaternion desiredRotation = SecondaryHand ? (Quaternion.LookRotation((PrimaryHand.Offset.parent.position - SecondaryHand.Offset.parent.position), PrimaryHand.Offset.parent.forward)
                                                            * Quaternion.AngleAxis(90f, Vector3.right))
                                                            : PrimaryHand.Offset.parent.rotation * Quaternion.Inverse(Quaternion.Euler(RotationOffset));

                Vector3 positionDelta = (PrimaryHand.Offset.parent.TransformPoint(PositionOffset) - transform.position) * setPositionSpeed;

                rb.velocity = positionDelta * Time.deltaTime;

                rb.angularVelocity = VectorHistoryAverage.GetAngularVelocityAngleAxis(transform.rotation, desiredRotation) * (SecondaryHand ? setTwoHandRotationSpeed : setRotationSpeed);
            }
        }

        protected virtual JointDrive SetJointDrive(JointDrive jointDrive, float spring)
        {
            JointDrive newDrive = new JointDrive();
            newDrive.positionSpring = spring;
            newDrive.positionDamper = jointDrive.positionDamper;
            newDrive.maximumForce = jointDrive.maximumForce;
            return newDrive;
        }

        protected virtual JointDrive SetJointDriveDamper(JointDrive jointDrive, float damper)
        {
            JointDrive newDrive = new JointDrive();
            newDrive.positionSpring = jointDrive.positionSpring;
            newDrive.positionDamper = damper;
            newDrive.maximumForce = jointDrive.maximumForce;
            return newDrive;
        }
        
        protected virtual JointDrive SetJointDrive(JointDrive jointDrive, float spring, float damper, float maximumForce)
        {
            JointDrive newDrive = new JointDrive();
            newDrive.positionSpring = spring;
            newDrive.positionDamper = damper;
            newDrive.maximumForce = maximumForce;
            return newDrive;
        }
    }
}
