using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private Collider bladeCollider;
    [SerializeField] private float baseDamage;
    [SerializeField] private Rigidbody rb;
    private float impactForce;

    private Vector3 oldPos;
    private Vector3 newPos;
    private Vector3 velocityVector;
    private float velocityMagnitude;

    // Start is called before the first frame update
    void Start()
    {
        if (!bladeCollider)
        {
            Debug.LogError("Melee Weapon Blade Collider Missing.");
        }

        oldPos = transform.position;
    }

    private void FixedUpdate()
    {
        //Grab velocity vector based on world position each phys update,
        // since Unity doesn't do this for kinematic rigidbodies
        newPos = transform.position;
        velocityVector = (newPos - oldPos);
        velocityMagnitude = (velocityVector / Time.deltaTime).magnitude;
        oldPos = newPos;
    }

    private void OnTriggerEnter(Collider col)
    {
        IDamageAble damageAble = col.gameObject.GetComponentInParent<IDamageAble>();

        if (damageAble != null && velocityMagnitude >= 3.5f)
        {
            damageAble.Damage(baseDamage, col.gameObject.name);
            col.GetComponent<Rigidbody>().AddForce((bladeCollider.ClosestPointOnBounds(col.transform.position) - col.transform.position) * velocityMagnitude, ForceMode.Impulse);
        }
    }
}
