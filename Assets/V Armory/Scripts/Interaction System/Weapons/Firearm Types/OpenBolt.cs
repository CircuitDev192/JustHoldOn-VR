using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class OpenBolt : Firearm
    {
        protected override void Start()
        {
            base.Start();

            if (slide)
                slide._OnReachedStart += Fire;

        }

        protected override void SlideStop()
        {
            slide.SlideStop = true;
        }

        protected override void Fire()
        {
            slide.SlideStop = false;
            base.Fire();
        }
    }
}
