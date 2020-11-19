using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameState : UIBaseState
{
    public override void EnterState(UIManager context)
    {
        context.GameUI.SetActive(true);
        EventManager.TriggerMouseShouldHide(true);
    }

    public override void ExitState(UIManager context)
    {
        context.GameUI.SetActive(false);
    }

    public override BaseState<UIManager> UpdateState(UIManager context)
    {
        return this;
    }
}
