using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Alertstate for Soldier or guard type of enemy. When entered the enemy will go to its last known playerlocation, 
/// if it cant go there it will instead go to the closest edge. If it finds the player within chase range it will 
/// go to <see cref="SoldierChaseState"/> otherwise it will go back to <see cref="SoldierIdleState"/> when it has 
/// reached its destination and if it cant see the player.
/// </summary>
[CreateAssetMenu(menuName = "States/Enemies/Soldier/Alert State")]
public class SoldierAlertState : SoldierBaseState
{
    private Vector3 destination;
    private float coolDown;

    public override void Enter()
    {
        coolDown = 2;
        Agent.SetDestination(PlayerLastLocation);
        if (Agent.pathStatus.Equals(NavMeshPathStatus.PathPartial))
        {
            NavMeshHit closestEdge;
            NavMesh.SamplePosition(Position, out closestEdge, 80, VisionMask);
            destination = closestEdge.position;
            Agent.SetDestination(destination);
        }
    }

    public override void HandleUpdate()
    {
        if (Vector3.Distance(Position, destination) < 2.0f || Agent.velocity.magnitude < MathHelper.floatEpsilon)
        {
            coolDown -= Time.deltaTime;
            if (coolDown < 0)
                owner.TransitionTo<SoldierIdleState>();
        } 
        else if(PlayerVisionCheck(60))
            owner.TransitionTo<SoldierChaseState>();
    }
}
