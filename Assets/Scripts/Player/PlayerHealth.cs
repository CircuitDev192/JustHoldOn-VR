using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.TriggerPlayerHealthChanged(health);

        EventManager.PlayerDamaged += Damage;
    }

    //Called by PlayerDamaged Event
    public void Damage(float damage)
    {
        health = Mathf.Clamp((health - damage), 0f, 100f);

        EventManager.TriggerPlayerHealthChanged(health);

        if (health == 0f)
        {
            EventManager.TriggerPlayerKilled();
        }
    }

    private void OnDestroy()
    {
        EventManager.PlayerDamaged -= Damage;
    }
}
