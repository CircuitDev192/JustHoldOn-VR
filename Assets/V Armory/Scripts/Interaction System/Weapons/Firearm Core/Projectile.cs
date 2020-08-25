using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected Collider col;

        public Collider Col { get { return col; } }

        [SerializeField] protected float muzzleVelocity = 169f;

        //[SerializeField] protected float forceDamp = 10f;

        //[SerializeField] protected float baseDamage = 100;
        //[SerializeField] protected float maxDistance = 100;

        [SerializeField] protected Explosive explosive;
        //[SerializeField] protected float extraForce;
        //[SerializeField] protected float trauma = 0.25f;

        public Transform[] unchildOnCollision;

        [SerializeField] protected float pellets;
        [SerializeField] protected bool destroyOnImpact = true;
        [SerializeField] protected float destroyDelay = 0.05f;

        [System.Serializable]
        public struct BulletImpactEffect
        {
            public string hitObjectTag;
            public GameObject impactParticle;
            public AudioClip[] impactAudio;
            public GameObject exitParticle;
            public AudioClip[] exitAudio;
        }

        [SerializeField] protected List<BulletImpactEffect> impactEffects;

        void Start()
        {
            Destroy(gameObject, 7.5f);
        }

        void OnCollisionEnter(Collision col)
        {
            Effect(col, true);

            if (explosive) explosive.enabled = true;

            foreach (Transform child in unchildOnCollision)
                child.parent = null;

            if (destroyOnImpact)
                if (destroyDelay != 0)
                    Destroy(gameObject, destroyDelay);
                else
                    Destroy(gameObject);
        }

        public void Fire()
        {
            rb.AddForce(transform.forward * muzzleVelocity, ForceMode.Impulse);
            Destroy(gameObject, 7.5f);

            GameObject clone = gameObject;

            for (int i = 0; i < pellets; i++)
            {
                Instantiate(gameObject, transform.position, transform.rotation);
                Destroy(clone, 7.5f);
            }
        }

        public void Fire(float muzzleVelocity, float spread)
        {
            rb.AddForce(transform.forward * muzzleVelocity, ForceMode.Impulse);
            Destroy(gameObject, 7.5f);

            Projectile clone = this;
            Vector3 spreadV = Vector3.zero;

            for (int i = 0; i < pellets; i++)
            {
                clone = Instantiate(clone, transform.position, transform.rotation);
                spreadV.x = Random.Range(-spread, spread);
                spreadV.y = Random.Range(-spread, spread);
                spreadV.z = Random.Range(-spread, spread);
                clone.rb.AddForce((transform.forward * muzzleVelocity) + spreadV, ForceMode.Impulse);
                Destroy(clone, 7.5f);
            }
        }

        void Effect(Collision col, bool ENTEREXIT)
        {
            for (int i = 0; i < impactEffects.Count; i++)
            {
                BulletImpactEffect impactEffect = impactEffects[i];

                if (col.gameObject.tag == impactEffect.hitObjectTag)
                {
                    GameObject clone = null;
                    GameObject particleEffect = ENTEREXIT ? impactEffect.impactParticle : impactEffect.exitParticle;
                    //AudioClip audioEffect = ENTEREXIT ? impactEffect.impactAudio[Random.Range(0, impactEffect.impactAudio.Length - 1)] : impactEffect.exitAudio[Random.Range(0, impactEffect.exitAudio.Length - 1)];

                    if (particleEffect)
                    {
                        clone = Instantiate(particleEffect,
                            col.contacts[0].point + col.contacts[0].normal * 0.01f,
                            Quaternion.FromToRotation(Vector3.forward, col.contacts[0].normal)) as GameObject;

                        Destroy(clone, 3f);

                        Transform decal = clone.transform.GetChild(0);

                        if (decal)
                            if (decal.name == "Decal")
                            {
                                decal.SetParent(col.transform, true);
                                Destroy(decal.gameObject, 5f);
                            }
                    }

                    //if (audioEffect)
                    //	AudioSource.PlayClipAtPoint(audioEffect, col.contacts[0].point);
                }
            }
        }
    }
}