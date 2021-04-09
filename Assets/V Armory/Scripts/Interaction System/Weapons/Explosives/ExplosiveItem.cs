using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class ExplosiveItem : Item
    {
        [SerializeField] protected Explosive explosive;
        [SerializeField] protected bool armed;
        public bool Armed { get { return armed; } }
        [SerializeField] protected float armDelay;
        [SerializeField] protected float delay;

        [SerializeField] protected AudioClip explosionSFX;

        protected override void Start()
        {
            base.Start();

            if (explosive)
            {
                explosive._Explode += Detach;
                explosive._Explode += DetachSlot;
            }
        }

        public virtual void Arm()
        {
            armed = !armed;
        }

        public virtual void Disarm()
        {
            //Primarily for the grenades
            // to allow pin to be put back in.
            armed = false;
        }

        public virtual void DelayedArm()
        {
            StartCoroutine(DelayedArmRoutine(armDelay));
        }

        public virtual void Explode()
        {
            StartCoroutine(ExplodeRoutine());
        }

        protected virtual IEnumerator DelayedArmRoutine(float seconds)
        {
            if (armed)
            {
                armed = false;
                yield break;
            }

            yield return new WaitForSeconds(seconds);

            armed = true;
        }

        protected virtual IEnumerator ExplodeRoutine()
        {
            if (!armed) yield break;

            armed = false;

            yield return new WaitForSeconds(delay);

            Detach();
            DetachSlot();

            if (explosionSFX) AudioSource.PlayClipAtPoint(explosionSFX, transform.position);

            EventManager.TriggerSoundGenerated(this.transform.position, 75f);

            yield return new WaitForEndOfFrame();

            if (explosive)
            {
                explosive.Explode(0);
            }
        }
    }
}