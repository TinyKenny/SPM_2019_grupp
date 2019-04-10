using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Chase State")]
public class SoldierChaseState : SoldierBaseState
{
    public override void HandleUpdate()
    {
        owner.agent.SetDestination(owner.playerTransform.position);

        if (PlayerVisionCheck(20))
            owner.TransitionTo<SoldierAttackState>();
        else if (!PlayerVisionCheck(40))
            owner.TransitionTo<SoldierAlertState>();
    }
}
