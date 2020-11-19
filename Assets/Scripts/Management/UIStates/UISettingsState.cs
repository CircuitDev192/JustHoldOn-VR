using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingsState : UIBaseState
{
    public override void EnterState(UIManager context)
    {
        context.PauseUI.SetActive(false);
        context.SettingsUI.SetActive(true);
    }

    public override void ExitState(UIManager context)
    {
        context.PauseUI.SetActive(true);
        context.SettingsUI.SetActive(false);
    }

    public override BaseState<UIManager> UpdateState(UIManager context)
    {
        return this;
    }
}
