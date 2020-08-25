using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VArmory;

namespace VArmory
{
    public class AttachSafetyWhenBoltIsRotatedUp : MonoBehaviour
    {
        [SerializeField] protected RotatingBolt rotatingBolt;
        [SerializeField] protected Transform safety;
        Vector3 initialPos;
        Vector3 initialRot;
        Transform initialParent;

        private void Start()
        {
            rotatingBolt._DetachHinge += ParentSafety;
            rotatingBolt._GrabHinge += UnparentSafety;
            initialPos = transform.localPosition;
            initialRot = transform.localEulerAngles;
            initialParent = transform.parent;
        }

        void ParentSafety()
        {
            safety.parent = rotatingBolt.transform;
        }

        void UnparentSafety()
        {
            safety.parent = initialParent;
            transform.localPosition = initialPos;
            transform.localEulerAngles = initialRot;
        }
    }
}