using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class MeleeWeapon : MonoBehaviour
    {
        [SerializeField] private Collider bladeCollider;
        [SerializeField] private float baseDamage;
        [SerializeField] private Rigidbody rb;
        private float impactForce;
        private bool justHit;

        private Vector3 oldPos;
        private Vector3 newPos;
        private Vector3 velocityVector;
        private float velocityMagnitude;
        private float relativeVelocityMagnitude;

        [SerializeField] private Item itemScript;
        [SerializeField] private CharacterController character;

        #region Swing and Hit Effects

        [SerializeField] private AudioSource audioSource;

        [SerializeField] private AudioClip slowSwing;
        [SerializeField] private AudioClip mediumSwing;
        [SerializeField] private AudioClip fastSwing;

        [SerializeField] private AudioClip slowHit;
        [SerializeField] private AudioClip mediumHit;
        [SerializeField] private AudioClip fastHit;

        [SerializeField] private GameObject bloodEffect;
        [SerializeField] private GameObject sparkEffect;

        [SerializeField] private float fastThreshold;
        [SerializeField] private float mediumThreshold;
        [SerializeField] private float slowThreshold;

        private float timer = 0;
        private float timeToPlaySound = 0.3f;

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            if (!bladeCollider)
            {
                Debug.LogError("Melee Weapon Blade Collider Missing.");
            }

            oldPos = transform.position;
            timer = timeToPlaySound;
        }

        private void FixedUpdate()
        {
            //Grab velocity vector based on world position each phys update,
            // since Unity doesn't do this for kinematic rigidbodies
            newPos = transform.position;
            velocityVector = (newPos - oldPos);
            velocityMagnitude = (velocityVector / Time.deltaTime).magnitude;
            oldPos = newPos;

            relativeVelocityMagnitude = velocityMagnitude - character.velocity.magnitude;


            if (itemScript.PrimaryHand != null || itemScript.SecondaryHand != null)
            {
                HandleSound();
            }

        }

        private void HandleSound()
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                if (relativeVelocityMagnitude > fastThreshold)
                {
                    audioSource.PlayOneShot(fastSwing, 1f);
                    timer = timeToPlaySound;
                }
                else if (relativeVelocityMagnitude <= fastThreshold && relativeVelocityMagnitude > mediumThreshold)
                {
                    audioSource.PlayOneShot(mediumSwing, 1f);
                    timer = timeToPlaySound;
                }
                else if (relativeVelocityMagnitude <= mediumThreshold && relativeVelocityMagnitude > slowThreshold)
                {
                    audioSource.PlayOneShot(slowSwing, 1f);
                    timer = timeToPlaySound;
                }
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            IDamageAble damageAble = col.gameObject.GetComponentInParent<IDamageAble>();
            Rigidbody hitRb = col.gameObject.GetComponent<Rigidbody>();

            if (damageAble != null && velocityMagnitude >= 3.5f)
            {
                damageAble.Damage(baseDamage, col.gameObject.name);
                if (hitRb) hitRb.AddForce((bladeCollider.ClosestPointOnBounds(col.transform.position) - col.transform.position) * velocityMagnitude, ForceMode.Impulse);

                GameObject effect = Instantiate(bloodEffect, bladeCollider.ClosestPointOnBounds(col.transform.position), Quaternion.Euler(col.transform.position - bladeCollider.ClosestPointOnBounds(col.transform.position)).normalized);

                Destroy(effect, 3f);

                if (!justHit)
                {
                    if (relativeVelocityMagnitude > fastThreshold)
                    {
                        AudioSource.PlayClipAtPoint(fastHit, bladeCollider.ClosestPointOnBounds(col.transform.position));
                    } else if (relativeVelocityMagnitude <= fastThreshold && relativeVelocityMagnitude > mediumThreshold)
                    {
                        AudioSource.PlayClipAtPoint(mediumHit, bladeCollider.ClosestPointOnBounds(col.transform.position));
                    } else
                    {
                        AudioSource.PlayClipAtPoint(slowHit, bladeCollider.ClosestPointOnBounds(col.transform.position));
                    }
                    justHit = true;
                    StartCoroutine(WaitToHitAgain());
                }
                
            }
            else if (relativeVelocityMagnitude >= slowThreshold)
            {
                if (col.gameObject.CompareTag("EnvMetal") ||
                    col.gameObject.CompareTag("EnvConcrete"))
                { 
                    GameObject effect = Instantiate(sparkEffect, bladeCollider.ClosestPointOnBounds(col.transform.position), Quaternion.Euler(col.transform.position - bladeCollider.ClosestPointOnBounds(col.transform.position)).normalized);

                    Destroy(effect, 3f);
                }
            }
        }

        IEnumerator WaitToHitAgain()
        {
            yield return new WaitForSeconds(0.5f);
            justHit = false;
        }
    }
}
