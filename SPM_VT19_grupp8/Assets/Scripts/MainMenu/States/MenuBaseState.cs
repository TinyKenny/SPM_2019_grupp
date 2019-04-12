using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBaseState : State
{
    protected MainMenuStateMachine owner;
    protected GameObject menu;

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
        owner.TransitionTo<MenuMainState>();
    }
}
