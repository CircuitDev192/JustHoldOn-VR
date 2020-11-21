using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Slide : MonoBehaviour
    {
        public bool Restrain
        {
            set
            {
                if (interactionVolume)
                    interactionVolume.restrained = value;
            }
        }

        [SerializeField] protected InteractionVolume interactionVolume;

        public delegate void ReachedEnd();
        public delegate void ReachedStart();
        public delegate void FullCycle();

        public ReachedEnd _OnReachedEnd;
        public ReachedStart _OnReachedStart;
        public FullCycle _OnFullCycle;

        public Transform startPosition;
        public Transform endPosition;

        public Vector3 StartPosition { get { return startPosition ? startPosition.position: Vector3.zero; } }
        public Vector3 EndPosition { get { return endPosition ? endPosition.position : Vector3.zero; } }
        
        //[ReadOnly]
        public Transform slideObject;

        protected float sliderOffset;
        protected float handOffset;

        [ReadOnly] public float slidePosition;

        protected float sliderLength;
        [HideInInspector] public float maxSliderDistance = 1;
        [HideInInspector] public float minSliderDistance = 0;
        [ReadOnly] [SerializeField] protected bool onReachedEndHysteresis;
        [SerializeField] protected float endHysteresis = 0.1f;

        [ReadOnly] [SerializeField] protected bool onReachedStartHysteresis;
        [SerializeField] protected float startHysteresis = 0.1f;

        [SerializeField] protected bool continuous;

        [SerializeField] [ReadOnly] protected Transform interactionPoint;
        public Transform InteractionPoint { get { return interactionPoint; } }

        protected virtual void Start()
        {
            if (!startPosition || !endPosition) return;

            sliderLength = Vector3.Distance(startPosition.position, endPosition.position);

            if (slideObject)
            {
                Vector3 diff = (endPosition.position - startPosition.position);
                diff.Normalize();

                sliderOffset = Vector3.Dot(diff, (endPosition.position - slideObject.position) / sliderLength);
                handOffset = Vector3.Dot(diff, (endPosition.position - Vector3.one) / sliderLength);

                slidePosition = Mathf.Clamp(Vector3.Dot(diff, (endPosition.position - Vector3.one) / sliderLength) + sliderOffset - handOffset, minSliderDistance, maxSliderDistance);
            }

            StartCoroutine(MoveSlide());

            velocitySampleSize = 3;

            if (interactionVolume)
            {
                interactionVolume._StartInteraction += GrabSlide;
                interactionVolume._EndInteraction += DetachSlide;
            }

            if (!slideObject) slideObject = transform;
        }

        public void ForceStop() { if (interactionVolume) interactionVolume.StopInteraction(); }
        public void ForceStart(Hand hand) { if (interactionVolume) interactionVolume.ForceStartInteraction(hand); }

        public void OnEnable() { StartCoroutine(MoveSlide()); }

        public virtual void GrabSlide()
        {
            if (interactionVolume.Hand)
                GrabSlide(interactionVolume.Hand.transform);
        }

        public virtual void GrabSlide(Transform newInteractPoint)
        {
            if (!slideObject)
                return;

            interactionPoint = newInteractPoint;

            Vector3 diff = (endPosition.position - startPosition.position);
            diff.Normalize();

            sliderOffset = Vector3.Dot(diff, (endPosition.position - slideObject.position) / sliderLength);
            handOffset = Vector3.Dot(diff, (endPosition.position - interactionPoint.transform.position) / sliderLength);
        }

        public virtual void DetachSlide() { interactionPoint = null; }

        protected float prevNormal;

        protected virtual void SlideEvents()
        {
            CallSlideEvents();
            ResetHysteresis();
        }

        protected void CallSlideEvents()
        {
            if (prevNormal != slidePosition)
            {
                Debug.Log("Call slide events");
                prevNormal = slidePosition;

                if (slidePosition == 1)
                    OnReachedStart();
                if (slidePosition == 0)
                    OnReachedEnd();
            }
        }

        protected virtual void ResetHysteresis()
        {
            if (slideObject)
                if (slidePosition >= 0)
                {
                    if (slidePosition <= 1 - startHysteresis && !onReachedStartHysteresis)
                    {
                        Debug.Log("Reset start hysteresis");
                        onReachedStartHysteresis = true;
                    }

                    if (slidePosition >= 0 + endHysteresis && !onReachedEndHysteresis)
                    {
                        Debug.Log("Reset end hysteresis");
                        onReachedEndHysteresis = true;
                    }
                }
        }

        protected virtual void Update()
        {
            if (interactionPoint)
                SlideEvents();

            VelocityStep();
        }

        protected int velocitySampleSize = 3;
        protected float[] velocityHistory;
        protected int velocityHistoryStep = 1;

        protected float previousPosition;

        public void VelocityStep()
        {
            if (velocityHistory == null)
                velocityHistory = new float[velocitySampleSize];

            velocityHistoryStep++;

            if (velocityHistoryStep >= velocityHistory.Length)
                velocityHistoryStep = 0;

            velocityHistory[velocityHistoryStep] = (slidePosition - previousPosition) / Time.deltaTime;
            previousPosition = slidePosition;
        }

        public float AverageVelocity()
        {
            float total = 0;

            foreach (float velHist in velocityHistory)
                total += velHist;

            return total / velocityHistory.Length;
        }

        protected virtual IEnumerator MoveSlide()
        {
            while (true)
            {
                if (!slideObject || !interactionPoint)
                {
                    yield return null;
                    continue;
                }

                Vector3 diff = (endPosition.position - startPosition.position);
                diff.Normalize();
                slidePosition = Mathf.Clamp(Vector3.Dot(diff, (endPosition.position - interactionPoint.transform.position) / sliderLength) + sliderOffset - handOffset, minSliderDistance, maxSliderDistance);
                Vector3 desiredPosition = Vector3.Lerp(endPosition.position, startPosition.position, slidePosition);
                slideObject.position = desiredPosition;
                PoseSlide();

                if (continuous)
                    if ((previousPosition == 0 && slidePosition == 0) || (previousPosition == 1 && slidePosition == 1))
                        GrabSlide(interactionPoint);

                //VelocityStep();
                yield return null;
            }
        }

        protected virtual void PoseSlide() { }

        public void SetSlidePositionToTransform(Transform setTo)
        {
            Vector3 diff = (endPosition.position - startPosition.position);
            diff.Normalize();
            SetSlidePosition(Vector3.Dot(diff, (endPosition.position - setTo.position) / sliderLength));
        }

        public void SetSlidePosition(float newNormal)
        {
            newNormal = Mathf.Clamp(newNormal, minSliderDistance, maxSliderDistance);
            slidePosition = newNormal;
            Vector3 desiredPosition = Vector3.Lerp(endPosition.position, startPosition.position, slidePosition);

            if (slideObject) slideObject.position = desiredPosition;
        }

        protected virtual void OnFullCycle() { if (_OnFullCycle != null) _OnFullCycle(); }

        protected virtual void OnReachedEnd()
        {
            Debug.Log("Attempt on reach end");

            if (!onReachedEndHysteresis)
                return;

            onReachedEndHysteresis = false;

            Debug.Log("End hysteresis successfull");

            if (_OnReachedEnd != null)
                _OnReachedEnd();
        }

        protected virtual void OnReachedStart()
        {
            Debug.Log("Attempt on reach start");

            if (!onReachedStartHysteresis)
                return;

            onReachedStartHysteresis = false;

            Debug.Log("Start hysteresis successfull");

            if (_OnReachedStart != null)
                _OnReachedStart();
        }

        protected IEnumerator slideBack;
        protected IEnumerator slideForward;

        public IEnumerator AnimateSlide(float[] speed)
        {
            if (speed[0] <= 0 || speed[1] <= 0)
                yield break;

            slideBack = AnimateSlideBack(speed[0]);
            slideForward = AnimateSlideForward(speed[1]);

            yield return slideBack;
            yield return slideForward;

            yield break;
        } 
        
        public IEnumerator AnimateSlide(float speed)
        {
            if (speed <= 0)
                yield break;

            slideBack = AnimateSlideBack(speed);
            slideForward = AnimateSlideForward(speed);

            yield return slideBack;
            yield return slideForward;

            yield break;
        }

        public IEnumerator AnimateSlideBack(float speed)
        {
            if (speed <= 0)
                yield break;

            float startTime = Time.time - Time.deltaTime;
            float initialDistanceNormal = slidePosition;

            while (slidePosition != 0)
            {
                SetSlidePosition(initialDistanceNormal - ((Time.time - startTime) * speed));
                SlideEvents();
                yield return null;           
            }
        }

        public IEnumerator AnimateSlideForward(float speed)
        {
            if (speed <= 0)
                yield break;

            float startTime = Time.time - Time.deltaTime;
            float initialDistanceNormal = slidePosition;

            while (slidePosition != 1)
            {
                SetSlidePosition(initialDistanceNormal + ((Time.time - startTime) * speed));
                SlideEvents();
                yield return null;
            }
        }

        protected virtual void ResetSlider()
        {

        }
    }
}