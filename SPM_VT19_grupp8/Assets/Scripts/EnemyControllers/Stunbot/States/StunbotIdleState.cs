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
            Paths.Clear();
            Paths.Add(ThisTransform.position);
        }
    }

    public override void HandleUpdate()
    {
        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);

        if (Paths.Count > 0)
        {
            FlyToTarget(Paths[0]);
        }

        base.HandleUpdate();

        if (CanSeePlayer(60.0f))
        {
            Owner.TransitionTo<StunbotChaseState>();
        }
    }

    protected override void NoTargetAvailable()
    {
        Paths.Add(PatrolLocations[CurrentPatrolPointIndex].position);

        if (Vector3.Distance(Paths[0], ThisTransform.position) < Mathf.Max(Velocity.magnitude * 0.1f, 0.1f))
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
