using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Chase State")]
public class SoldierChaseState : SoldierBaseState
{
    public override void HandleUpdate()
    {
        owner.agent.SetDestination(owner.playerTransform.position);
        if(owner.agent.pathStatus.Equals(UnityEngine.AI.NavMeshPathStatus.PathPartial))
        {
            NavMeshHit closestEdge;
            NavMesh.SamplePosition(owner.transform.position, out closestEdge, 40, owner.visionMask);
            owner.agent.SetDestination(closestEdge.position);
        }

        if (PlayerVisionCheck(20))
            owner.TransitionTo<SoldierAttackState>();
        else if (!PlayerVisionCheck(40))
            owner.TransitionTo<SoldierAlertState>();
    }
}
