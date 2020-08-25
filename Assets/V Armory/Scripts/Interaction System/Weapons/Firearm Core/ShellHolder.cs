using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class ShellHolder : MonoBehaviour
    {

        [SerializeField] protected string shellTag;
        [SerializeField] List<Transform> slots = new List<Transform>();

        GameObject old;
        Bullet potentialShell;

        [Range(-1, 1)] [SerializeField] protected float primaryAttachDot;
        [Range(-1, 1)] [SerializeField] protected float secondaryAttachDot;

        public TwitchExtension.DotAxis primaryAttachDotAxis = TwitchExtension.DotAxis.forward;
        public TwitchExtension.DotAxis secondaryAttachDotAxis = TwitchExtension.DotAxis.right;

        int shellCount;

        public InteractionVolume interactionVolume;

        void Start()
        {
            interactionVolume._StartInteraction += GrabShell;
        }

        public void GrabShell()
        {
            Hand hand = interactionVolume.Hand;
            Transform tempSlot = null;

            foreach (Transform slot in slots)
                if (slot.childCount != 0)
                    tempSlot = slot;

            if (!tempSlot)
                return;

            Bullet tempShell = tempSlot.GetComponentInChildren<Bullet>();
            tempShell.Restrained = false;
            tempShell.Attach(hand);
            old = tempShell.gameObject;
            interactionVolume.StopInteraction();
            shellCount--;

            if (shellCount == 0)
                interactionVolume.restrained = true;
        }

        void Update()
        {
            if (!potentialShell)
                return;

            Transform tempSlot = null;

            foreach (Transform slot in slots)
                if (slot.childCount == 0)
                    tempSlot = slot;

            Vector3 tempPrimaryAxis = TwitchExtension.ReturnAxis(primaryAttachDotAxis, tempSlot);
            Vector3 tempSecondaryAxis = TwitchExtension.ReturnAxis(secondaryAttachDotAxis, tempSlot);

            Vector3 tempAttachmentPrimaryAxis = TwitchExtension.ReturnAxis(primaryAttachDotAxis, potentialShell.transform);
            Vector3 tempAttachmentSecondaryAxis = TwitchExtension.ReturnAxis(secondaryAttachDotAxis, potentialShell.transform);

            if (Vector3.Dot(tempPrimaryAxis, tempAttachmentPrimaryAxis) >= primaryAttachDot &&
                Vector3.Dot(tempSecondaryAxis, tempAttachmentSecondaryAxis) >= secondaryAttachDot)
            {
                potentialShell.DetachWithOutStoring();
                potentialShell.Restrained = true;
                potentialShell.SetPhysics(true, false, true, true);
                potentialShell.transform.parent = tempSlot;
                potentialShell.transform.localPosition = Vector3.zero;
                potentialShell.transform.localEulerAngles = Vector3.zero;
                potentialShell = null;
                shellCount++;
                interactionVolume.restrained = false;
                interactionVolume.StopInteraction();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == old)
                return;

            if (other.gameObject.tag != "Interactable")
                return;

            Bullet tempBullet = other.GetComponent<Bullet>();

            if (tempBullet)
                if (tempBullet.HasTag(shellTag))
                    if (tempBullet.PrimaryHand && !tempBullet.Spent)
                    {
                        potentialShell = tempBullet;
                    }
            
        }

        void OnTriggerExit(Collider other) { if (other.gameObject == old) old = null; }
    }
}
