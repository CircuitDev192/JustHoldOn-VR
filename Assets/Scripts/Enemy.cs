using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageAble
{
    [SerializeField] private float health;

    public void Damage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
