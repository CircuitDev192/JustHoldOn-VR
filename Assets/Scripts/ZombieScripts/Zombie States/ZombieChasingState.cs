using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChasingState : ZombieBaseState
{
    private float destinationUpdateTime = 0.5f;
    private float nextUpdateTime;

    public override void EnterState(ZombieContext context)
    {
        context.zombieNavMeshAgent.enabled = true;
        context.zombieNavMeshAgent.speed = context.runSpeed;
        context.currentTarget = context.playerTransform.position;
        context.zombieNavMeshAgent.destination = context.currentTarget;

        context.zombieAnimator.SetFloat("Speed_f", context.runSpeed);

        nextUpdateTime = Time.time + destinationUpdateTime;
    }

    public override void ExitState(ZombieContext context)
    {
        
    }

    public override BaseState<ZombieContext> UpdateState(ZombieContext context)
    {
        if (base.ShouldDie(context)) return context.deadState;

        if (base.ShouldFlee(context)) return context.fleeState;

        context.PlayTimedSound(this);

        context.currentTarget = context.playerTransform.position;

        float distance = Vector3.Distance(context.transform.position, context.currentTarget);

        if (context.zombieNavMeshAgent.remainingDistance <= context.zombieNavMeshAgent.stoppingDistance) return context.attackState;

        if (distance > context.visionDistance) return context.idleState;

        if (Time.time > nextUpdateTime) context.zombieNavMeshAgent.destination = context.currentTarget;

        return this;
    }
}
