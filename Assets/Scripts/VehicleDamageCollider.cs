using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleDamageCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        IDamageAble damageAble = col.gameObject.GetComponentInParent<IDamageAble>();

        if (damageAble != null)
        {
            damageAble.Damage(500f, col.gameObject.name);
        }
    }
}