using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Backpack : Item
    {
        protected List<Item> items = new List<Item>();
        protected List<InteractionVolume> interactionVolumes = new List<InteractionVolume>();

        protected override void Start()
        {
            base.Start();
            Slot[] slots = GetComponentsInChildren<Slot>();

            foreach (Slot slot in slots)
            {
                slot._OnStore += SetPhysicsForStoredItem;
                slot._OnUnstore += SetPhysicsForUnstoredItem;
            }
        }

        void SetPhysicsForStoredItem(Item item)
        {
            item.IgnoreCollision(col, true);
            items.Add(item);
        }

        void SetPhysicsForUnstoredItem(Item item)
        {
            item.IgnoreCollision(col, false);
            items.Remove(item);
        }
    }
}
