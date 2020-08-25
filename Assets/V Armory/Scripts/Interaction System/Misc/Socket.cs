using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Socket : MonoBehaviour
    {
        [SerializeField] protected List<string> acceptedTags = new List<string>();

        [SerializeField] protected float attachDistance;

        [SerializeField] [Range(0, 1)] protected float primaryDotThreshold;
        [SerializeField] [Range(0, 1)] protected float secondaryDotThreshold;

        [SerializeField] protected DotAxis socketPrimaryDotAxis;
        [SerializeField] protected DotAxis socketSecondaryDotAxis;

        [SerializeField] protected DotAxis attachmentPrimaryDotAxis;
        [SerializeField] protected DotAxis attachmentSecondaryDotAxis;

        [ReadOnly] protected List<Item> potentialAttachments = new List<Item>();
        [ReadOnly] protected Item attachment;
        [ReadOnly] protected Item oldAttachment;

        [SerializeField] protected bool attachFromSlot;
        [SerializeField] protected bool requireHeld;

        protected bool Full { get { return attachment; } }

        public enum DotAxis
        {
            right,
            left,
            up,
            down,
            forward,
            back
        }

        Vector3 ReturnAxis(DotAxis axis, Transform transform)
        {
            switch (axis)
            {
                case DotAxis.left:
                    return -transform.right;
                case DotAxis.right:
                    return transform.right;
                case DotAxis.up:
                    return transform.up;
                case DotAxis.down:
                    return -transform.up;
                case DotAxis.forward:
                    return transform.forward;
                case DotAxis.back:
                    return -transform.forward;
                default:
                    return Vector3.zero;
            }
        }

        protected virtual bool HasTag(Item gameObj)
        {
            for (int i = 0; i < acceptedTags.Count; i++)
                if(gameObj.HasTag(acceptedTags[i]))
                    return true;

            return false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Interactable")
            {
                Item tempItem = other.GetComponent<Item>();

                if (tempItem)
                    if(HasTag(tempItem))
                        potentialAttachments.Add(tempItem);
            }
        }

        protected virtual bool AttachRequirements() { return true; }

        protected virtual void AttemptAttach(Item item)
        {
            if (Full) return;

            if (!(((item.PrimaryHand ^ item.SecondaryHand) && requireHeld) || (item.Slot && attachFromSlot))) return;

            if (Vector3.Distance(transform.position, item.transform.position) > attachDistance) return;

            if (!AttachRequirements()) return;

            Vector3 tempSocketPrimaryDot = ReturnAxis(socketPrimaryDotAxis, transform);
            Vector3 tempSocketSecondaryDot = ReturnAxis(socketSecondaryDotAxis, transform);

            Vector3 tempItemPrimaryDot = ReturnAxis(attachmentPrimaryDotAxis, transform);
            Vector3 tempItemSecondaryDot = ReturnAxis(attachmentSecondaryDotAxis, transform);

            if (Vector3.Dot(tempSocketPrimaryDot, tempItemPrimaryDot) < primaryDotThreshold ||
                Vector3.Dot(tempSocketSecondaryDot, tempItemSecondaryDot) < secondaryDotThreshold)
                return;

            Attach(item);
        }

        protected virtual void Attach(Item item)
        {
            attachment = item;
            attachment.DetachWithOutStoring();

            //position
            //rotation
            //parent
            //physics
            //visibility
        }

        protected virtual void Detach()
        {
            attachment = null;
        }

        protected virtual void Detach(Slot slot)
        {
            Detach();
            attachment.StoreOnSlot(slot);
        }
    }
}