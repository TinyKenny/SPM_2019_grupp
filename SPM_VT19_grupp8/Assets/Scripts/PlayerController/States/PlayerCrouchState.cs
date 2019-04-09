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

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        if (grounded && !Input.GetButton("Crouch") && !FindCollision(Transform.forward, Mathf.Epsilon))
        {
            owner.TransitionTo<PlayerWalkingState>();
        }
    }
}
