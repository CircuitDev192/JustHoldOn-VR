﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] protected Transform globalOffset;
        public virtual Transform GlobalOffset { get { return globalOffset; } }

        [SerializeField] protected Transform offset;
        public virtual Transform Offset { get { return offset; } }

        protected Rigidbody offsetRigidbody;
        public Rigidbody OffsetRigidbody
        {
            get { return offsetRigidbody; }
        }

        [SerializeField] public int sizeLimit;
        [SerializeField] protected List<string> AcceptedTags = new List<string>();
        [SerializeField] protected List<string> UnacceptedTags = new List<string>();

        [SerializeField] protected bool poseItem;

        [HideInInspector] public List<Item> stackedItems = new List<Item>();
        public bool StackIsFull { get { return stackedItems.Count >= 5; } }
        public bool StackIsEmpty { get { return stackedItems.Count == 0; } }

        [ReadOnly] [SerializeField] protected Item storedItem;

        public virtual Item StoredItem
        {
            get { return storedItem; }
            set
            {
                storedItem = value;

                if (value != null && _OnStore != null) 
                    _OnStore(storedItem);

                if (value == null && _OnUnstore != null)
                    _OnUnstore(storedItem);
            }
        }

        public delegate void OnStore(Item item);
        public OnStore _OnStore;

        public delegate void OnUnstore(Item item);
        public OnUnstore _OnUnstore;

        private void Start()
        {
            gameObject.tag = "Slot";
            gameObject.layer = LayerMask.NameToLayer("ItemDetection");
        }

        public bool HasItem { get { return storedItem; } }

        public bool StackableStoredItem { get { return storedItem ? storedItem.Stackable : false; } }

        public void UnstoreItemLight(Item item)
        {
            if (storedItem == item)
                storedItem = null;
        }

        public void UnstoreItem()
        {
            if (storedItem)
                storedItem.DetachSlot();
        }

        [SerializeField] protected MeshRenderer mesh;
        [SerializeField] protected Material highlightMaterial;

        protected Material unhighlightedMat;

        public void _Highlight()
        {
            if (mesh)
                if (mesh.sharedMaterial != highlightMaterial)
                {
                    if (unhighlightedMat == null)
                        unhighlightedMat = mesh.sharedMaterial;

                    mesh.sharedMaterial = highlightMaterial;
                }
        }

        public void _Unhighlight()
        {
            if (mesh)
            {
                if (mesh.sharedMaterial != unhighlightedMat)
                    mesh.sharedMaterial = unhighlightedMat;
            }
        }

        protected virtual void SetVisibility(bool visible)
        {
            MeshRenderer tempMesh = GetComponent<MeshRenderer>();

            if (tempMesh)
                tempMesh.enabled = visible;
        }
        
        protected virtual bool HasTag(Item item)
        {
            if (AcceptedTags.Count == 0)
                return true;

            for (int i = 0; i < AcceptedTags.Count; i++)
                if(item.HasTag(AcceptedTags[i]))
                    return true;

            return false;
        }

        protected virtual bool HasUnacceptedTag(Item item)
        {

            if (UnacceptedTags.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < UnacceptedTags.Count; i++)
            {
                if (item.HasTag(UnacceptedTags[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool ValidItem(Item potentialItem)
        {
            if (potentialItem == null)
                return false;

            if (potentialItem.Size > sizeLimit)
                return false;

            if (!HasTag(potentialItem))
                return false;

            if (HasUnacceptedTag(potentialItem))
                return false;

            return true;
        }

        protected virtual void FixedUpdate()
        {
            if (storedItem)
            {
                if (storedItem.transform.parent != offset)
                    return;

                if (poseItem)
                    storedItem.Pose();
            }
        }

        public virtual bool AddToStack(Item newStackable)
        {
            if (!HasItem)
            {
                newStackable.Restrained = false;
                newStackable.StoreOnSlot(this);
                return false;
            }

            if (!newStackable.Stackable)
                return false;

            if (newStackable.StackableID != storedItem.StackableID)
                return false;

            if (StackIsFull)
                return false;

            newStackable.transform.SetParent(offset, true);
            Vector3 newPosition = newStackable.PositionOffset;
            newPosition.z -= (stackedItems.Count + 1) * 0.025f; //Make a variable for stack offset
            newPosition.y -= (stackedItems.Count + 1) * 0.025f;
            newStackable.transform.localPosition = newPosition;
            newStackable.transform.localRotation = storedItem.transform.localRotation;
            newStackable.SetPhysics(true, false, true, true);
            newStackable.Restrained = true;
            stackedItems.Add(newStackable);
            return true;
        }
    }
}