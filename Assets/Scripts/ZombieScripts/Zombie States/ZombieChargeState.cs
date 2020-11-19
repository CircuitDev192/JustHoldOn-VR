using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChargeState : ZombieBaseState
{
    public override void EnterState(ZombieContext context)
    {
        context.livingDespawnDistance = 300f;
        context.deadDespawnDistance = 60f;
        context.zombieNavMeshAgent.enabled = true;
        context.zombieNavMeshAgent.speed = context.walkSpeed;
        context.currentTarget = context.chargeTransform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        context.zombieNavMeshAgent.destination = context.currentTarget;

        context.zombieAnimator.SetFloat("Speed_f", context.walkSpeed);
    }

    public override void ExitState(ZombieContext context)
    {

    }

    public override BaseState<ZombieContext> UpdateState(ZombieContext context)
    {
        if (base.ShouldDie(context)) return context.deadState;

        context.PlayTimedSound(this);

        return this;
    }
}
