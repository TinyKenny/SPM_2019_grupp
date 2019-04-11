using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Slide State")]
public class PlayerSlideState : PlayerBaseState
{
    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        ThisCollider.height = ThisCollider.radius * 2;
        ThisCollider.center = new Vector3(0.0f, ThisCollider.radius - StandardColliderHeight / 2, 0.0f);
    }

    public override void HandleUpdate()
    {
        Velocity += Vector3.down * Gravity * PlayerDeltaTime;

        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        Shoot();

        CheckCollision(Velocity * PlayerDeltaTime);

        if (!GroundCheck())
        {
            owner.TransitionTo<PlayerAirState>();
        } else if (Velocity.magnitude < MathHelper.floatEpsilon)
        {
            owner.TransitionTo<PlayerCrouchState>();
        } else if (!Input.GetButton("Crouch"))
        {
            if(FindCollision(Vector3.up, Mathf.Clamp(StandardColliderHeight, SkinWidth + ThisCollider.radius * 2, Mathf.Infinity) - ThisCollider.radius * 2))
            {
                owner.TransitionTo<PlayerCrouchState>();
            } else
            {
                owner.TransitionTo<PlayerWalkingState>();
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        ThisCollider.height = StandardColliderHeight;
        ThisCollider.center = Vector3.zero;
    }
}
