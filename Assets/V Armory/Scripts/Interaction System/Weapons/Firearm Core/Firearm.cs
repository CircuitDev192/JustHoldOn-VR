using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Firearm : Item
    {
        [Space(10)]

        [Header("Firearm Settings")]

        protected Transform muzzle;
        [SerializeField] protected FirearmSlide slide;
        protected Trigger trigger;
        protected FireSelector fireSelector;
        protected SocketSlide magWell;
        protected Magazine magazine;

        protected List<Chamber> chambers = new List<Chamber>();
        protected int selectedChamberIndex;

        [SerializeField] protected RecoilManager recoilManager;
        protected FirearmSFX_Manager firearmAudioManager;

        [SerializeField] protected bool stackTriggerPulls;
        [SerializeField] protected bool slideStopOnEmptyMag;

        [SerializeField] protected GameObject muzzleFlash;
        [SerializeField] protected float muzzleVelocity;
        [SerializeField] protected float spread;

        [SerializeField] protected float slideEjectVelocity;

        protected bool enableSlideAudio = true;

        public bool allowOffHandStabilization = false;
        public float offhandStabilizationDistance = 0f;
        public Hand[] hands;
        [SerializeField] protected bool isMountedWeapon = false;
        public StaticPosForHMG staticPosScript;

        protected override void Start()
        {
            base.Start();

            if (isMountedWeapon)
            {
                staticPosScript = GetComponent<StaticPosForHMG>();
            }

            recoilManager = GetComponent<RecoilManager>();
            recoilManager.SetSelf(this);

            if (!slide) slide = GetComponentInChildren<FirearmSlide>();
            if (!magWell) magWell = GetComponentInChildren<SocketSlide>();
            if (!trigger) trigger = GetComponentInChildren<Trigger>();
            if (!fireSelector) fireSelector = GetComponentInChildren<FireSelector>();
            if (!muzzle) muzzle = transform.Find("Muzzle");
            if (!muzzle) muzzle = transform.Find("muzzle");
            if (!magazine) magazine = GetComponentInChildren<Magazine>();

            if (!firearmAudioManager) firearmAudioManager = GetComponent<FirearmSFX_Manager>();
            if (!firearmAudioManager) firearmAudioManager = gameObject.AddComponent<FirearmSFX_Manager>();

            if (virtualStock)
            {
                virtualStock._StartVirtualStock += StartVirutalStock;
                virtualStock._StopVirtualStock += StopVirtualStock;
            }

            if (chambers.Count == 0)
            {
                Chamber[] tempChambers = GetComponentsInChildren<Chamber>();

                foreach (Chamber tempChamber in tempChambers)
                {
                    chambers.Add(tempChamber);
                    tempChamber._LoadBullet += ChamberBullet;
                }
            }

            if (magWell)
            {
                magWell._OnGrab += RemoveMag;
                magWell._OnLoad += LoadMag;
                magWell._OnReachedStart += EjectMagVelocity;
                magWell._OnReachedStart += RemoveMag;

                magWell._OnReachedStart += DetachMagSFX;

                magWell._OnStartSlide += RestrainTapedMag;
            }

            if (slide)
            {
                slide._OnReachedStart += ChamberRoundFromMagazine;

                slide._PulledPassedSlideStop += SlideStop;
                slide._RestingOnSlideStop += RestOnSlideStop;

                slide._OnReachedEnd += EjectCartridge;
                //slide._OnReachedEnd += CatchBullet;

                slide._OnReachedStart += SlideFowardAudio;
                slide._OnReachedEnd += SlideBackAudio;
            }

            if (trigger)
            {
                trigger._TriggerPulled += PullTrigger;
                trigger._TriggerHeld += HoldTrigger;
                trigger._TriggerReleased += ReleaseTrigger;
            }

            if (slide && fireSelector)
                slide.minSliderDistance = fireSelector.FireMode == FireSelector._FireMode.safety ? fireSelector.SafetySlideLimit > 0 ? fireSelector.SafetySlideLimit : slide.minSliderDistance : 0;
            else if (slide)
                slide.minSliderDistance = 0;
        }

        protected virtual void SlideFowardAudio()
        {
            if (!enableSlideAudio)
            {
                enableSlideAudio = true;
                return;
            }

            if (firearmAudioManager) firearmAudioManager.PlayRandomAudioClip(firearmAudioManager.actionFowardSounds, slide.transform.position);
        }

        protected virtual void SlideBackAudio()
        {
            if (!enableSlideAudio)
                return;

            if (firearmAudioManager) firearmAudioManager.PlayRandomAudioClip(firearmAudioManager.actionBackSounds, slide.transform.position);
        }

        bool magOnPressDown;

        protected override void Update()
        {
            base.Update();

            recoilManager.ApplyRecoil();

            
            if (!PrimaryHand)
            {
                if(isMountedWeapon) staticPosScript.ResetPos();
                return;
            }
            

            if (trigger) trigger.PoseTrigger(PrimaryHand.TriggerRotation);

            LocalInputDown(TouchpadDownInput, PrimaryHand.TouchpadInput);

            LocalInputUp(TouchpadUpInput, PrimaryHand.TouchpadInput);
        
            if (LocalInputUp(null, PrimaryHand.TriggerInput))
            {
                releasedTriggerAfterPickUp = true;
            }

            LocalEjectMagInput();
            LocalFireSelectorInput();
            LocalSlideStopInput();
            
        }

        protected virtual void LocalFireSelectorInput()
        {
            if (fireSelector)
                if (fireSelector.FireSelectorInput != null)
                    if(LocalInputDown(null, fireSelector.FireSelectorInput))
                    {
                        float tempSlidePosition = slide ? slide.slidePosition : Mathf.Infinity;

                        if ((fireSelector.NextFireMode() == FireSelector._FireMode.safety && tempSlidePosition >= fireSelector.SafetySlideLimit)
                            || fireSelector.NextFireMode() != FireSelector._FireMode.safety)
                        {
                            fireSelector.SwitchFireMode();
                        }
                    }
        }

        protected virtual void LocalSlideStopInput()
        {
            if(slide)
                if (slide.SlideStopInput != null)
                    if (LocalInputDown(null, slide.SlideStopInput))
                    {
                        if (slide)
                        {
                            if (fireSelector)
                                slide.minSliderDistance = fireSelector.FireMode == FireSelector._FireMode.safety ? fireSelector.SafetySlideLimit > 0 ? fireSelector.SafetySlideLimit : slide.minSliderDistance : 0;

                            slide.SlideStopTouchpadInput();
                        }
                    }
        }

        protected virtual void LocalEjectMagInput()
        {
            if (magWell)
                if (magWell.EjectInput != null)
                {
                    if(LocalInputDown(null, magWell.EjectInput))
                        magOnPressDown = magazine;

                    LocalInputUp(EjectMagazine, magWell.EjectInput);
                }
        }

        protected virtual void TouchpadUpInput()
        {   
            if(magWell)
            TouchPadInput(EjectMagazine, magWell.EjectTouchpadDirection);
        }

        Magazine ejectMag;

        void EjectMagazine()
        {
            if (magWell && magOnPressDown)
            {
                magWell.EjectSlider();
                ejectMag = magazine;
                magazine = null;
                caughtRoundFromMagazine = false;
            }

            magOnPressDown = false;
        }

        protected virtual void TouchpadDownInput()
        {
            magOnPressDown = magazine;

            float tempSlidePosition = slide ? slide.slidePosition : Mathf.Infinity;

            if (fireSelector)
                if ((fireSelector.NextFireMode() == FireSelector._FireMode.safety && tempSlidePosition >= fireSelector.SafetySlideLimit)
                    || fireSelector.NextFireMode() != FireSelector._FireMode.safety)
                {
                    TouchPadInput(fireSelector.SwitchFireMode, fireSelector._TouchPadDirection);
                }

            if (slide)
            {
                if (fireSelector)
                    slide.minSliderDistance = fireSelector.FireMode == FireSelector._FireMode.safety ? fireSelector.SafetySlideLimit > 0 ? fireSelector.SafetySlideLimit : slide.minSliderDistance : 0;

                TouchPadInput(slide.SlideStopTouchpadInput, slide.SlideStopTouchpadDirection);
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();


            //USE A SPHERE COLLIDER ON EACH HAND, IF THEY COLLIDE, AND ONE HAND IS HOLDING A FIREARM, AND ALLOWOFFHANDSTABILIZATION IS ON, SCALE RECOIL. 
            if (PrimaryGrip && allowOffHandStabilization && Vector3.Distance(hands[0].transform.position, hands[1].transform.position) < offhandStabilizationDistance)
            {
                recoilManager.ScaleRotation(0.1f, 0.1f, 1, 0.1f, 1);
                recoilManager.ScaleTranslation(0.5f, 0.5f, 1, 0.5f, 1);
            }
            else if (allowOffHandStabilization)
            {
                recoilManager.UnscaleRotation();
                recoilManager.UnscaleTranslation();
            }
        }

        bool releasedTriggerAfterPickUp = true;

        protected override void PrimaryGrasp()
        {
            if (trigger) trigger.ReleasedTrigger = false;

            if (primaryGrip.StartInputID == PrimaryHand.TriggerInput && VArmoryInput.Input(primaryGrip.StartInputID, PrimaryHand)) 
                releasedTriggerAfterPickUp = false;

            if (recoilManager.recoilRotationOffset)
            {
                GameObject destroyThis = recoilManager.recoilRotationOffset.gameObject;
                Destroy(destroyThis);
            }

            recoilManager.recoilRotationOffset = new GameObject(string.Format("Recoil Rotation Offset", name)).transform;
            recoilManager.recoilRotationOffset.SetParent(PrimaryHand.Offset, true);
            recoilManager.recoilRotationOffset.localScale = Vector3.one;
            recoilManager.recoilRotationOffset.localPosition = Vector3.zero;
            recoilManager.recoilRotationOffset.localRotation = Quaternion.identity;

            recoilManager.recoilPositionOffset = new GameObject(string.Format("Recoil Position Offset", name)).transform;
            recoilManager.recoilPositionOffset.SetParent(recoilManager.recoilRotationOffset, true);
            recoilManager.recoilPositionOffset.localScale = Vector3.one;
            recoilManager.recoilPositionOffset.localPosition = Vector3.zero;
            recoilManager.recoilPositionOffset.localRotation = Quaternion.identity;

            base.PrimaryGrasp();

            transform.SetParent(recoilManager.recoilPositionOffset, true);

            PrimaryHand.audioSourceContainer.transform.parent = muzzle;
            PrimaryHand.audioSourceContainer.transform.localPosition = Vector3.zero;
        }

        public override void StartVirutalStock()
        {
            base.StartVirutalStock();
            transform.SetParent(recoilManager.recoilPositionOffset, true);
        }

        protected override void PrimaryDrop()
        {

            PrimaryHand.audioSourceContainer.transform.parent = PrimaryHand.transform;
            PrimaryHand.audioSourceContainer.transform.localPosition = Vector3.zero;

            base.PrimaryDrop();

            releasedTriggerAfterPickUp = true;

            if (recoilManager.recoilPositionOffset || recoilManager.recoilRotationOffset)
            {
                GameObject destroyThis = recoilManager.recoilRotationOffset.gameObject;
                recoilManager.recoilPositionOffset = null;
                recoilManager.recoilRotationOffset = null;
                Destroy(destroyThis);
            }

            recoilManager.ClearRecoil();
        }

        public override void StopVirtualStock()
        {
            base.StopVirtualStock();
            transform.SetParent(recoilManager.recoilPositionOffset, true);
        }

        protected override void SecondaryGrasp()
        {
            base.SecondaryGrasp();

            SecondaryHand.audioSourceContainer.transform.parent = muzzle;
            SecondaryHand.audioSourceContainer.transform.localPosition = Vector3.zero;
        }

        protected override void SecondaryDrop()
        {
            SecondaryHand.audioSourceContainer.transform.parent = SecondaryHand.transform;
            SecondaryHand.audioSourceContainer.transform.localPosition = Vector3.zero;

            base.SecondaryDrop();
        }

        protected int burstCount;

        protected virtual void PullTrigger()
        {
            if (!releasedTriggerAfterPickUp)
                return;

            switch (fireSelector.FireMode)
            {
                case FireSelector._FireMode.safety:
                    DryFireSounds();
                    break;
                case FireSelector._FireMode.semi:
                    if (!FirePreconditions()) DryFireSounds();
                    Fire();
                    break;
                case FireSelector._FireMode.burst:
                    if (!FirePreconditions()) DryFireSounds();
                    break;
                case FireSelector._FireMode.full:
                    if (!FirePreconditions()) DryFireSounds();
                    break;
            }
        }

        void DryFireSounds() { if (firearmAudioManager) firearmAudioManager.PlayRandomAudioClip(firearmAudioManager.dryFireSounds, chambers[0].transform.position); }

        protected virtual void HoldTrigger()
        {
            if (!releasedTriggerAfterPickUp)
                return;

            switch (fireSelector.FireMode)
            {
                case FireSelector._FireMode.semi:
                    if (stackTriggerPulls && pulledTriggerWhileSlideWasBack)
                    {
                        pulledTriggerWhileSlideWasBack = false;
                        Fire();
                    }
                    break;
                case FireSelector._FireMode.burst:
                    if (burstCount < 3)
                        Fire();
                    break;
                case FireSelector._FireMode.full:
                    Fire();
                    break;
            }
        }

        protected virtual void ReleaseTrigger()
        {
            burstCount = 0;
            pulledTriggerWhileSlideWasBack = false;
        }

        protected virtual void EjectCartridge() { EjectCartridge(selectedChamberIndex); }

        protected virtual void EjectCartridge(int i)
        {
            Debug.Log("Attempt eject cartridge");

            Chamber tempChamber = chambers[i];

            if (!tempChamber)
                return;

            if (slide)
                if (slide.InteractionPoint)
                    if (slide.AverageVelocity() >= slideEjectVelocity)
                        return;

            Bullet tempBullet = tempChamber.Bullet;

            if (!tempBullet)
                return;

            Debug.Log("Successfully ejected cartridge");

            tempChamber.EjectBullet(velocityHistory._ReleaseVelocity);
        }

        protected virtual void SlideStop()
        {
            if (!slideStopOnEmptyMag)
                return;

            bool emptyMag = magazine ? magazine.Empty : false;

            slide.SlideStop = emptyMag;

            if (emptyMag)
                enableSlideAudio = true;
        }

        protected virtual void RestOnSlideStop() { if (firearmAudioManager) firearmAudioManager.PlayRandomAudioClip(firearmAudioManager.restOnSlideStopSounds, slide.transform.position); }

        protected bool caughtRoundFromMagazine;

        protected virtual void CatchBullet()
        { 
            caughtRoundFromMagazine = magazine ? !magazine.Empty : false;

            Debug.Log("Catch bullet from magazine : " + caughtRoundFromMagazine);
        }

        protected virtual void ChamberRoundFromMagazine()
        {
            Debug.Log("Attempt to chamber round from magazine");

            //if (!caughtRoundFromMagazine) return;

            //caughtRoundFromMagazine = false;

            //Debug.Log("Has caught round");

            if (!magazine) return;

            Debug.Log("Has magazine");

            if (magazine.Empty) return;

            Debug.Log("Has non empty magazine");

            if (chambers[0].Bullet) return;

            Debug.Log("Succesfully chambered bullet");

            Bullet tempBullet = magazine.SavedBullets[magazine.SavedBullets.Count - 1];

            magazine.SavedBullets.Remove(tempBullet);
            magazine.CurrentRounds -= 1;

            chambers[0].ChamberBullet(tempBullet);

            IgnoreCollision(chambers[0].Bullet.Projectile.Col, true);
        }

        protected virtual void ChamberBullet(Chamber chamber) { chamber.LoadPotentialBullet(); }

        protected virtual void LoadMag(Item item)
        {
            Magazine tempMag = item as Magazine;

            if (tempMag)
                LoadMag(tempMag);
        }

        protected virtual void LoadMag(Magazine magazine)
        {
            if (!magazine) return;

            this.magazine = magazine;
            if (firearmAudioManager) firearmAudioManager.PlayRandomAudioClip(firearmAudioManager.loadMagazineSounds, magazine.transform.position);
        }

        protected virtual void RemoveMag()
        {
            magazine = null;
            caughtRoundFromMagazine = false;
        } 

        protected virtual void EjectMagVelocity()
        {
            StartCoroutine(SetEjectMagVelocity());

            if (magazine)
            {
                //StartCoroutine(DelayIgnoreCollision(magazine.Col, false, 0.25f));

                if (magazine.TapedMagazine)
                    magazine.TapedMagazine.Restrained = false;
            }
        }

        IEnumerator SetEjectMagVelocity()
        {
            yield return new WaitForFixedUpdate();

            if (ejectMag)
            {
                ejectMag.Rb.velocity = velocityHistory._ReleaseVelocity;

                ejectMag = null;
            }
        }

        protected virtual void RestrainTapedMag(Item mag)
        {
            mag.gameObject.layer = gameObject.layer;

            Magazine tapedMag = mag as Magazine;

            if (tapedMag == null) return;

            //IgnoreCollision(tapedMag.Col, true);

            tapedMag = tapedMag.TapedMagazine;

            if (tapedMag == null) return;

            tapedMag.Detach();
            tapedMag.DetachSlot();
            tapedMag.transform.SetParent(mag.transform, true);
            tapedMag.SetPhysics(true, false, true);

            tapedMag.Restrained = true;
        }

        IEnumerator DelayIgnorePhysics(Collider colliderOne, Collider colliderTwo, bool ignore, float delay)
        {
            yield return new WaitForSeconds(delay);
            Physics.IgnoreCollision(colliderOne, colliderTwo, ignore);
        }

        IEnumerator DelayIgnoreCollision(Collider col, bool ignore, float delay)
        {
            yield return new WaitForSeconds(delay);
            IgnoreCollision(col, ignore);
        }

        protected virtual void DetachMagSFX() { if (firearmAudioManager) firearmAudioManager.PlayRandomAudioClip(firearmAudioManager.unloadMagazineSounds, magWell.transform.position); }

        protected bool pulledTriggerWhileSlideWasBack;

        protected virtual bool FirePreconditions() { return FirePreconditions(selectedChamberIndex); }

        protected virtual bool FirePreconditions(int i)
        {
            if (slide)
                if (slide.slidePosition < 1)
                    pulledTriggerWhileSlideWasBack = true;

            if (chambers[i].Bullet)
                if (!chambers[i].Bullet.Spent)
                    if (slide)
                    {
                        if (slide.slidePosition >= 1)
                            return true;
                    }
                    else
                        return true;

            return false;
        }

        protected virtual void Fire() { Fire(selectedChamberIndex); }

        protected virtual bool Fire(int i)
        {
            if (!FirePreconditions(i)) return false;

            if (PrimaryHand || SecondaryHand)
                recoilManager.IncreaseAllRecoil();

            if (muzzleVelocity == 0)
                chambers[i].Bullet.Fire(muzzle);
            else
                chambers[i].Bullet.Fire(muzzle, muzzleVelocity, spread);

            MuzzleFlash();

            if (fireSelector.FireMode == FireSelector._FireMode.burst)
                burstCount++;

            enableSlideAudio = false;

            if (firearmAudioManager) firearmAudioManager.FireFX(suppressed);

            if (slide)
                slide.AnimateSlide();

            return true;
        }

        protected virtual void MuzzleFlash()
        {
            if (muzzleFlash)
            {
                GameObject clone = Instantiate(muzzleFlash, muzzle.position, Quaternion.LookRotation(muzzle.forward), muzzle);
                Transform smoke = clone.transform.Find("Smoke");

                if (smoke)
                {
                    smoke.SetParent(null, true);
                    Destroy(smoke.gameObject, 1f);
                }

                Destroy(clone.gameObject, 0.5f);
            }
        }

        protected virtual void AdvanceChamberIndex()
        {
            if (selectedChamberIndex == chambers.Count - 1)
                selectedChamberIndex = 0;
            else
                selectedChamberIndex++;
        }

        protected bool suppressed;

        protected override void AddAttachment(Attachment attachment)
        {
            base.AddAttachment(attachment);

            IgnoreCollision(attachment.Col, true);

            if (attachment.type == Attachment.Type.foregrip)
                Foregrip = true;

            if (attachment.type == Attachment.Type.suppressor)
                Suppressed = true;
        }

        protected override void RemoveAttachment(Attachment attachment)
        {
            base.RemoveAttachment(attachment);

            IgnoreCollision(attachment.Col, false);

            if (attachment.type == Attachment.Type.foregrip)
                Foregrip = false;

            if (attachment.type == Attachment.Type.suppressor)
                Suppressed = false;
        }

        protected bool Suppressed
        {
            set
            {
                Vector3 tempMuzzlePos = muzzle.localPosition;

                if (value != suppressed)
                    tempMuzzlePos.z += value ? 0.1513081f / transform.lossyScale.z : -0.1513081f / transform.lossyScale.z;

                muzzle.localPosition = tempMuzzlePos;
                suppressed = value;
            }
        }

        protected bool Foregrip
        {
            set
            {
                if (value)
                {
                    recoilManager.ScaleRotation(0.5f, 0.5f, 1, 0.5f, 1);
                    recoilManager.ScaleTranslation(0.5f, 0.5f, 1, 0.5f, 1);
                }
                else
                {
                    recoilManager.UnscaleRotation();
                    recoilManager.UnscaleTranslation();
                }
            }
        }
    }
}