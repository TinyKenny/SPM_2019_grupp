using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Idle State")]
public class SoldierIdleState : SoldierBaseState
{
    public override void HandleUpdate()
    {
        if (PlayerVisioCheck())
        {
            owner.TransitionTo<SoldierAlertState>();
        }
    }
}
