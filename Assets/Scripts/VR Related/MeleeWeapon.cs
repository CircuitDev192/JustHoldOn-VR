using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private Collider bladeCollider;
    [SerializeField] private float baseDamage;
    [SerializeField] private Rigidbody rb;
    private float impactForce;

    // Start is called before the first frame update
    void Start()
    {
        if (!bladeCollider)
        {
            Debug.LogError("Melee Weapon Blade Collider Missing.");
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        impactForce = rb.velocity.magnitude;

        IDamageAble damageAble = col.gameObject.GetComponentInParent<IDamageAble>();

        if (damageAble != null)
        {
            damageAble.Damage(baseDamage, col.gameObject.name);
            col.GetComponent<Rigidbody>().AddForce((bladeCollider.ClosestPointOnBounds(col.transform.position) - col.transform.position) * impactForce, ForceMode.Impulse);
        }
    }
}
