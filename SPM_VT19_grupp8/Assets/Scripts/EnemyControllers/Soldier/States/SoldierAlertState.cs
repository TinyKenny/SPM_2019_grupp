using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Alert State")]
public class SoldierAlertState : SoldierBaseState
{
    public override void Enter()
    {
        owner.agent.SetDestination(owner.playerLastLocation);
    }

    public override void HandleUpdate()
    {
        if (Vector3.Distance(owner.transform.position, owner.playerLastLocation) < 2.0f)
        {
            owner.TransitionTo<SoldierIdleState>();
        } 
        else if(PlayerVisionCheck(30))
            owner.TransitionTo<SoldierChaseState>();
    }
}
