﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class FreeSwingHinge : Hinge
    {
        public bool Restrained
        {
            set
            {
                interactionVolume.restrained = value;
            }
        }

        [SerializeField] protected bool inverseFreeSwing;

        public delegate void Open();
        public delegate void Close();

        public Open _Open;
        public Close _Close;

        [SerializeField] protected float openSpeed;
        [SerializeField] protected float closeSpeed;

        Vector3 currentDirection;
        Vector3 previousDirection;
        [SerializeField] protected Transform hinge;
        public bool locked;
        [SerializeField] protected float angleDeltaThreshold = 2f;

        bool prevHand;

        [SerializeField] protected bool lockHinge;

        public override Hand HingeHand
        {
            get { return base.HingeHand; }

            set
            {
                bool temp = value != HingeHand;

                base.HingeHand = value;

                if (lockHinge)
                    if (temp && value == HingeHand && HingeHand != null)
                        Unlock();

                if (lockHinge)
                    if (HingeHand == null)
                        Lock();
            }
        }

        protected override void Start()
        {
            base.Start();

            _Open += OpenSFX;
            _Close += CloseSFX;
        }

        protected override void Update()
        {
            base.Update();

            if (!HingeHand)
            {
                if (axis == TwitchExtension.Axis.x)
                {
                    if (prevHand)
                        previousDirection = hinge.up;

                    RotateWithAroundLocalXAxis();
                }

                if (axis == TwitchExtension.Axis.z)
                {
                    if (prevHand)
                        previousDirection = hinge.right;

                    RotateWithAroundLocalZAxis();
                }
            }

            prevHand = HingeHand;
        }

        protected void Lock()
        {
            bool temp = locked;

            switch (axis)
            {
                case TwitchExtension.Axis.x:
                    locked = transform.localEulerAngles.x < 3;
                    break;
                case TwitchExtension.Axis.y:
                    locked = transform.localEulerAngles.y < 3;
                    break;
                case TwitchExtension.Axis.z:
                    locked = transform.localEulerAngles.z < 3;
                    break;
            }

            if (!temp)
                if (locked)
                {
                    if (_Close != null)
                        _Close();
                }

            CloseSFX();
        }

        public void Unlock()
        {
            if (!locked) return;

            switch (axis)
            {
                case TwitchExtension.Axis.x:
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + 5, transform.localEulerAngles.y, transform.localEulerAngles.z);
                    break;
                case TwitchExtension.Axis.y:
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 5, transform.localEulerAngles.z);
                    break;
                case TwitchExtension.Axis.z:
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + 5);
                    break;
            }

            locked = false;

            interactionVolume.restrained = false;

            if (_Open != null)
                _Open();

            OpenSFX();
        }

        protected void RotateWithAroundLocalXAxis()
        {
            currentDirection = hinge.up;

            float difference = Vector3.Angle(previousDirection, currentDirection);

            Vector3 cross = Vector3.Cross(currentDirection, previousDirection).normalized;
            float dot = Vector3.Dot(cross, hinge.right);

            if (dot > 0 && difference > angleDeltaThreshold && transform.localEulerAngles.x > 0)
                transform.localEulerAngles = new Vector3(Mathf.Clamp(transform.localEulerAngles.x + (-difference * closeSpeed * dot * Time.deltaTime), 0, maxAngle), transform.localEulerAngles.y, transform.localEulerAngles.z);
            else if (transform.localEulerAngles.x < maxAngle && !locked)
                transform.localEulerAngles = new Vector3(Mathf.Clamp(transform.localEulerAngles.x + (openSpeed * Time.deltaTime), 0, maxAngle), transform.localEulerAngles.y, transform.localEulerAngles.z);

            if (transform.localEulerAngles.x < 1 && !locked)
            {
                locked = true;

                if (_Close != null)
                    _Close();
            }

            previousDirection = currentDirection;
        }

        protected void RotateWithAroundLocalZAxis()
        {
            currentDirection = hinge.right;

            float difference = Vector3.Angle(previousDirection, currentDirection);

            Vector3 cross = Vector3.Cross(currentDirection, previousDirection).normalized;
            float dot = Vector3.Dot(cross, hinge.forward) * (inverseFreeSwing ? -1 : 1);

            if (dot > 0 && difference > angleDeltaThreshold && transform.localEulerAngles.z > 0)
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Clamp(transform.localEulerAngles.z + (-difference * closeSpeed * dot * Time.deltaTime), 0, maxAngle));
            else if (transform.localEulerAngles.z < maxAngle && !locked)
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Clamp(transform.localEulerAngles.z + (openSpeed * Time.deltaTime), 0, maxAngle));

            if (transform.localEulerAngles.z < 1 && !locked)
            {
                locked = true;

                if (_Close != null)
                    _Close();
            }

            previousDirection = currentDirection;
        }
    }
}