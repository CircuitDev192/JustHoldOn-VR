using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDeadState : ZombieBaseState
{
    public override void EnterState(ZombieContext context)
    {
        EventManager.TriggerZombieKilled(context.gameObject);
        context.EnableRagdoll();
        EventManager.SoundGenerated -= context.SoundGenerated;
    }

    public override void ExitState(ZombieContext context)
    {
        
    }

    public override BaseState<ZombieContext> UpdateState(ZombieContext context)
    {
        return this;
    }
}
