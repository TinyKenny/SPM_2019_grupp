using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Walking State")]
public class PlayerWalkingState : PlayerBaseState
{
    [SerializeField] protected float jumpPower = 12.5f;
    
    protected bool jumpAllowed = true;
    protected bool grounded = true;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        MovementInput();

        Shoot();

        Velocity += Vector3.down * Gravity * PlayerDeltaTime; // because slopes are a thing

        CheckCollision(Velocity * PlayerDeltaTime);
        Velocity *= Mathf.Pow(AirResistanceCoefficient, PlayerDeltaTime);

        float soundDistance = (Velocity.magnitude / owner.MaxSpeed) * movementSoundRange;
        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new PlayerSoundEventInfo(owner.gameObject, soundDistance));

        //if speed is high enough for running, and you are in walking state
        if (Velocity.magnitude > (MaxSpeed / 2) && Mathf.Approximately(MaxSpeedMod, 1.0f))
        {
            //you are running, this is relevant because of reasons
        }
        if (!grounded)
        {
            owner.TransitionTo<PlayerAirState>();
        }
        else if(grounded && Input.GetButton("Crouch"))
        {
            if (Velocity.magnitude > (MaxSpeed / 2) && Mathf.Approximately(MaxSpeedMod, 1.0f))
            {
                owner.TransitionTo<PlayerSlideState>();
            }
            else
            {
                owner.TransitionTo<PlayerCrouchState>();
            }
            //Debug.Log("Wat");
        }
    }

    private void MovementInput()
    {
        RaycastHit groundCheckHit;

        grounded = GroundCheck(out groundCheckHit);
        if(grounded)
        {
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
            direction = Camera.main.transform.rotation * direction;
            direction = Vector3.ProjectOnPlane(direction, groundCheckHit.normal).normalized * direction.magnitude;

            if (direction.magnitude < MathHelper.floatEpsilon)
            {
                Decelerate(groundCheckHit);
            }
            else
            {
                Accelerate(groundCheckHit, direction);
            }

            if (Input.GetButtonDown("Jump") && jumpAllowed && Time.timeScale > 0)
            {
                Velocity += Vector3.up * (jumpPower * owner.TimeSlowMultiplier);
            }
        }
    }

    private void Decelerate(RaycastHit groundCheckHit)
    {
        Vector3 velocityOnGround = Vector3.ProjectOnPlane(Velocity, groundCheckHit.normal);

        Vector3 decelerationVector = velocityOnGround.normalized * Deceleration * PlayerDeltaTime;

        if (decelerationVector.magnitude > velocityOnGround.magnitude)
        {
            Velocity = Vector3.zero;
        }
        else
        {
            Velocity -= decelerationVector;
        }
    }

    private void Accelerate(RaycastHit groundCheckHit, Vector3 direction)
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


        if (Velocity.magnitude > MaxSpeed)
        {
            Velocity = Velocity.normalized * MaxSpeed;
        }
    }
}
