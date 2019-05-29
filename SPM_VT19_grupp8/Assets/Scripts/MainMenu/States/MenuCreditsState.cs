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
        Menu = ((MainMenuStateMachine)owner).CreditsState;

        foreach (Button b in Menu.GetComponentsInChildren<Button>())
        {
            Buttons.Add(b.name, b);
        }
        
        Buttons["CreditsBack"].onClick.AddListener(Back);

        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(Buttons["CreditsBack"].gameObject);
    }

}
