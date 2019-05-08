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
        ThisCollider.height = ThisCollider.radius * 2;
        ThisCollider.center = new Vector3(0.0f, ThisCollider.radius - StandardColliderHeight / 2, 0.0f);
        owner.GetComponentInChildren<Animator>().SetTrigger("Crouch");
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        if (grounded && !Input.GetButton("Crouch") && 
            !FindCollision(Vector3.up, Mathf.Clamp(StandardColliderHeight, SkinWidth + ThisCollider.radius * 2, Mathf.Infinity) - ThisCollider.radius * 2))
        {
            owner.TransitionTo<PlayerWalkingState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
        ThisCollider.height = StandardColliderHeight;
        ThisCollider.center = Vector3.zero;
    }
}
