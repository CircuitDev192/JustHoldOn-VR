using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class FirearmSlide : Slide
    {
        [SerializeField] protected bool animateForwardOnRelease;

        [SerializeField] protected float slideForwardSpeed;
        [SerializeField] protected float slideBackSpeed;

        [SerializeField] protected float dropSlideForwardSpeed;

        [SerializeField] protected SocketSlide magWell;

        protected bool slideStop;

        public bool SlideStop
        {
            set
            {
                if (value && slidePosition > slideStopPosition)
                    return;

                if (slideStopObject)
                	slideStopObject.localEulerAngles = slideStopRotations[value ? 0 : 1];

                maxSliderDistance = value ? slideStopPosition : 1;

                slideStop = value;
                if (!slideStop)
                    if (restingOnSlideStop && !interactionPoint)
                        if (dropSlideForwardSpeed > 0 && slideForward == null) //in case the slide stop ever causes issues, check the stop slide anim finc
                        {
                            StartCoroutine("AnimateSlideForward", dropSlideForwardSpeed);
                        }
            }

            get
            {
                return slideStop;
            }
        }

        [SerializeField] [Range(-1, 1)] protected float slideStopPosition;
        [SerializeField] [Range(0, 1)] protected float catchBulletPosition;

        public float SlideStopPosition
        {
            get
            {
                return slideStopPosition;
            }
        }

        public delegate void SlideStopEvent();
        public SlideStopEvent _PulledPassedSlideStop;
        public SlideStopEvent _CatchBullet;
        public SlideStopEvent _RestingOnSlideStop;

        [ReadOnly] [SerializeField] protected bool restingOnSlideStop;
        public bool RestingOnSlideStop { get { return restingOnSlideStop; } }

        [SerializeField] protected Transform slideStopObject;
        [SerializeField] protected Vector3[] slideStopRotations = new Vector3[2];

        [SerializeField] protected SteamVR_Action_Boolean slideStopInput;
        public SteamVR_Action_Boolean SlideStopInput { get { return slideStopInput; } }

        [SerializeField] protected VArmoryInput.TouchPadDirection slideStopTouchpadDirection;
        public VArmoryInput.TouchPadDirection SlideStopTouchpadDirection { get { return slideStopTouchpadDirection; } }

        //start = 1 = resting; end = 0 = pulled;

        [SerializeField] protected SlideStopFunction slideStopFunction;
        protected enum SlideStopFunction
        {
            none,
            off,
            on,
            toggle
        }

        public override void GrabSlide()
        {
            StopSlideAnimations();
            base.GrabSlide();
        }

        float firearmPrevNormal;

        protected virtual void SlideStopEvents()
        {
            if (firearmPrevNormal <= catchBulletPosition && slidePosition > catchBulletPosition)
                if (_CatchBullet != null)
                    _CatchBullet();

            if (slideStopPosition > 0)
            {
                if (firearmPrevNormal >= slideStopPosition && slidePosition < slideStopPosition)
                    if (_PulledPassedSlideStop != null)
                        _PulledPassedSlideStop();

                if (firearmPrevNormal < slideStopPosition && slidePosition >= slideStopPosition && slideStop && !interactionPoint) //!interactionVolume.Hand
                {
                    restingOnSlideStop = true;
                    StopSlideAnimations();
                    SetSlidePosition(slideStopPosition);

                    if (_RestingOnSlideStop != null)
                        _RestingOnSlideStop();
                }
            }

            firearmPrevNormal = slidePosition;
        }

        protected override void SlideEvents()
        {
            SlideStopEvents();
            base.SlideEvents();
        }

        public void LockSlideStop() { SlideStop = true; }

        public void UnlockSlideStop() { SlideStop = false; }

        public void ToggleSlideStop() { SlideStop = !slideStop; }

        public void SlideStopTouchpadInput()
        {
            if (magWell.slideObject != null)
                if(magWell.slideObject.GetComponent<Magazine>().Empty) return;

            switch (slideStopFunction)
            {
                case SlideStopFunction.none:
                    break;
                case SlideStopFunction.off:
                    UnlockSlideStop();
                    break;
                case SlideStopFunction.on:
                    LockSlideStop();
                    break;
                case SlideStopFunction.toggle:
                    ToggleSlideStop();
                    break;
                default:
                    break;
            }
        }

        public virtual void AnimateSlide()
        {
            if (slideForwardSpeed == 0 || slideBackSpeed == 0)
                return;

            if (interactionVolume)
                if (interactionVolume.Hand)
                    return;

            StopSlideAnimations();
            StartCoroutine(AnimateSlide(slideBackSpeed));
        }

        public void StopSlideAnimations()
        {
            StopCoroutine("AnimateSlide");
            StopCoroutine("AnimateSlideBack");
            StopCoroutine("AnimateSlideForward");
            if (slideBack != null) StopCoroutine(slideBack);
            if (slideForward != null) StopCoroutine(slideForward);
            slideForward = null; //this MIGHT cause issues later on. 
        }

        public override void DetachSlide()
        {
            base.DetachSlide();

            StopSlideAnimations();

            if (!(slidePosition >= slideStopPosition && slideStop) && animateForwardOnRelease)
            {
                StartCoroutine("AnimateSlideForward", dropSlideForwardSpeed);
            }
        }
    }
}