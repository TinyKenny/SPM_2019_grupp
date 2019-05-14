using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Chase State")]
public class SoldierChaseState : SoldierBaseState
{
    public override void HandleUpdate()
    {
        owner.agent.SetDestination(PlayerTransform.position);
        if(owner.agent.pathStatus.Equals(NavMeshPathStatus.PathPartial))
        {
            NavMesh.SamplePosition(owner.transform.position, out NavMeshHit closestEdge, 40, owner.visionMask);
            owner.agent.SetDestination(closestEdge.position);
        }

        if (PlayerVisionCheck(30))
            owner.TransitionTo<SoldierAttackState>();
        else if (!PlayerVisionCheck(60))
            owner.TransitionTo<SoldierAlertState>();
    }
}
