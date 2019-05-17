using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Slide State")]
public class PlayerSlideState : PlayerBaseState
{
    [Range(0.0f, 1.0f)]
    public float decelerationMultiplier = 0.1f;

    private bool standingObstructed;
    private bool grounded;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        ThisCollider.height = ThisCollider.radius * 2;
        ThisCollider.center = new Vector3(0.0f, ThisCollider.radius - StandardColliderHeight / 2, 0.0f);
        Animator.SetTrigger("Slide");
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        Velocity += Vector3.down * Gravity * PlayerDeltaTime;

        SlidingDecelerate();

        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        Shoot();

        standingObstructed = FindCollision(Vector3.up, Mathf.Clamp(StandardColliderHeight, SkinWidth + ThisCollider.radius * 2, Mathf.Infinity) - ThisCollider.radius * 2);
        grounded = GroundCheck();

        if(grounded && standingObstructed == false && Input.GetButton("Jump"))
        {
            Debug.Log("slide jump");
            Velocity += Vector3.up * JumpPower;
            Owner.TransitionTo<PlayerAirState>();
        }

        CheckCollision(Velocity * PlayerDeltaTime);

        if (!grounded)
        {
            Owner.TransitionTo<PlayerAirState>();
        }
        else if (Velocity.magnitude < MathHelper.floatEpsilon)
        {
            Owner.TransitionTo<PlayerCrouchState>();
        }
        else if (!Input.GetButton("Crouch"))
        {
            if (standingObstructed)
            {
                Owner.TransitionTo<PlayerCrouchState>();
            }
            else
            {
                Owner.TransitionTo<PlayerWalkingState>();
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        ThisCollider.height = StandardColliderHeight;
        ThisCollider.center = Vector3.zero;
    }

    private void SlidingDecelerate()
    {
        RaycastHit groundCheckHit;

        bool grounded = GroundCheck(out groundCheckHit);

        if (grounded)
        {
            Vector3 velocityOnGround = Vector3.ProjectOnPlane(Velocity, groundCheckHit.normal);

            Vector3 decelerationVector = velocityOnGround.normalized * Deceleration * decelerationMultiplier * PlayerDeltaTime;

            if (decelerationVector.magnitude > velocityOnGround.magnitude)
            {
                Velocity = Vector3.zero;
            }
            else
            {
                Velocity -= decelerationVector;
            }
        }
    }

    protected override void UpdatePlayerRotation()
    {
        Transform.LookAt(Transform.position + new Vector3(Velocity.x, 0.0f, Velocity.z).normalized);
    }
}
