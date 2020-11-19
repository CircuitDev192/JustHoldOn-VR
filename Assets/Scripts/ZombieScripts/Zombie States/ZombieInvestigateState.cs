using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieInvestigateState : ZombieBaseState
{
    public override void EnterState(ZombieContext context)
    {
        context.zombieNavMeshAgent.enabled = true;
        context.zombieNavMeshAgent.destination = context.soundLocation;
        context.zombieAnimator.SetFloat("Speed_f", context.walkSpeed);
    }

    public override void ExitState(ZombieContext context)
    {
        context.heardSound = false;
    }

    public override BaseState<ZombieContext> UpdateState(ZombieContext context)
    {
        if (base.ShouldDie(context)) return context.deadState;

        float distance = Vector3.Distance(context.transform.position, context.playerTransform.position);

        if (distance > context.livingDespawnDistance) return context.despawnState;

        if (base.ShouldFlee(context)) return context.fleeState;

        if (base.SeesPlayer(context)) return context.chaseState;
        
        context.PlayTimedSound(this);

        if (Vector3.Distance(context.zombieNavMeshAgent.destination, context.transform.position) <= context.zombieNavMeshAgent.stoppingDistance) return context.idleState;

        if (context.zombieNavMeshAgent.destination != context.soundLocation) context.zombieNavMeshAgent.destination = context.soundLocation;

        return this;
    }
}
