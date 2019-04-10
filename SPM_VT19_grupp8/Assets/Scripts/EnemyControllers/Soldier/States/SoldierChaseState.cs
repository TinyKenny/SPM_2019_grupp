using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Chase State")]
public class SoldierChaseState : SoldierBaseState
{
    public override void HandleUpdate()
    {
        owner.agent.SetDestination(owner.playerTransform.position);

        if (PlayerVisioCheck(20))
            owner.TransitionTo<SoldierAlertState>();
        else if (!PlayerVisioCheck(30))
            owner.TransitionTo<SoldierIdleState>();
    }
}
