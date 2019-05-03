using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuBaseState : State
{
    protected MainMenuStateMachine owner;
    protected GameObject menu;
    protected Dictionary<string, Button> buttons = new Dictionary<string, Button>();

    public override void Initialize(StateMachine owner)
    {
        this.owner = (MainMenuStateMachine)owner;
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
        GameObject selected = owner.previousSelected;
        owner.TransitionTo<MenuMainState>();
        if (selected != null && buttons.ContainsKey(owner.previousSelected.name))
            EventSystem.current.SetSelectedGameObject(selected);
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        if (Input.GetButtonDown("Cancel"))
        {
            Back();
        }
    }
}
