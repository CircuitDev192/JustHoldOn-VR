using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Context<UIManager>
{
    #region States

    public UIMenuState menuState = new UIMenuState();
    public UIGameState gameState = new UIGameState();
    public UIPauseState pauseState = new UIPauseState();
    public UIControlsState controlsState = new UIControlsState();
    public UISettingsState settingsState = new UISettingsState();
    public UICreditsState creditsState = new UICreditsState();

    #endregion

    public GameObject MenuUI;
    public GameObject GameUI;
    public GameObject ControlsUI;
    public GameObject SettingsUI;
    public GameObject PauseUI;

    private void Awake()
    {
        EventManager.GameStateChanged += GameStateChanged;
        EventManager.UIControlsClicked += UIControlsButtonClicked;
        EventManager.UISettingsClicked += UISettingsButtonClicked;
        EventManager.UIControlsBackClicked += UIControlsButtonBackClicked;
        EventManager.UISettingsBackClicked += UISettingsButtonBackClicked;
        InitializeContext();
    }

    private void UISettingsButtonBackClicked()
    {
        currentState.ExitState(this);
        currentState = pauseState;
        currentState.EnterState(this);
    }

    private void UISettingsButtonClicked()
    {
        currentState.ExitState(this);
        currentState = settingsState;
        currentState.EnterState(this);
    }

    private void UIControlsButtonClicked()
    {
        currentState.ExitState(this);
        currentState = controlsState;
        currentState.EnterState(this);
    }

    private void UIControlsButtonBackClicked()
    {
        currentState.ExitState(this);
        currentState = pauseState;
        currentState.EnterState(this);
    }

    public override void InitializeContext()
    {
        currentState = menuState;
        currentState.EnterState(this);
    }

    private void GameStateChanged(GameState gameState)
    {
        currentState.ExitState(this);

        switch(gameState)
        {
            case GameState.MainMenu:
                currentState = menuState;
                break;

            case GameState.Play:
                currentState = this.gameState;
                break;

            case GameState.Paused:
                currentState = pauseState;
                break;

            case GameState.Controls:
                currentState = controlsState;
                break;

            case GameState.Credits:
                currentState = creditsState;
                break;
        }

        currentState.EnterState(this);
    }

    private void OnDestroy()
    {
        EventManager.GameStateChanged -= GameStateChanged;
        EventManager.UIControlsClicked -= UIControlsButtonClicked;
        EventManager.UISettingsClicked -= UISettingsButtonClicked;
        EventManager.UIControlsBackClicked -= UIControlsButtonBackClicked;
        EventManager.UISettingsBackClicked -= UISettingsButtonBackClicked;
    }
}
