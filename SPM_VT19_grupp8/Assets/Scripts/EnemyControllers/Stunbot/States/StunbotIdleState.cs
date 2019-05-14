﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Idle State")]
public class StunbotIdleState : StunbotBaseState
{

    public override void Enter()
    {
        base.Enter();
        FindTarget();
    }

    //protected override void FindTarget()
    //{
    //    NextTargetPosition = owner.transform.position;
    //    owner.PathFinder.FindPath(ThisTransform.position, owner.patrolLocations[CurrentPatrolPointIndex].position);
    //    if (owner.PathFinder.Paths.Count > 0)
    //    {
    //        // this is copied from HandleUpdate()
    //        float f = 0;
    //        foreach (KeyValuePair<float, Vector3> pos in owner.PathFinder.Paths)
    //        {
    //            NextTargetPosition = pos.Value;
    //            f = pos.Key;
    //            break;
    //        }

    //        owner.PathFinder.Paths.Remove(f);
    //    }
    //}

    public override void HandleUpdate()
    {
        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);

        FlyToTarget(NextTargetPosition);

        base.HandleUpdate();

        if (CanSeePlayer(60.0f)
            /*&& Vector3.Distance(ThisTransform.position, owner.patrolLocations[CurrentPatrolPointIndex].position) < MaxSpeed * 0.1f*/
            /*&& Vector3.Distance(PlayerTransform.position, owner.patrolLocations[0].position) < owner.allowedOriginDistance*/)
        {
            Debug.Log("Idle -> Chase (player found)");
            owner.TransitionTo<StunbotChaseState>();
        }
    }

    protected override void NoTargetAvailable()
    {
        NextTargetPosition = PatrolLocations[CurrentPatrolPointIndex].position;

        if (Vector3.Distance(NextTargetPosition, ThisTransform.position) < Mathf.Max(Velocity.magnitude * 0.1f, 0.1f))
        {
            if (PatrolLocations.Length == 1)
            {
                if (Velocity.magnitude > 0.1f)
                {
                    Velocity = Vector3.zero;
                }
            }
            else
            {
                CurrentPatrolPointIndex = (CurrentPatrolPointIndex + 1) % PatrolLocations.Length;
                FindTarget();
            }
        }
    }
}
