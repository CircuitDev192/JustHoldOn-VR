using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCreditsState : GameBaseState
{
    public override void EnterState(GameManager context)
    {
        EventManager.TriggerGameStateChanged(GameState.Credits);
    }

    public override void ExitState(GameManager context)
    {
        throw new System.NotImplementedException();
    }

    public override BaseState<GameManager> UpdateState(GameManager context)
    {
        throw new System.NotImplementedException();
    }
}
