using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Booped State")]
public class StunbotBoopedState : StunbotBaseState
{
    private float boopMultiplier = 1.5f; // 2.0f

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        LastPlayerLocation = PlayerTransform.position;
        Velocity = Velocity.normalized * MaxSpeed * boopMultiplier;
    }

    public override void HandleUpdate()
    {
        Decelerate();
        ApplyMovement(Velocity * Time.deltaTime);

        if(Velocity.sqrMagnitude <= MaxSpeed * MaxSpeed * MathHelper.floatEpsilon)
        {
            if (CanSeePlayer(60.0f))
            {
                Owner.TransitionTo<StunbotChaseState>();
            }
            else
            {
                Owner.TransitionTo<StunbotSearchState>();
            }
        }
    }
}
