using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Crouch State")]
public class PlayerCrouchState : PlayerWalkingState
{

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        jumpAllowed = false;
    }

    public override void Enter()
    {
        base.Enter();
        ThisCollider.direction = 2;
        ThisCollider.center = new Vector3(0.0f, ThisCollider.radius - StandardColliderHeight / 2, StandardColliderHeight / 2 - ThisCollider.radius);
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        if (grounded && !Input.GetButton("Crouch") && !FindCollision(Transform.forward, Mathf.Epsilon))
        {
            owner.TransitionTo<PlayerWalkingState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
        ThisCollider.direction = 1;
        ThisCollider.center = Vector3.zero;
    }
}
