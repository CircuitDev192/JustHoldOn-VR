using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuState : GameBaseState
{
    public override void EnterState(GameManager context)
    {
        EventManager.TriggerGameStateChanged(GameState.MainMenu);
    }

    public override void ExitState(GameManager context)
    {
       
    }

    public override BaseState<GameManager> UpdateState(GameManager context)
    {
        if (Input.GetKeyDown(KeyCode.Return)) return context.playState;

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        return this;
    }
}
