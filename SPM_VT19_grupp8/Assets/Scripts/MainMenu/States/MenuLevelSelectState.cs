using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "States/Menu/Main/Level Select State")]
public class MenuLevelSelectState : MenuBaseState
{
    public override void Initialize(StateMachine owner)
    {
        menu = GameObject.Find("LevelSelectState");

        GameObject.Find("LSBack").GetComponent<Button>().onClick.AddListener(Back);
        GameObject.Find("Level1").GetComponent<Button>().onClick.AddListener(StartLevel1);
        GameObject.Find("Level2").GetComponent<Button>().onClick.AddListener(StartLevel2);

        base.Initialize(owner);
    }

    public void StartLevel1()
    {
        owner.levelToLoad = 1;
        owner.TransitionTo<MenuSetPlayerNameState>();
    }

    public void StartLevel2()
    {
        owner.levelToLoad = 2;
        owner.TransitionTo<MenuSetPlayerNameState>();
    }
}
