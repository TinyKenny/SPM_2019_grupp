using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Default idle state for enemy guard or soldier type of enemy. During this state the enemy will patrol between its patrolpoints, 
/// if it hears the player it will go to <see cref="SoldierAlertState"/>, if it sees the player within range it will go to 
/// <see cref="SoldierChaseState"/>.
/// </summary>
[CreateAssetMenu(menuName = "States/Enemies/Soldier/Idle State")]
public class SoldierIdleState : SoldierBaseState
{
    private Vector3 closestPoint;
    private int index;

    public override void Enter()
    {
        closestPoint = Vector3.positiveInfinity;

        for (int i = 0; i < PatrolLocations.Length; i++)
        {  
            if (Vector3.Distance(closestPoint, Position) > Vector3.Distance(PatrolLocations[i].position, Position))
            {
                closestPoint = PatrolLocations[i].position;
                index = i;
            }
        } 
        Agent.SetDestination(closestPoint);
    }

    public override void HandleUpdate()
    {
        if (Vector3.Distance(Position, closestPoint) < 1f)
        {
            if (index < PatrolLocations.Length - 1)
            {
                closestPoint = PatrolLocations[++index].position;
                Agent.SetDestination(closestPoint);
            }
            else
            {
                index = 0;
                closestPoint = PatrolLocations[index].position;
                Agent.SetDestination(closestPoint);
            }
        }
            

        if (PlayerVisionCheck(60))
        {
            owner.TransitionTo<SoldierChaseState>();
        }
    }
}
