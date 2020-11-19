using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrollingState : ZombieBaseState
{
    public override void EnterState(ZombieContext context)
    {
        do
        {
            // Calculate the X and Z values to offset with
            float xOffset = Random.Range(-context.visionDistance, context.visionDistance) + context.transform.position.x;
            float zOffset = Random.Range(-context.visionDistance, context.visionDistance) + context.transform.position.z;

            // Set the current target to the random position within our FOV and vision range
            context.currentTarget = new Vector3(xOffset, context.transform.position.y, zOffset);

            context.zombieNavMeshAgent.enabled = true;
            context.zombieNavMeshAgent.destination = context.currentTarget;
            context.zombieNavMeshAgent.speed = context.walkSpeed;
        }
        while (context.zombieNavMeshAgent.path.status != NavMeshPathStatus.PathComplete);

        context.zombieAnimator.SetFloat("Speed_f", context.walkSpeed);
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

        if (base.SeesPlayer(context)) return context.chaseState;
        
        context.PlayTimedSound(this);
        
        if (context.zombieNavMeshAgent.remainingDistance <= context.zombieNavMeshAgent.stoppingDistance) return context.idleState;

        return this;
    }
}
