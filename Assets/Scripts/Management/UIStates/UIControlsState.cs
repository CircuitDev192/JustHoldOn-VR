using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControlsState : UIBaseState
{
    public override void EnterState(UIManager context)
    {
        context.PauseUI.SetActive(false);
        context.ControlsUI.SetActive(true);
    }

    public override void ExitState(UIManager context)
    {
        context.PauseUI.SetActive(true);
        context.ControlsUI.SetActive(false);
    }

    public override BaseState<UIManager> UpdateState(UIManager context)
    {
        return this;
    }
}