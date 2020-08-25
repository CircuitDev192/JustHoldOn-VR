using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class PumpAction : Shotgun
    {
        [SerializeField] protected VArmoryInput.TouchPadDirection unlockPumpDirection;

        protected bool PumpLocked { get { return slide.minSliderDistance == 1; } }

        [SerializeField] protected SteamVR_Action_Boolean unlockPumpInput;

        protected override void Start()
        {
            base.Start();

            if (slide)
                slide._PulledPassedSlideStop -= SlideStop;
        }

        void LockPump() { slide.minSliderDistance = 1; }

        void UnlockPump()
        {
            slide.minSliderDistance = 0;

            if (SecondaryHand)
                if (!slide.InteractionPoint)
                    slide.GrabSlide(SecondaryHand.transform);
        }

        protected override void Update()
        {
            if (SecondaryHand)
            {
                if (SecondaryHand.TriggerInput != null)
                {
                    //if (SecondaryHand.TriggerInput.GetStateUp(SecondaryHand.inputSource))
                    //   slide.DetachSlide();

                    if (VArmoryInput.Input(SecondaryHand.TriggerInput, SecondaryHand) && !PumpLocked)
                        if (!slide.InteractionPoint)
                            slide.GrabSlide(SecondaryHand.transform);
                }
            }

            LocalInputDown(UnlockPump, unlockPumpInput);

            base.Update();
        }

        protected override void SecondaryGrasp()
        {
            base.SecondaryGrasp();

            if(!PumpLocked)
                if (!slide.InteractionPoint)
                {
                    slide.GrabSlide(SecondaryHand.transform);
                }
        }

        protected override void SecondaryDrop()
        {
            base.SecondaryDrop();
            slide.DetachSlide();
        }

        protected override void ChamberRoundFromMagazine()
        {
            LockPump();
            slide.DetachSlide();

            base.ChamberRoundFromMagazine();
        }

        protected override bool FirePreconditions()
        {
            UnlockPump();
            return base.FirePreconditions();
        }

        protected override void TouchpadDownInput()
        {
            base.TouchpadDownInput();
            TouchPadInput(UnlockPump, unlockPumpDirection);
        }

        protected override void TouchpadUpInput() { }

        protected override bool Fire(int i)
        {
            bool tempFire = base.Fire(i);
            enableSlideAudio = true;
            return tempFire;
        }
    }
}