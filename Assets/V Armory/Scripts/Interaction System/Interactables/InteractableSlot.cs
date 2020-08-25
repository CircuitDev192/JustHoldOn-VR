using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class InteractableSlot : MonoBehaviour
    {
        [SerializeField] protected Slot slot;
        [SerializeField] protected InteractionVolume iv;

        void Start()
        {
            slot = GetComponent<Slot>();
            iv = GetComponent<InteractionVolume>();

            slot._OnStore += UnrestrainOnStore;
            slot._OnUnstore += RestrainOnUnstore;

            iv._StartInteraction += GrabItem;
        }

        void UnrestrainOnStore(Item item) { iv.restrained = false; }

        void RestrainOnUnstore(Item item) { iv.restrained = true; }

        void GrabItem()
        {
            Hand interactingHand = iv.Hand;
            Item storedItem = slot.StoredItem;

            iv.StopInteraction();

            if (storedItem)
            {
                storedItem.PrimaryGrip.ForceStartInteraction(interactingHand);
            }
        }
    }
}