using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Idle State")]
public class StunbotIdleState : StunbotBaseState
{

    public override void Enter()
    {
        base.Enter();
        if (PatrolLocations != null)
        {
            FindTarget(PatrolLocations[CurrentPatrolPointIndex].position);
        }
        else
        {
            NextTargetPosition = ThisTransform.position;
        }
    }

    public override void HandleUpdate()
    {
        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);

        FlyToTarget(NextTargetPosition);

        base.HandleUpdate();

        if (false && CanSeePlayer(60.0f))
        {
            Owner.TransitionTo<StunbotChaseState>();
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
                FindTarget(PatrolLocations[CurrentPatrolPointIndex].position);
            }
        }
    }
}
