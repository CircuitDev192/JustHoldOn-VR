using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class RotatingBolt : Hinge
    {
        public delegate void HingeEvent();
        public HingeEvent _RotateUp;
        public HingeEvent _RotateDown;


        public HingeEvent _GrabSlide;
        public HingeEvent _GrabHinge;
        public HingeEvent _DetachSlide;
        public HingeEvent _DetachHinge;

        public Slide slider;

        public float slideTolerance;
        public float hingeTolerance;

        float previousRotation;
        float previousSliderNormal;

        protected enum ActionType
        {
            upAndBack,
            backAndUp
        }

        [SerializeField] protected ActionType actionType;

        protected override void Start()
        {
            base.Start();

            _GrabSlide += OpenSFX;
            _DetachSlide += CloseSFX;
        }

        protected override void Update()
        {
            Hand tempHand = interactionVolume.Hand;

            if (!tempHand)
                return;

            if (actionType == ActionType.upAndBack)
                UpAndBack(tempHand);
            else
                BackAndUp(tempHand);

            previousRotation = transform.localEulerAngles.z;
            previousSliderNormal = slider.slidePosition;
        }

        void UpAndBack(Hand hand)
        {
            //When the slide is moved close enough to start by atleast a distance of slideTolerance start interaction with the hinge 
            if (previousSliderNormal < 1 - slideTolerance && slider.slidePosition >= 1 - slideTolerance)
            {
                HingeHand = hand;

                if (_GrabHinge != null)
                    _GrabHinge();
            }

            //When the slide is moved away from start by atleast a distance of slideTolerance stop interaction with the hinge
            if (previousSliderNormal >= 1 - slideTolerance && slider.slidePosition < 1 - slideTolerance)
            {
                HingeHand = null;

                if (_DetachHinge != null)
                    _DetachHinge();
            }

            PullHinge();

            //When the hinge is rotated with in hingeTolerance degrees of maxAngle start interaction with the slide
            if (previousRotation <= maxAngle - hingeTolerance && transform.localEulerAngles.z >= maxAngle - hingeTolerance)
            { 
                slider.GrabSlide(hand.transform);

                if (_GrabSlide != null)
                    _GrabSlide();
            }

            //When the hinge is rotated away from its maxAngle by hingeTolerance degrees stop interaction with the slide
            if (previousRotation >= maxAngle - hingeTolerance && transform.localEulerAngles.z < maxAngle - hingeTolerance)
            {
                slider.DetachSlide();

                if (_DetachSlide != null)
                    _DetachSlide();
            }
            //Note: There is a state where both the slide and hinge are interacted with by the same hand simultaneously. This is intended
        }

        void BackAndUp(Hand hand)
        {
            //When the slide is moved close enough to end by atleast a distance of slideTolerance start interaction with the hinge 
            if (previousSliderNormal > slideTolerance && slider.slidePosition <= slideTolerance)
                HingeHand = hand;

            //When the slide is moved away from end by atleast a distance of slideTolerance stop interaction with the hinge
            if (previousSliderNormal < slideTolerance && slider.slidePosition >= slideTolerance)
                HingeHand = null;

            PullHinge();

            //When the hinge is rotated with in hingeTolerance degrees of its lowest possible rotation call the _RotateDown event
            if (previousRotation > hingeTolerance && transform.localEulerAngles.z <= hingeTolerance)
                if (_RotateDown != null)
                    _RotateDown();

            //When the hinge is rotated away from its lowest possible rotation by hingeTolerance degrees call the _RotateUp event
            if (previousRotation < hingeTolerance && transform.localEulerAngles.z >= hingeTolerance)
                if (_RotateUp != null)
                    _RotateUp();

            //_RotateUp & _RotateDown are used by the RotatingBoltSMG class to enable and disable the slideStop
        }

        protected override void GrabHinge()
        {
            Hand hand = interactionVolume.Hand;

            if (actionType == ActionType.upAndBack)
            {
                //If the slide position is close to start grab the hinge
                if (slider.slidePosition >= 1 - slideTolerance)
                    HingeHand = hand;

                //If the hinge rotation is close to maxAngle grab the slide
                if (transform.localEulerAngles.z >= maxAngle - hingeTolerance)
                    slider.GrabSlide(hand.transform);
            }
            else
            {
                //If the slide position is close to end grab the hinge
                if (slider.slidePosition <= slideTolerance)
                    HingeHand = hand;

                slider.GrabSlide(hand.transform);
            }
        }

        protected override void RemoveHingeHand()
        {
            HingeHand = null;
            slider.DetachSlide();
        }

    }
}