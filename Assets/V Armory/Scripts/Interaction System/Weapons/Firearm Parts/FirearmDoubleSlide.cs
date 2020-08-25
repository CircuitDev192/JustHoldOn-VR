using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class FirearmDoubleSlide : FirearmSlide
    {
        [SerializeField] protected FirearmSlide firearmSlide;

        bool lower;

        //1 == start
        // 0 == end

        void LateUpdate()
        {
            if (interactionPoint && !firearmSlide.InteractionPoint)
            {
                //If firearmSlides position is closer to start than this grab the firearmSlide too
                if (lower)
                {
                    firearmSlide.StopSlideAnimations();
                    firearmSlide.SetSlidePosition(slidePosition);
                    firearmSlide.GrabSlide(interactionPoint);
                    GrabSlide(interactionPoint);
                }
            }

            Lower();
        }

        public void Lower()
        {
            lower = slidePosition < firearmSlide.slidePosition;
        }       

        public override void DetachSlide()
        {
            if (firearmSlide.InteractionPoint)
                firearmSlide.DetachSlide();

            base.DetachSlide();
        }
    }
}