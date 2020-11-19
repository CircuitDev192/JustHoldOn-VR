using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackingState : ZombieBaseState
{
    private float attackDelay = 1f;
    private float timeToAttack;

    public override void EnterState(ZombieContext context)
    {
        timeToAttack = (timeToAttack > Time.time) ? timeToAttack : Time.time;
    }

    public override void ExitState(ZombieContext context)
    {
        
    }

    public override BaseState<ZombieContext> UpdateState(ZombieContext context)
    {
        if (base.ShouldDie(context)) return context.deadState;

        if(base.ShouldFlee(context)) return context.fleeState;

        float distance = Vector3.Distance(context.transform.position, context.playerTransform.position);

        if (distance / 3 > context.zombieNavMeshAgent.stoppingDistance) return context.chaseState;

        if(Time.time > timeToAttack)
        {
            context.PlaySound(context.attackSounds);

            EventManager.TriggerPlayerDamaged(context.damage);

            timeToAttack = Time.time + attackDelay;
        }

        if (context.playerDead){
            context.zombieAnimator.SetBool("Eating_b", true);
            return context.idleState;
        }

        return this;
    }
}
