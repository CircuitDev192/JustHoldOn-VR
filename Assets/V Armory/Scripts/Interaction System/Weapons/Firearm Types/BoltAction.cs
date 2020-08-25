using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class BoltAction : Firearm
    {

        [SerializeField] protected RotatingBolt rotatingBolt;

        protected override bool FirePreconditions()
        {
            if (chambers[selectedChamberIndex].Bullet)
                if (!chambers[selectedChamberIndex].Bullet.Spent
                    && slide.slidePosition >= 1 - rotatingBolt.slideTolerance
                    && rotatingBolt.transform.localEulerAngles.z <= rotatingBolt.hingeTolerance)
                    return true;

            return false;
        }

        protected override void ChamberBullet(Chamber chamber)
        {
            if (slide.slidePosition < 0.5f)
                base.ChamberBullet(chamber);
        }
    }
}