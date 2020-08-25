using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class RevolverSpeedLoader : MonoBehaviour
    {

        protected Bullet potentialBullet;

        [SerializeField] protected string bulletTag;

        [Range(-1, 1)] [SerializeField] protected float primaryAttachDot;
        [Range(-1, 1)] [SerializeField] protected float secondaryAttachDot;

        [SerializeField] protected TwitchExtension.DotAxis itemPrimaryAttachAxis;
        [SerializeField] protected TwitchExtension.DotAxis itemSecondaryAttachAxis;

        public TwitchExtension.DotAxis primaryAttachDotAxis = TwitchExtension.DotAxis.forward;
        public TwitchExtension.DotAxis secondaryAttachDotAxis = TwitchExtension.DotAxis.right;

        [SerializeField] protected List<Transform> slots = new List<Transform>();

        [SerializeField] protected InteractionVolume interactonVolume;

        void Start()
        {
            //interactonVolume = GetComponentInChildren<InteractionVolume>();
            //interactonVolume._StartInteraction += GrabRound;
        }

        Vector3 scale;

        void Update()
        {
            if (!potentialBullet)
                return;

            Vector3 tempPrimaryAxis = TwitchExtension.ReturnAxis(primaryAttachDotAxis, transform);
            Vector3 tempSecondaryAxis = TwitchExtension.ReturnAxis(secondaryAttachDotAxis, transform);

            Vector3 tempAttachmentPrimaryAxis = TwitchExtension.ReturnAxis(itemPrimaryAttachAxis, potentialBullet.transform);
            Vector3 tempAttachmentSecondaryAxis = TwitchExtension.ReturnAxis(itemSecondaryAttachAxis, potentialBullet.transform);

            if (Vector3.Dot(tempPrimaryAxis, tempAttachmentPrimaryAxis) >= primaryAttachDot &&
                Vector3.Dot(tempSecondaryAxis, tempAttachmentSecondaryAxis) >= secondaryAttachDot)
            {
                potentialBullet.DetachWithOutStoring();
                potentialBullet.Restrained = true;
                potentialBullet.SetPhysics(true, false, true, true);

                foreach (Transform slot in slots)
                    if (slot.childCount == 0)
                    {
                        scale.x *= 1 / slot.lossyScale.x;
                        scale.y *= 1 / slot.lossyScale.y;
                        scale.z *= 1 / slot.lossyScale.z;
                        potentialBullet.transform.SetParent(slot);
                        break;
                    }

                potentialBullet.transform.localPosition = Vector3.zero;
                potentialBullet.transform.localEulerAngles = Vector3.zero;
                potentialBullet.transform.localScale = scale;

                potentialBullet = null;
            }
        }

        void GrabRound()
        {
            Bullet tempBullet = GetComponentInChildren<Bullet>();

            if (!tempBullet)
                return;

            tempBullet.Restrained = false;
            tempBullet.Attach(interactonVolume.Hand);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Interactable")
                return;

            Bullet tempBullet = other.GetComponent<Bullet>();

            if (tempBullet)
                if (tempBullet.HasTag(bulletTag))
                    if (tempBullet.PrimaryHand && !tempBullet.Spent)
                    {
                        potentialBullet = tempBullet;
                        scale = potentialBullet.transform.localScale;
                    }
        }
    }
}