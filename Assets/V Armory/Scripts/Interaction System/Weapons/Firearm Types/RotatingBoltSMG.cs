using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class RotatingBoltSMG : Firearm
    {
        protected RotatingBolt rotatingBolt;
        protected FirearmDoubleSlide handle;

        protected override void Start()
        {
            if (!rotatingBolt) rotatingBolt = GetComponentInChildren<RotatingBolt>();

            handle = rotatingBolt.slider as FirearmDoubleSlide;

            if (!handle)
            {
                handle = GetComponentInChildren<FirearmDoubleSlide>();
                rotatingBolt.slider = handle;
            }

            base.Start();

            rotatingBolt._RotateUp += handle.LockSlideStop;
            rotatingBolt._RotateDown += handle.UnlockSlideStop;

            rotatingBolt._RotateUp += slide.LockSlideStop;
            rotatingBolt._RotateDown += slide.UnlockSlideStop;
        }
    }
}