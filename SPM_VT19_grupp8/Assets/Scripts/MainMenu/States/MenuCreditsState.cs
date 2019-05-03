using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "States/Menu/Main/Credits State")]
public class MenuCreditsState : MenuBaseState
{
    public override void Initialize(StateMachine owner)
    {
        menu = ((MainMenuStateMachine)owner).CreditsState;

        foreach (Button b in menu.GetComponentsInChildren<Button>())
        {
            buttons.Add(b.name, b);
        }
        
        buttons["CreditsBack"].onClick.AddListener(Back);

        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(buttons["CreditsBack"].gameObject);
    }

}
