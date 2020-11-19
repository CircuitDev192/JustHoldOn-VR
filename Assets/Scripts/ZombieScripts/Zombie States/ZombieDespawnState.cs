using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDespawnState : ZombieBaseState
{
    public override void EnterState(ZombieContext context)
    {
        EventManager.TriggerZombieShouldDespawn(context.gameObject);
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
