using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]

public abstract class ZombieContext : Context<ZombieContext>, IDamageAble
{
    #region Fields

    #region Zombie States

    public ZombieIdleState idleState = new ZombieIdleState();
    public ZombiePatrollingState patrolState = new ZombiePatrollingState();
    public ZombieInvestigateState investigateState = new ZombieInvestigateState();
    public ZombieChasingState chaseState = new ZombieChasingState();
    public ZombieAttackingState attackState = new ZombieAttackingState();
    public ZombieDeadState deadState = new ZombieDeadState();
    public ZombieDespawnState despawnState = new ZombieDespawnState();
    public ZombieFleeState fleeState = new ZombieFleeState();
    public ZombieChargeState chargeState = new ZombieChargeState();

    #endregion

    public Animator zombieAnimator;

    // Movement variables
    public NavMeshAgent zombieNavMeshAgent;
    public float epsilon = 0.1f;
    public float walkSpeed;
    public float minimumRunSpeed;
    public float maximumRunSpeed;
    public float runSpeed;
    public float currentSpeed;
    public Transform chargeTransform;

    // Health generation variables
    public float minimumHealth;
    public float maximumHealth;
    public float health;

    // Damage generation variables
    public float minimumDamage;
    public float maximumDamage;
    public float damage;

    // Calculation distance values
    public float fieldOfView;
    public float visionDistance;
    public float deadDespawnDistance;
    public float livingDespawnDistance;

    // Illumination Intensity Flee Threshold
    public float fleeThreshold = 10f;
    // Flee vector
    public Vector3 fleeVector;

    // Idle time range
    public float minimumIdleTime;
    public float maximumIdleTime;

    // Track player and current target position
    public Transform playerTransform;
    public Vector3 currentTarget;

    public bool playerDead = false;
    public bool heardSound = false;
    public Vector3 soundLocation;
    
    // Sound effects
    public AudioClip[] idleSounds;
    public AudioClip[] attackSounds;
    public AudioClip[] hurtSounds;
    public AudioClip[] deathSounds;
    public AudioClip[] footstepSounds;

    public float minTimeBetweenSounds;
    public float maxTimeBetweenSounds;
    public float nextSoundTime;
    public float nextFootstepTime;

    public AudioSource audioSource;

    #endregion

    public override void InitializeContext()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        zombieNavMeshAgent.enabled = true;
        if (!zombieNavMeshAgent.isOnNavMesh)
        {
            currentState = despawnState;
            currentState.EnterState(this);
            return;
        }

        DisableRagdoll();

        EventManager.PlayerKilled += PlayerKilled;
        EventManager.SoundGenerated += SoundGenerated;
        EventManager.ZombieCharge += ZombieCharge;

        playerTransform = PlayerManager.instance.player.transform;
        currentSpeed = walkSpeed;
        runSpeed = Random.Range(minimumRunSpeed, maximumRunSpeed);
        health = Random.Range(minimumHealth, maximumHealth);
        damage = Random.Range(minimumDamage, maximumDamage);
        
        nextSoundTime = Time.time + Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
        nextFootstepTime = Time.time;
        
        currentState = idleState;
        idleState.EnterState(this);
    }

    private void ZombieCharge(Transform chargeTransform)
    {
        if (currentState == idleState || currentState == patrolState || currentState == investigateState)
        {
            this.chargeTransform = chargeTransform;
            currentState.ExitState(this);
            currentState = chargeState;
            currentState.EnterState(this);
        }
    }

    public void Damage(float damage, string limbName)
    {
        if (currentState == deadState) return;

        //calculate final damage based on limb hit using limb name
        float finalDamage;
        switch (limbName)
        {
            case "Head_jnt":
                finalDamage = health;
                break;
            case "Chest_jnt":
            case "Hips_jnt":
                finalDamage = damage;
                break;
            default:
                finalDamage = 0.7f * damage;
                break;
        }



        health -= finalDamage;

        if (health <= 0){
            Debug.LogWarning("Played death sound.");
            this.PlaySound(deathSounds);
            currentState = deadState;
        }
        else{
            Debug.LogWarning("Played hurt sound.");
            this.PlaySound(hurtSounds);
            //For the last mission, we dont want the zombies attacking the inactive player object,
            // so check to make sure the zombie isnt charging first before switching to chase.
            if (currentState != chargeState)
            {
                currentState = chaseState;
            }
        }

        currentState.EnterState(this);
    }

    public void DisableRagdoll()
    {
        Rigidbody[] rigidBodies = this.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;
        }
    }

    public void EnableRagdoll()
    {
        zombieAnimator.enabled = false;
        zombieNavMeshAgent.enabled = false;

        Rigidbody[] rigidBodies = this.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = false;
            rigidBody.useGravity = true;
        }
    }

    private void PlayerKilled()
    {
        playerDead = true;
    }

    public void SoundGenerated(Vector3 location, float audibleDistance)
    {
        if (Vector3.Distance(this.transform.position, location) > audibleDistance) return;

        heardSound = true;
        soundLocation = location;
    }
    
    public void PlayTimedSound(ZombieBaseState state)
    {
        PlayFootStepSound(state);

        float distance = Vector3.Distance(this.transform.position, this.playerTransform.position);
        if (distance > this.visionDistance) return;

        if (Time.time < this.nextSoundTime) return;

        float pauseScale = 1.0F;

        if (state == this.idleState || state == this.patrolState || state == this.investigateState)
        {
            Debug.LogWarning("Played idle sound.");
            PlaySound(this.idleSounds);
            pauseScale = 2.5F;
        }
        else if(state == this.chaseState)
        {
            Debug.LogWarning("Played chase sound.");
            PlaySound(this.attackSounds);
        }

        this.nextSoundTime = Time.time + Random.Range(this.minTimeBetweenSounds, this.maxTimeBetweenSounds) * pauseScale;
    }

    public void PlayFootStepSound(ZombieBaseState state)
    {
        if (nextFootstepTime > Time.time) return;

        if (state == this.idleState) return;

        PlaySound(footstepSounds);

        nextFootstepTime = Time.time + Mathf.Clamp(1.5f / this.zombieNavMeshAgent.speed, 0.4f, 1f);
    }

    public void PlaySound(AudioClip[] clipArray){
        int index = Random.Range(0, clipArray.Length);
        AudioClip clipToPlay = clipArray[index];

        audioSource.clip = clipToPlay;
        audioSource.pitch = Random.Range(0.75F, 1.25F);
        audioSource.Play();
    }

    private void OnDestroy()
    {
        EventManager.PlayerKilled -= PlayerKilled;
        EventManager.SoundGenerated -= SoundGenerated;
        EventManager.ZombieCharge -= ZombieCharge;
    }
}