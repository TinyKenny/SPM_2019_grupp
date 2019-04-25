using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Alert State")]
public class SoldierAlertState : SoldierBaseState
{
    public override void Enter()
    {
        owner.agent.SetDestination(owner.playerLastLocation);
        if (owner.agent.pathStatus.Equals(UnityEngine.AI.NavMeshPathStatus.PathPartial))
        {
            NavMeshHit closestEdge;
            NavMesh.SamplePosition(owner.transform.position, out closestEdge, 40, owner.visionMask);
            owner.agent.SetDestination(closestEdge.position);
        }
    }

    public override void HandleUpdate()
    {
        if (Vector3.Distance(owner.transform.position, owner.playerLastLocation) < 2.0f || owner.agent.velocity.magnitude < MathHelper.floatEpsilon)
        {
            owner.TransitionTo<SoldierIdleState>();
        } 
        else if(PlayerVisionCheck(60))
            owner.TransitionTo<SoldierChaseState>();
    }
}
