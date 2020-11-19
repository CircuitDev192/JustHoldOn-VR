using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseState : UIBaseState
{
    public override void EnterState(UIManager context)
    {
        context.PauseUI.SetActive(true);
        EventManager.TriggerMouseShouldHide(false);
    }

    public override void ExitState(UIManager context)
    {
        context.PauseUI.SetActive(false);
    }

    public override BaseState<UIManager> UpdateState(UIManager context)
    {
        return this;
    }
}
