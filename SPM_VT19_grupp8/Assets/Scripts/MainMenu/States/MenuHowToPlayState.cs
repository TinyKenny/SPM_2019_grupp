using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "States/Menu/Main/How To Play State")]
public class MenuHowToPlayState : MenuBaseState
{
    public override void Initialize(StateMachine owner)
    {
        Menu = ((MainMenuStateMachine)owner).HowToPlayState;

        foreach (Button b in Menu.GetComponentsInChildren<Button>())
        {
            Buttons.Add(b.name, b);
        }

        Buttons["HTPBack"].onClick.AddListener(Back);

        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(Buttons["HTPBack"].gameObject);
    }

}
