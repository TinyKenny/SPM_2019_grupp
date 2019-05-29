using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "States/Menu/Main/Level Select State")]
public class MenuLevelSelectState : MenuBaseState
{
    public override void Initialize(StateMachine owner)
    {
        menu = ((MainMenuStateMachine)owner).LevelSelectState;

        foreach (Button b in menu.GetComponentsInChildren<Button>())
        {
            buttons.Add(b.name, b);
        }

        buttons["LSBack"].onClick.AddListener(Back);
        buttons["Level1"].onClick.AddListener(StartLevel1);
        buttons["Level2"].onClick.AddListener(StartLevel2);

        base.Initialize(owner);
    }

    public void StartLevel1()
    {
        Owner.levelToLoad = 1;
        Owner.TransitionTo<MenuSetPlayerNameState>();
    }

    public void StartLevel2()
    {
        Owner.levelToLoad = 2;
        Owner.TransitionTo<MenuSetPlayerNameState>();
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(buttons["Level1"].gameObject);
    }
}
