using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Alert State")]
public class SoldierAlertState : SoldierBaseState
{
    private Vector3 destination;
    private float coolDown;

    public override void Enter()
    {
        coolDown = 2;
        destination = owner.playerLastLocation;
        owner.agent.SetDestination(destination);
        if (owner.agent.pathStatus.Equals(UnityEngine.AI.NavMeshPathStatus.PathPartial))
        {
            NavMeshHit closestEdge;
            NavMesh.SamplePosition(owner.transform.position, out closestEdge, 60, owner.visionMask);
            destination = closestEdge.position;
            owner.agent.SetDestination(destination);
        }
    }

    public override void HandleUpdate()
    {
        if (Vector3.Distance(owner.transform.position, destination) < 2.0f || owner.agent.velocity.magnitude < MathHelper.floatEpsilon)
        {
            coolDown -= Time.deltaTime;
            if (coolDown < 0)
                owner.TransitionTo<SoldierIdleState>();
        } 
        else if(PlayerVisionCheck(60))
            owner.TransitionTo<SoldierChaseState>();
    }
}
