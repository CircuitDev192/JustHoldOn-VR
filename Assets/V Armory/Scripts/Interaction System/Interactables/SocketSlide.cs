using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class SocketSlide : Slide
    {
        [SerializeField] protected SteamVR_Action_Boolean ejectInput;
        public SteamVR_Action_Boolean EjectInput { get { return ejectInput; } }

        [SerializeField] protected VArmoryInput.TouchPadDirection ejectTouchpadDirection;
        public VArmoryInput.TouchPadDirection EjectTouchpadDirection { get { return ejectTouchpadDirection; } }

        Vector3 initialSize;
        Vector3 initialCenter;

        public delegate void OnStartSlide(Item item);
        public OnStartSlide _OnStartSlide;

        public delegate void Detached();
        public Detached _OnDetach;

        public delegate void OnLoad(Item item);
        public OnLoad _OnLoad;

        public delegate void OnGrab();
        public OnGrab _OnGrab;

        [SerializeField] protected string[] acceptedTags;

        [ReadOnly] [SerializeField] protected Slot potentialSlot;
        [ReadOnly] [SerializeField] protected Item potentialSlider;

        protected Item lastItem;
        protected Hand lastHand;

        [ReadOnly] [SerializeField] protected Hand savedHand; //Find a better way to keep a reference of the hands

        [SerializeField] protected TwitchExtension.DotAxis primaryAttachDotAxis = TwitchExtension.DotAxis.up;
        [SerializeField] protected TwitchExtension.DotAxis secondaryAttachDotAxis = TwitchExtension.DotAxis.right;
        [Range(-1, 1)] [SerializeField] protected float primaryAttachDotThreshold = 0.75f;
        [Range(-1, 1)] [SerializeField] protected float secondaryAttachDotThreshold = 0.75f;
        [SerializeField] protected float attachDistanceThreshold = 0.1f;
        [SerializeField] protected float detachDistanceThreshold = 0.2f;
        [SerializeField] protected float detachSliderPositionThreshold = 0.9f;

        protected bool offsetInitialAttach = true;

        [SerializeField] protected bool loadFromSlot = false;

        BoxCollider triggerCollider;

        [SerializeField] protected Collider[] itemColliders = new Collider[0];
        [SerializeField] protected float ejectSpeed = 5;
        [SerializeField] protected float ejectAutoGrabDistance = 0.1f;

        public float EjectSpeed { get { return ejectSpeed; } }

        [SerializeField] protected Vector3 loadedCenter;
        [SerializeField] protected Vector3 loadedSize;

        [SerializeField] protected bool resetSlideOnLoad;

        protected void Awake()
        {
            _OnReachedEnd += LoadSlider;
            _OnReachedStart += DetachSlider;
        }

        protected override void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("ItemDetection");
            triggerCollider = GetComponent<Collider>() as BoxCollider;

            if (triggerCollider)
            {
                initialSize = triggerCollider.size;
                initialCenter = triggerCollider.center;

                if(loadedSize == Vector3.zero)
                {
                    loadedSize = triggerCollider.size;
                    loadedSize = triggerCollider.center;
                }
            }
            else
            {
                Debug.LogWarning("Please attach a box collider and set it as a trigger to the socket slide : " + name);
            }

            if (slideObject)
            {
                SetColliderLoaded();

                potentialSlider = slideObject.GetComponent<Item>();
                potentialSlot = potentialSlider.PrimaryHand ?? potentialSlider.SecondaryHand ?? potentialSlider.Slot;
            }
            else
            {
                Item tempItem = GetComponentInChildren<Item>();

                if (tempItem)
                {
                    SetColliderLoaded();
                    slideObject = tempItem.transform;
                    potentialSlider = tempItem;
                    potentialSlot = potentialSlider.PrimaryHand ?? potentialSlider.SecondaryHand ?? potentialSlider.Slot;
                }
            }

            if (potentialSlider)
            {
                StartSliding();
                LoadSlider();
            }

            if (!slideObject)
                interactionVolume.restrained = true;

            Transform tempSlide = slideObject;

            base.Start();

            slideObject = tempSlide;

            //interactionVolume.Highlight = null;
        }

        public override void GrabSlide()
        {
            if (_OnGrab != null)
                _OnGrab();

            if (interactionVolume)
            {
                potentialSlot = interactionVolume.Hand;
                savedHand = savedHand ? savedHand : interactionVolume.Hand;
            }

            if (eject != null)
            {
                StopCoroutine(eject);
                eject = null;
            }

            base.GrabSlide();
        }

        void SetColliderLoaded()
        {
            triggerCollider.center = loadedCenter;
            triggerCollider.size = loadedSize;
        }

        void SetColliderInitial()
        {
           triggerCollider.center = initialCenter;
           triggerCollider.size = initialSize;
        }

        protected override void Update()
        {
            if (slideObject)
                base.Update();

            AttachPotentialSlider();
            
            DetachSliderByDistance();

            AutoDropLastSlider();
        }
        
        protected virtual void AutoDropLastSlider()
        {
            if (lastHand && lastItem)
            {
                if (interactionVolume.StartInputID != null)

                    if (VArmoryInput.InputUp(interactionVolume.StartInputID, lastHand))
                    {
                        if (lastHand.StoredItem == lastItem)
                            lastItem.Detach();

                        lastHand = null;
                        lastItem = null;
                    }
            }
        }

        float removedTime;
        float newAfterRemovedThreshold = 0.15f;

        protected virtual IEnumerator OnTriggerEnter(Collider other)
        {
            if (Time.time - removedTime < newAfterRemovedThreshold) yield break;

            if (other.isTrigger)
                yield break;

            if (slideObject)
                yield break;

            if (other.gameObject.tag != "Interactable")
                yield break;

            Item tempAttachedItem = other.transform.GetComponent<Item>();

            if (tempAttachedItem)
                if (ItemHasAcceptedTag(tempAttachedItem))
                {
                    if (tempAttachedItem.PrimaryHand || tempAttachedItem.SecondaryHand || (tempAttachedItem.Slot && loadFromSlot))
                    {
                        potentialSlider = tempAttachedItem;
                        potentialSlot = potentialSlider.PrimaryHand ?? potentialSlider.SecondaryHand ?? potentialSlider.Slot;
                    }
                }
        }

        bool ItemHasAcceptedTag(Item item)
        {
            foreach (string tag in acceptedTags)
                if (item.HasTag(tag)) return true;

            return false;
        }

        protected Hand slidingHand;

        protected virtual void AttachPotentialSlider()
        {
            if (!potentialSlider)
                return;

            if (!potentialSlot)
                return;

            if (slideObject)
                return;

            float distance = Vector3.Distance(startPosition.position, potentialSlider.transform.position);

            if (distance < attachDistanceThreshold)
            {
                Vector3 tempPrimaryDesiredRotation = TwitchExtension.ReturnAxis(primaryAttachDotAxis, transform);
                Vector3 tempPrimaryCurrentRotation = TwitchExtension.ReturnAxis(primaryAttachDotAxis, potentialSlider.transform);

                Vector3 tempSecondaryDesiredRotation = TwitchExtension.ReturnAxis(secondaryAttachDotAxis, transform);
                Vector3 tempSecondaryCurrentRotation = TwitchExtension.ReturnAxis(secondaryAttachDotAxis, potentialSlider.transform);

                if (Vector3.Dot(tempPrimaryCurrentRotation, tempPrimaryDesiredRotation) > primaryAttachDotThreshold
                && Vector3.Dot(tempSecondaryDesiredRotation, tempSecondaryCurrentRotation) > secondaryAttachDotThreshold)
                {
                    StartSliding();
                }
            }
        }

        IEnumerator SnapPose(Hand tempHand)
        {
            if (!tempHand) yield break;

            bool leftHand = tempHand.inputSource == SteamVR_Input_Sources.LeftHand;
            var tempPoser = leftHand ? interactionVolume.LeftHandPoser : interactionVolume.RightHandPoser;

            if (leftHand)
            {
                interactionVolume.LeftHandPoser = snapPoser;
            }
            else
            {
                interactionVolume.RightHandPoser = snapPoser;
            }

            yield return new WaitForEndOfFrame();

            if (leftHand)
            {
                interactionVolume.LeftHandPoser = tempPoser;
            }
            else
            {
                interactionVolume.RightHandPoser = tempPoser;
            }
        }

        public SteamVR_Skeleton_Poser snapPoser;

        protected virtual void StartSliding()
        {
            if (potentialSlot) StartCoroutine(SnapPose(potentialSlot as Hand));

            interactionVolume.restrained = false;

            potentialSlider.Restrained = true;

            potentialSlider.dropStack = false;
            potentialSlider.DetachWithOutStoring();
            if (potentialSlot) potentialSlot.UnstoreItem();
            potentialSlider.dropStack = true;

            Destroy(potentialSlider.Rb);

            potentialSlider.SetPhysics(true, false, true, false);

            potentialSlider.transform.SetParent(endPosition, true);

            if (_OnStartSlide != null)
                _OnStartSlide(potentialSlider);

            slideObject = potentialSlider.transform;
            slideObject.rotation = startPosition.rotation;

            if (potentialSlot)
            {
                Hand tempHand = potentialSlot.GetType() == typeof(Hand) ? potentialSlot as Hand : null;

                if (tempHand && !interactionPoint)
                {
                    if (interactionVolume)
                        interactionVolume.ForceStartInteraction(tempHand);
                    else
                        GrabSlide(tempHand.transform);

                    potentialSlot.StoredItem = null;
                }
                else
                    GrabSlide(potentialSlot.transform);
            }

            if (offsetInitialAttach) { sliderOffset = 1f; }

            interactionVolume.handRoot = slideObject;
        }

        protected virtual void LoadSlider()
        {
            SetColliderLoaded();

            slideObject.localPosition = Vector3.zero;
            slideObject.transform.rotation = endPosition.rotation;

            if (interactionVolume)
                interactionVolume.StopInteraction();
            else
                DetachSlide();

            if(potentialSlot)
                if (potentialSlot.GetType() == typeof(Hand))
                    (potentialSlot as Hand).GrabFromStack();

            potentialSlot = null;

            if (_OnLoad != null)
                _OnLoad(potentialSlider);

            if (resetSlideOnLoad)
                ClearSlider();
            else if(potentialSlider.PrimaryGrip.Highlight)
                interactionVolume.Highlight = potentialSlider.PrimaryGrip.Highlight;
        }

        protected void DetachSliderByDistance()
        {
            if (potentialSlot)
            {
                float distance = Vector3.Distance(startPosition.position, potentialSlot.transform.position);

                if (distance >= detachDistanceThreshold && slidePosition >= detachSliderPositionThreshold)
                    DetachSlider();
            }
        }

        protected void DetachSlider() 
        {
            if (!slideObject)
                return;

            potentialSlider.Restrained = false;
            potentialSlider.transform.SetParent(null);

            //Auto Equip
            //Clear Slider

            if (interactionVolume)
                interactionVolume.StopInteraction();
            else
                DetachSlide();

            if (!potentialSlider.Rb)
            {
                potentialSlider.rb = potentialSlider.gameObject.AddComponent<Rigidbody>();
            }

            if (AutoEquipRemovedSlide() == false)
                if (AutoEquipEjectedSlide(savedHand) == false)
                {
                    potentialSlider.SetPhysics(potentialSlider.OnDetach);
                }

            ClearSlider();

            removedTime = Time.time;

            interactionVolume.restrained = true;
            interactionVolume.HighlightIsActive = false;
        }

        public void ClearSlider()
        {
            slideObject = null;
            potentialSlider = null;
            potentialSlot = null;
            interactionPoint = null;

            ResetSlider();
            SetColliderInitial();
            interactionVolume.Highlight = null;
        }

        protected override void ResetSlider()
        {
            onReachedEndHysteresis = true;
            onReachedStartHysteresis = false;
            base.ResetSlider();
        }

        bool AutoEquipRemovedSlide()
        {
            bool interactionVolumeHand = interactionVolume ? interactionVolume.Hand : false;

            if (interactionVolumeHand)
            {
                potentialSlider.Attach(interactionVolume.Hand);
                SetForAutoDrop(interactionVolume.Hand);
            }
            else if (potentialSlot)
            {
                if (potentialSlot.GetType() == typeof(Hand))
                {
                    Hand tempHand = potentialSlot as Hand;
                    StartCoroutine(SnapPose(tempHand, potentialSlider));
                    potentialSlider.Attach(tempHand);
                    SetForAutoDrop(tempHand);
                }
                else if (!potentialSlot.HasItem)
                    potentialSlider.StoreOnSlot(potentialSlot);
                else
                    return false;
            }
            else
                return false;

            return true;
        }

        IEnumerator SnapPose(Hand hand, Item item)
        {
            if (!hand) yield break;

            bool leftHand = hand.inputSource == SteamVR_Input_Sources.LeftHand;
            var tempPoser = leftHand ? item.PrimaryGrip.LeftHandPoser : item.PrimaryGrip.RightHandPoser;

            if (leftHand)
            {
                item.PrimaryGrip.LeftHandPoser = snapPoser;
            }
            else
            {
                item.PrimaryGrip.RightHandPoser = snapPoser;
            }

            yield return new WaitForEndOfFrame();

            if (leftHand)
            {
                item.PrimaryGrip.LeftHandPoser = tempPoser;
            }
            else
            {
                item.PrimaryGrip.RightHandPoser = tempPoser;
            }
        }

        void SetForAutoDrop(Hand hand)
        {
            if(interactionVolume.StartInputID != null)
                if(VArmoryInput.Input(interactionVolume.StartInputID, hand))
                {
                    lastHand = hand;
                    lastItem = potentialSlider;
                }
        }

        bool AutoEquipEjectedSlide(Hand hand)
        {
            if (!hand)
                return false;

            if (ValidSlot(hand))
                return true;

            if (ValidSlot(hand.Sibling))
                return true;

            return false;
        }

        bool ValidSlot(Hand hand)
        {
            if (interactionVolume.StartInputID == null)
                return false;

            if (Vector3.Distance(startPosition.position, hand.transform.position) <= ejectAutoGrabDistance)
                if (!hand.HasItem && !hand.IsInteracting
                && VArmoryInput.Input(interactionVolume.StartInputID, hand))
                {
                    potentialSlider.Attach(hand);
                    lastHand = hand;
                    lastItem = potentialSlider;
                    return true;
                }

            return false;
        }

        IEnumerator eject;

        public virtual void EjectSlider()
        {
            if (eject == null)
                StartCoroutine(eject = EjectSliderCoroutine());
        }

        protected virtual IEnumerator EjectSliderCoroutine()
        {
            if (!potentialSlider)
                yield break;

            if (interactionVolume.Hand)
                yield break;

            Item tempSlider = potentialSlider;

            float initialTime = Time.time;

            do
            {
                SetSlidePosition((Time.time - initialTime) * ejectSpeed);
                yield return new WaitForFixedUpdate();
            }
            while ((Time.time - initialTime) * ejectSpeed < 1);

            _OnReachedStart();
            eject = null;

            yield return new WaitForSeconds(0.333f);
        }

        //yield return new WaitForFixedUpdate();
        //Slot stackedHand = null;

        //if (potentialSlider.Stackable)
        //{
        //	(hand ?? potentialHand).AddItemsToStack(potentialSlider, false);
        //	stackedHand = hand ?? potentialHand;
        //}
        //else

        //Haptic feedback and sfx

        //yield return new WaitForFixedUpdate();
        //if (stackedHand)
        //	stackedHand.PositionStack();
        //yield return new WaitForFixedUpdate();
        //potentialSlider.Col.enabled = true;
    }
}