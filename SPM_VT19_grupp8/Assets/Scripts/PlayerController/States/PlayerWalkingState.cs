using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Walking State")]
public class PlayerWalkingState : PlayerBaseState
{
    
    protected bool jumpAllowed = true; //replace this with something better
    protected bool grounded = true;
    protected RaycastHit groundCheckHit;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        grounded = GroundCheck(out groundCheckHit);

        MovementInput();

        Shoot();

        Velocity += Vector3.down * Gravity * PlayerDeltaTime;

        CheckCollision(Velocity * PlayerDeltaTime);
        Velocity *= Mathf.Pow(AirResistanceCoefficient, PlayerDeltaTime);

        float soundDistance = (Velocity.magnitude / Owner.MaxSpeed) * MovementSoundRange;
        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new PlayerDiegeticSoundEventInfo(Transform.gameObject, soundDistance));
        
        if (!grounded)
        {
            Owner.TransitionTo<PlayerAirState>();
        }
        else if(Input.GetButton("Crouch"))
        {
            if (Velocity.magnitude > (MaxSpeed / 2) && Mathf.Approximately(MaxSpeedMod, 1.0f))
            {
                Owner.TransitionTo<PlayerSlideState>();
            }
            else
            {
                Owner.TransitionTo<PlayerCrouchState>();
            }
        }
    }

    private void MovementInput()
    {
        if(grounded)
        {
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
            direction = MainCameraController.transform.rotation * direction;
            direction = Vector3.ProjectOnPlane(direction, groundCheckHit.normal).normalized * direction.magnitude;

            if (direction.magnitude < MathHelper.floatEpsilon)
            {
                Decelerate();
            }
            else
            {
                Accelerate(direction);
            }

            if (Input.GetButtonDown("Jump") && jumpAllowed && Time.timeScale > 0)
            {
                Animator.SetTrigger("Jump");
                Velocity += Vector3.up * (JumpPower/* * Owner.TimeSlowMultiplier*/); // replace timwslowmultiplier with gravity reduction
            }
        }
    }

    private void Decelerate()
    {
        Vector3 velocityOnGround = Vector3.ProjectOnPlane(Velocity, groundCheckHit.normal);

        Vector3 decelerationVector = velocityOnGround.normalized * Deceleration * PlayerDeltaTime;

        if (decelerationVector.sqrMagnitude > velocityOnGround.sqrMagnitude)
        {
            Velocity = Vector3.zero;
        }
        else
        {
            Velocity -= decelerationVector;
        }
    }

    private void Accelerate(Vector3 direction)
    {
        Vector3 velocityOnGround = Vector3.ProjectOnPlane(Velocity, groundCheckHit.normal);

        float turnDot = Vector3.Dot(direction.normalized, velocityOnGround.normalized);

        if (velocityOnGround.magnitude > MathHelper.floatEpsilon && turnDot < -0.5f)
        {
            Velocity += Vector3.ClampMagnitude(direction, 1.0f) * TurnSpeedModifier * Acceleration * PlayerDeltaTime;
        }
        else
        {
            Velocity += Vector3.ClampMagnitude(direction, 1.0f) * Acceleration * PlayerDeltaTime;
        }

        if (Velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
        {
            Velocity = Velocity.normalized * MaxSpeed;
        }
    }
}
