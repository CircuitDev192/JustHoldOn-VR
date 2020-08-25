using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class MultiItemSlot : Slot
    {
        [SerializeField] protected Slot secondarySlot;
        [SerializeField] protected Slot tertiarySlot;
        [SerializeField] protected int secondarySizeLimit;

        public override bool ValidItem(Item potentialItem)
        {
            if (potentialItem == null)
                return false;

            if (potentialItem.Size > sizeLimit)
                return false;

            if ((secondarySlot.HasItem || tertiarySlot.HasItem) && potentialItem.Size > secondarySizeLimit)
                return false;

            if (!HasTag(potentialItem))
                return false;

            if (HasUnacceptedTag(potentialItem))
                return false;

            return true;
        }

        public override Item StoredItem
        {
            set
            {
                if (value)
                {
                    if (value.Size > secondarySizeLimit)
                    {
                        secondarySlot.sizeLimit = -1;
                        tertiarySlot.sizeLimit = -1;
                    }
                    else
                    {
                        secondarySlot.sizeLimit = secondarySizeLimit;
                        tertiarySlot.sizeLimit = secondarySizeLimit;
                    }
                }
                else
                {
                    secondarySlot.sizeLimit = secondarySizeLimit;
                    tertiarySlot.sizeLimit = secondarySizeLimit;

                    offset.localPosition = Vector3.zero;
                    offset.localRotation = Quaternion.identity;
                }

                storedItem = value;

                if (value != null && _OnStore != null)
                    _OnStore(storedItem);

                if (value == null && _OnUnstore != null)
                    _OnUnstore(storedItem);
            }
        }
    }
}