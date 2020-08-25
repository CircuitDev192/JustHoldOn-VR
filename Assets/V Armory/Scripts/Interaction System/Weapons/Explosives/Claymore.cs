using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Claymore : ExplosiveItem
    {

        [SerializeField] protected int pellets;
        [SerializeField] protected float projectileDistance = 5;
        [SerializeField] protected float horizontalSpread = 0.5f;
        [SerializeField] protected float verticalSpread = 0.5f;
        [SerializeField] protected float force = 5;

        [SerializeField] protected TwitchExtension.DotAxis tripwireDirection;
        [SerializeField] protected float tripWireTickRate = 0.33f;
        [SerializeField] protected float tripWireDistance = 3f;

        [SerializeField] protected Transform explosion;
        [SerializeField] protected InteractionVolume armingVolume;

        protected override void Start()
        {
            base.Start();
            laser = GetComponentInChildren<LineRenderer>();
            armingVolume._StartInteraction += Arm;
            if (explosive)
                explosive._Explode += Explode;
        }

        public override void Arm()
        {
            if (!armed) StartCoroutine(TripWire());
            else
            {
                armed = false;
                laser.SetPosition(1, Vector3.zero);
                StopAllCoroutines();
            }
            armingVolume.StopInteraction();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (PrimaryHand)
            {

                if (collision.relativeVelocity.magnitude > 1 || velocityHistory._ReleaseVelocity.magnitude > 1)
                {

                    if (transform.InverseTransformPoint(collision.contacts[0].point).y < 0)
                    {

                        if (Vector3.Dot(collision.relativeVelocity, transform.up) > 0.75f || Vector3.Dot(-velocityHistory._ReleaseVelocity, transform.up) > 0.75f)
                        {

                            Detach();
                            rb.isKinematic = true;
                            transform.SetParent(collision.transform);
                        }
                    }
                }
            }
        }

        IEnumerator TripWire()
        {
            armed = true;

            yield return new WaitForSeconds(armDelay);

            WaitForSeconds wait = new WaitForSeconds(tripWireTickRate);

            RaycastHit hit;

            if (Physics.Raycast(laser.transform.position, TwitchExtension.ReturnAxis(tripwireDirection, transform), out hit, tripWireDistance))
                laser.SetPosition(1, new Vector3(0, 0, hit.distance));
            else
                laser.SetPosition(1, new Vector3(0, 0, tripWireDistance));

            float hitDistance = hit.transform != null ? hit.distance : tripWireDistance;

            while (true)
            {
                if (Physics.Raycast(laser.transform.position, TwitchExtension.ReturnAxis(tripwireDirection, transform), out hit, tripWireDistance))
                {
                    if (hit.distance != hitDistance)
                    {
                        yield return ExplodeRoutine();
                        yield break;
                    }
                }

                yield return wait;
            }
        }

        protected override IEnumerator ExplodeRoutine()
        {
            yield return new WaitForSeconds(delay);

            Detach();
            DetachSlot();

            yield return new WaitForEndOfFrame();

            RaycastHit hit;

            for (int i = 0; i < pellets; i++)
            {
                Vector3 spread = new Vector3(Random.Range(-verticalSpread, verticalSpread), Random.Range(-horizontalSpread, horizontalSpread), Random.Range(-verticalSpread, verticalSpread));
                spread = transform.TransformDirection(spread);

                Debug.DrawRay(transform.position, (TwitchExtension.ReturnAxis(tripwireDirection, transform) + spread) * projectileDistance, Color.green, 15f);

                if (Physics.Raycast(transform.position, (TwitchExtension.ReturnAxis(tripwireDirection, transform) + spread) * projectileDistance, out hit, 5f))
                {
                    if (hit.rigidbody)
                    {
                        hit.rigidbody.AddForceAtPosition(force * TwitchExtension.ReturnAxis(tripwireDirection, transform), hit.point);

                        /*Health health = hit.transform.GetComponent<Health>();

                        if (health)
                        {
                            health.ApplyDamage(200);
                        }*/
                    }

                    if (hit.transform.tag == "Explosive")// || hit.transform.gameObject.HasTag("Explosive"))
                    {
                        Explosive tempExplosive = hit.transform.GetComponent<Explosive>();

                        if (tempExplosive && tempExplosive.DetonatedByOtherExplosives)
                            tempExplosive.ExplodeByOther(0.1f);
                    }
                }
            }

            if (explosion) Instantiate(explosion, transform.position, transform.rotation);

            Destroy(gameObject, 0.1f);
        }

        protected LineRenderer laser;
    }
}
