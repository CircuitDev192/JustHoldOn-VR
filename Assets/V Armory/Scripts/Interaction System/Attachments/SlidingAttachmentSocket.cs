using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class SlidingAttachmentSocket : AttachmentSocket
    {
        [SerializeField] protected Slide slide;

        protected override void Start()
        {
            base.Start();

            if(!slide) slide = GetComponent<Slide>();
        }

        protected override void AttachPotentialItem(Attachment item)
        {
            Hand tempHand = item.PrimaryHand;

            slide.SetSlidePositionToTransform(tempHand.transform);

            base.AttachPotentialItem(item);

            slide.Restrain = false;

            slide.ForceStart(tempHand);

            slide.Restrain = true;
        }
    }
}
