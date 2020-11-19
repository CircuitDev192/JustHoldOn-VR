using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseState : GameBaseState
{
    public override void EnterState(GameManager context)
    {
        Debug.Log("Game entered pause state!");
        EventManager.TriggerGameStateChanged(GameState.Paused);
        Time.timeScale = 0;
        AudioListener.pause = true;
    }

    public override void ExitState(GameManager context)
    {
        Debug.Log("Game exited pause state!");
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    public override BaseState<GameManager> UpdateState(GameManager context)
    {
        //if (Input.GetKeyDown(KeyCode.Escape)) return context.playState;
        // Dont need this, since we have a resume button, and letting the player hit escape again did not lock the cursor..

        return this;
    }
}
