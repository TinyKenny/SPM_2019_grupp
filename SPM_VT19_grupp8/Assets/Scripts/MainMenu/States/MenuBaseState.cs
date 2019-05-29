using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuBaseState : State
{
    protected MainMenuStateMachine Owner { get; private set; }
    protected GameObject menu;
    protected Dictionary<string, Button> buttons = new Dictionary<string, Button>();

    public override void Initialize(StateMachine owner)
    {
        this.Owner = (MainMenuStateMachine)owner;
        menu.SetActive(false);
    }

    public override void Enter()
    {
        menu.SetActive(true);
    }

    public override void Exit()
    {
        menu.SetActive(false);
    }

    public void Back()
    {
        GameObject selected = Owner.PreviousSelected;
        Owner.TransitionTo<MenuMainState>();
        if (selected != null && selected.activeInHierarchy)
            EventSystem.current.SetSelectedGameObject(selected);
        else
            EventSystem.current.SetSelectedGameObject(((MenuMainState)Owner.currentState).buttons["StartGame"].gameObject);
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
