﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "States/Menu/Main/How To Play State")]
public class MenuHowToPlayState : MenuBaseState
{
    public override void Initialize(StateMachine owner)
    {
        menu = GameObject.Find("HowToPlayState");

        GameObject.Find("HTPBack").GetComponent<Button>().onClick.AddListener(Back);

        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(GameObject.Find("HTPBack"));
    }

}
