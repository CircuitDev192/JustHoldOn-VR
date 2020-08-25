using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class DoubleSlide : Slide
    {
        [SerializeField] protected FirearmSlide firearmSlide;

        protected override void Start()
        {
            base.Start();
        }

        protected override void PoseSlide()
        {
            if (interactionPoint && !firearmSlide.InteractionPoint)
                if (Mathf.Round(firearmSlide.slidePosition * 10) == Mathf.Round(slidePosition * 10))
                {
                    firearmSlide.StopSlideAnimations();
                    firearmSlide.GrabSlide(interactionPoint);
                    GrabSlide(interactionPoint);
                }
        }

        public override void DetachSlide()
        {
            if (firearmSlide.InteractionPoint)
                firearmSlide.DetachSlide();

            base.DetachSlide();
        }
    }
}