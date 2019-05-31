using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName =  "States/Menu/Main/Leaderboard State")]
public class MenuLeaderboardState : MenuBaseState
{




    public override void Initialize(StateMachine owner)
    {
        Menu = ((MainMenuStateMachine)owner).LeaderboardState;

        foreach(Button b in Menu.GetComponentsInChildren<Button>())
        {
            Buttons.Add(b.name, b);
        }

        //GameObject dingy = Menu.transform.Find("Scores").GetComponentInChildren;

        base.Initialize(owner);



    }

}
