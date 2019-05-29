using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuBaseState : State
{
    protected MainMenuStateMachine Owner { get; private set; }
    protected GameObject Menu { get; set; }
    protected Dictionary<string, Button> Buttons { get; set; } = new Dictionary<string, Button>();

    protected GameObject PreviousSelected { get { return Owner.PreviousSelected; } }
    protected GameObject MainState { get { return Owner.MainState; } }
    protected GameObject CreditsState { get { return Owner.CreditsState; } }
    protected GameObject HowToPlayState { get { return Owner.HowToPlayState; } }
    protected GameObject LevelSelectState { get { return Owner.LevelSelectState; } }
    protected GameObject SetNameState { get { return Owner.SetNameState; } }
    protected GameObject ColorSelectState { get { return Owner.ColorSelectState; } }
    protected GameObject OptionsState { get { return Owner.OptionsState; } }
    protected GameObject TextField { get { return Owner.TextField; } }
    protected int LevelToLoad { get { return Owner.LevelToLoad; } set { Owner.LevelToLoad = value; } }

    public override void Initialize(StateMachine owner)
    {
        Owner = (MainMenuStateMachine)owner;
        Menu.SetActive(false);
    }

    public override void Enter()
    {
        Menu.SetActive(true);
    }

    public override void Exit()
    {
        Menu.SetActive(false);
    }

    public void Back()
    {
        GameObject selected = PreviousSelected;
        Owner.TransitionTo<MenuMainState>();
        if (selected != null && selected.activeInHierarchy)
            EventSystem.current.SetSelectedGameObject(selected);
        else
            EventSystem.current.SetSelectedGameObject(((MenuMainState)Owner.currentState).Buttons["StartGame"].gameObject);
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        if (Input.GetButtonDown("Cancel") && Owner.currentState.GetType() != typeof(MenuMainState))
        {
            Back();
        }
    }
}
