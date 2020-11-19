using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleState : ZombieBaseState
{
    private float idleTime;
    private float timeToPatrol;

    public override void EnterState(ZombieContext context)
    {
        context.zombieNavMeshAgent.enabled = false;
        context.zombieAnimator.SetFloat("Speed_f", 0f);

        idleTime = Random.Range(context.minimumIdleTime, context.maximumIdleTime);
        timeToPatrol = Time.time + idleTime;
    }

    public override void ExitState(ZombieContext context)
    {
        
    }

    public override BaseState<ZombieContext> UpdateState(ZombieContext context)
    {
        if (base.ShouldDie(context)) return context.deadState;

        float distance = Vector3.Distance(context.transform.position, context.playerTransform.position);

        if (distance > context.livingDespawnDistance) return context.despawnState;

        if (base.ShouldFlee(context)) return context.fleeState;

        if (context.heardSound) return context.investigateState;

        if (context.playerDead) return this;

        if (base.SeesPlayer(context)) return context.chaseState;

        if (Time.time > timeToPatrol) return context.patrolState;
        
        context.PlayTimedSound(this);

        return this;
    }
}
