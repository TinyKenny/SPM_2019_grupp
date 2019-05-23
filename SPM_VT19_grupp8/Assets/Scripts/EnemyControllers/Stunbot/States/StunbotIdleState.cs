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
            Target = PatrolLocations[CurrentPatrolPointIndex];
        }
        else
        {
            Target = ThisTransform;
        }
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        if (CanSeePlayer(60.0f))
        {
            Owner.TransitionTo<StunbotChaseState>();
        }
    }

    protected override void UpdateTarget()
    {
        CurrentPatrolPointIndex = (CurrentPatrolPointIndex + 1) % PatrolLocations.Length;
        Target = PatrolLocations[CurrentPatrolPointIndex];
    }
}
