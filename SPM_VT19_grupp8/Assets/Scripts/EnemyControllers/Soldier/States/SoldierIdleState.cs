using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (Vector3.Distance(closestPoint, owner.transform.position) > Vector3.Distance(PatrolLocations[i].position, owner.transform.position))
            {
                closestPoint = PatrolLocations[i].position;
                index = i;
            }
        } 
        owner.agent.SetDestination(closestPoint);
    }

    public override void HandleUpdate()
    {
        if (Vector3.Distance(owner.transform.position, closestPoint) < 1f)
        {
            if (index < PatrolLocations.Length - 1)
            {
                closestPoint = PatrolLocations[++index].position;
                owner.agent.SetDestination(closestPoint);
            }
            else
            {
                index = 0;
                closestPoint = PatrolLocations[index].position;
                owner.agent.SetDestination(closestPoint);
            }
        }
            

        if (PlayerVisionCheck(60))
        {
            owner.TransitionTo<SoldierChaseState>();
        }
    }
}
