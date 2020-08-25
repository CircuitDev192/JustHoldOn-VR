using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public delegate void HealthEvent(Health health);

    public HealthEvent _OnKilled;
    public HealthEvent _OnDamaged;
    public HealthEvent _OnHealed;

    public delegate void EffectEvent(float trauma, float collisionForce);
    public EffectEvent _OnEffect;

    [SerializeField] protected float current = 100;
    [SerializeField] protected float max = 100;
    [SerializeField] protected bool destroyOnDeath;

    public float CurrentHealth { get { return current; } }
    public float MaxHealth { get { return max; } }

    public Vector3 damagePoint;
    public Vector3 source;

    public float regenPerTick;
    public float regenTick;

    public bool kill = false;

    [SerializeField] protected Transform deathPFX;

    void Update()
    {
        if (kill)
        {
            ApplyDamage(15);
            kill = false;
        }
    }

    IEnumerator Start()
    {
        WaitForSeconds tick = new WaitForSeconds(regenTick);

        while (true)
        {
            if (CurrentHealth < MaxHealth)
                ApplyHealing(regenPerTick);

            yield return tick;
        }
    }

    public void Kill()
    {
        ApplyDamage(MaxHealth);
    }

    public void ApplyHealing(float healing)
    {
        if (healing < 0)
        {
            ApplyDamage(-healing);
            return;
        }

        current = Mathf.Clamp(current + healing, 0, max);

        if (_OnHealed != null)
            _OnHealed(this);
    }

    public void ApplyDamage(float damage)
    {
        if (damage == 0)
            return;

        bool alive = CurrentHealth > 0;

        if (damage < 0)
        {
            ApplyHealing(-damage);
            return;
        }

        current = Mathf.Clamp(current - damage, 0, max);

        damageType = 0;

        if (_OnDamaged != null && alive)
            _OnDamaged(this);

        if (Mathf.Approximately(current, 0.0f) && alive)
            OnDeath();

        damagePoint = Vector3.zero;
        source = Vector3.zero;
    }

    public void ApplyDamage(float damage, Vector3 damagePoint)
    {
        damageType = 0;

        ApplyDamage(damage);

        this.damagePoint = damagePoint;
    }

    public void ApplyDamage(float damage, Vector3 source, Vector3 damagePoint)
    {
        damageType = 0;

        ApplyDamage(damage);

        this.damagePoint = damagePoint;
        this.source = source;
    }

    public void DeathSFX(Vector3 position, Vector3 rotation)
    {
        if (!deathPFX) return;

        Transform tempPFX = Instantiate(deathPFX, position, Quaternion.LookRotation(rotation), transform);
        Destroy(tempPFX.gameObject, 6f);
    }

    public void ApplyDamageOverTime(float tick, float amount, float time)
    { StartCoroutine(_ApplyDamageOverTime(tick, amount, time)); }

    protected virtual IEnumerator _ApplyDamageOverTime(float tick, float amount, float time)
    {
        float tempTime = 0;
        WaitForSeconds wait = new WaitForSeconds(tick);
        while (Time.time - tempTime < time)
        {
            ApplyDamage(amount);
            yield return wait;
        }
    }

    protected virtual void OnDeath()
    {
        if (_OnKilled != null)
            _OnKilled(this);



        if (destroyOnDeath) Destroy(this.gameObject);
    }

    public float TraumaMultiplier = 1f;
    public float CollisionTraumaMultiplier = 1f;

    public void ApplyTrauma(float trauma)
    {
        if (_OnEffect != null)
        {
            _OnEffect(trauma * TraumaMultiplier, 0);
        }
    }

    public float collisionForceDamageMult;
    public float impactForceThreshold = 1;

    public int damageType;

    void OnCollisionEnter(Collision col)
    {
        float impactForce = col.relativeVelocity.magnitude;

        if (impactForce >= impactForceThreshold)
        {
            if (_OnEffect != null)
                _OnEffect(0, impactForce * CollisionTraumaMultiplier);

            damageType = 1;
            ApplyDamage(impactForce * collisionForceDamageMult);
        }
    }
}