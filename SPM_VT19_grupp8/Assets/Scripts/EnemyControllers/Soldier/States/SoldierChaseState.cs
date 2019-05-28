using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Solider or guard type of enemy chase state. In this state the enemy will move towards the player until it either 
/// cant see the player and change to <see cref="SoldierAlertState"/>, if the player is close enough and in its vision
/// it will go to <see cref="SoldierAttackState"/>.
/// </summary>
[CreateAssetMenu(menuName = "States/Enemies/Soldier/Chase State")]
public class SoldierChaseState : SoldierBaseState
{
    public override void HandleUpdate()
    {
        Agent.SetDestination(PlayerPosition);
        if(Agent.pathStatus.Equals(NavMeshPathStatus.PathPartial))
        {
            NavMesh.SamplePosition(Position, out NavMeshHit closestEdge, 40, VisionMask);
            Agent.SetDestination(closestEdge.position);
        }

        if (PlayerVisionCheck(40))
            owner.TransitionTo<SoldierAttackState>();
        else if (!PlayerVisionCheck(80))
            owner.TransitionTo<SoldierAlertState>();
    }
}
