using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class Grenade : ExplosiveItem
    {
        [SerializeField] protected SocketSlide pinSlide;

        [SerializeField] protected Collider handleCol;
        [SerializeField] protected Rigidbody handleRb;
        [SerializeField] protected float handleRotateSpeed;
        [SerializeField] protected float handleEjectForce;
        public Transform pin;
        public Transform ring;

        [SerializeField] protected float forceThreshold = -1;

        [SerializeField] protected VArmoryInput.TouchPadDirection ejectHandleTouchpadDirection;
        [SerializeField] protected SteamVR_Action_Boolean ejectHandleInput;

        protected override void Start()
        {
            base.Start();
            pinSlide = GetComponentInChildren<SocketSlide>();
            explosive = GetComponent<Explosive>();
            pinSlide._OnReachedStart += Arm;

            IgnoreCollision(pin.GetComponent<Collider>(), true);

            if(handleCol)
            Physics.IgnoreCollision(col, handleCol, true);
        }

        protected override void PrimaryDrop()
        {
            base.PrimaryDrop();

            if (armed) { StartCoroutine(EjectHandle()); }
        }

        protected override void Update()
        {
            base.Update();

            if (!PrimaryHand) return;

            if (ejectHandleInput != null)
                LocalInputUp(EjectHandleWrapper, ejectHandleInput);

            if (LocalInputUp(null, PrimaryHand.TouchpadInput))
                TouchPadInput(EjectHandleWrapper, ejectHandleTouchpadDirection);
        }

        void EjectHandleWrapper()
        {
            if (armed) StartCoroutine(EjectHandle());
        }

        public override void Arm() { armed = true; }

        void OnCollisionEnter(Collision col)
        {
            if (col.relativeVelocity.magnitude > forceThreshold && armed)
                Explode();
        }

        IEnumerator EjectHandle()
        {
            if (handleRb)
            {
                float angle = 0;

                while (angle < 90)
                {
                    angle += handleRotateSpeed * Time.deltaTime;
                    handleRb.transform.Rotate(Vector3.right, -handleRotateSpeed * Time.deltaTime);
                    yield return null;
                }

                handleRb.isKinematic = false;
                handleRb.useGravity = true;
                handleCol.transform.parent = null;
                handleCol.isTrigger = false;

                handleRb.AddForceAtPosition(transform.up * handleEjectForce, transform.position, ForceMode.Impulse);
            }

            yield return ExplodeRoutine();
        }
    }
}