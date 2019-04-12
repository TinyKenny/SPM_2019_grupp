using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "States/Menu/Main/Credits State")]
public class MenuCreditsState : MenuBaseState
{
    public override void Initialize(StateMachine owner)
    {
        menu = GameObject.Find("CreditsState");

        GameObject.Find("CreditsBack").GetComponent<Button>().onClick.AddListener(Back);

        base.Initialize(owner);
    }

}
