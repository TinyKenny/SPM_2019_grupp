using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Crouch State")]
public class PlayerCrouchState : PlayerWalkingState
{
    //fixa så att denna kan ärva av PlayerWalkingState?

    //[Range(0.0f, 1.0f)]
    //[SerializeField] private float maxSpeedMultiplier;
    //private float crouchMaxSpeed;


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
