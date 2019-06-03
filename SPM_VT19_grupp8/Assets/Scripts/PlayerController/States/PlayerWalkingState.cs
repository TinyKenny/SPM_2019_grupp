using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Walking State")]
public class PlayerWalkingState : PlayerBaseState
{
    protected bool grounded = true;
    protected RaycastHit groundCheckHit;
    private float groundToAirGracePeriod = 0.2f;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
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
            Owner.Invoke("TransitToAirState", groundToAirGracePeriod);
        }
    }

    private void MovementInput()
    {
        if(grounded)
        {
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
            direction = MainCameraController.transform.rotation * direction;
            direction = Vector3.ProjectOnPlane(direction, groundCheckHit.normal).normalized * direction.magnitude;

            if (direction.magnitude < MathHelper.FloatEpsilon)
            {
                Decelerate();
            }
            else
            {
                Accelerate(direction);
            }
        }

        if ((Input.GetKeyDown(PrimaryJumpKey) || Input.GetKeyDown(SecondaryJumpKey)) && Time.timeScale > 0)
        {
            Jump();
        }
    }

    private void Jump()
    {
        Owner.PlayJumpSound();
        Animator.SetTrigger("Jump");
        Velocity = Vector3.ProjectOnPlane(Velocity, Transform.up);
        Velocity += Vector3.up * JumpPower;
        Owner.TransitionTo<PlayerAirState>();
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

        Velocity += Vector3.ClampMagnitude(direction, 1.0f) * Acceleration * PlayerDeltaTime;
        Velocity = Vector3.Lerp(Velocity, direction.normalized * Velocity.magnitude, TurnSpeedModifier * PlayerDeltaTime);

        if (Velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
        {
            Velocity = Velocity.normalized * MaxSpeed;
        }
    }
}
