using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayState : GameBaseState
{
    public override void EnterState(GameManager context)
    {
        EventManager.TriggerGameStateChanged(GameState.Play);
        //context.UnLoadScene("Menu");
        //context.LoadScene("Game");
    }

    public override void ExitState(GameManager context)
    {
        
    }

    public override BaseState<GameManager> UpdateState(GameManager context)
    {
        if (Input.GetKeyDown(KeyCode.Escape)) return context.pauseState;

        return this;
    }
}
