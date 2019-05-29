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
        Menu = ((MainMenuStateMachine)owner).LevelSelectState;

        foreach (Button b in Menu.GetComponentsInChildren<Button>())
        {
            Buttons.Add(b.name, b);
        }

        Buttons["LSBack"].onClick.AddListener(Back);
        Buttons["Level1"].onClick.AddListener(StartLevel1);
        Buttons["Level2"].onClick.AddListener(StartLevel2);

        base.Initialize(owner);
    }

    public void StartLevel1()
    {
        LevelToLoad = 1;
        Owner.TransitionTo<MenuSetPlayerNameState>();
    }

    public void StartLevel2()
    {
        LevelToLoad = 2;
        Owner.TransitionTo<MenuSetPlayerNameState>();
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(Buttons["Level1"].gameObject);
    }
}
