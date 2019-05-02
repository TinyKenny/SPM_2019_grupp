﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "States/Menu/Main/Main State")]
public class MenuMainState : MenuBaseState
{
    public override void Initialize(StateMachine owner)
    {
        menu = GameObject.Find("MainState");

        GameObject.Find("StartGame").GetComponent<Button>().onClick.AddListener(StartGame);
        GameObject.Find("LevelSelect").GetComponent<Button>().onClick.AddListener(LevelSelect);
        GameObject.Find("Options").GetComponent<Button>().onClick.AddListener(Options);
        GameObject.Find("HowToPlay").GetComponent<Button>().onClick.AddListener(HowToPlay);
        GameObject.Find("Credits").GetComponent<Button>().onClick.AddListener(Credits);
        GameObject.Find("QuitGame").GetComponent<Button>().onClick.AddListener(Quit);

        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(GameObject.Find("StartGame"));
    }

    public void StartGame()
    {
        owner.levelToLoad = 1;
        owner.TransitionTo<MenuSetPlayerNameState>();
    }

    public void LevelSelect()
    {
        owner.TransitionTo<MenuLevelSelectState>();
    }

    public void Options()
    {

    }

    public void HowToPlay()
    {
        owner.TransitionTo<MenuHowToPlayState>();
    }

    public void Credits()
    {
        owner.TransitionTo<MenuCreditsState>();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
