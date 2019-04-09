using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Walking State")]
public class PlayerWalkingState : PlayerBaseState
{
    [SerializeField] protected float jumpPower = 10.0f;
    
    protected bool jumpAllowed = true;
    protected bool grounded = true;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
    }

    public override void HandleUpdate()
    {
        MovementInput();

        Velocity += Vector3.down * Gravity * Time.deltaTime; // because slopes are a thing

        CheckCollision(Velocity * Time.deltaTime);
        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);

        //if speed is high enough for running state
        if(Velocity.magnitude > (MaxSpeed / 2) && Mathf.Approximately(MaxSpeedMod, 1.0f))
        {
            //you are running, this is relevant because of reasons
        }
        if(grounded && Input.GetButton("Crouch"))
        {
            owner.TransitionTo<PlayerCrouchState>();
        }
    }

    private void MovementInput()
    {
        RaycastHit groundCheckHit;

        grounded = GroundCheck(out groundCheckHit);

        if (!grounded)
        {
            owner.TransitionTo<PlayerAirState>();
        }
        else
        {
            if (Input.GetButtonDown("Jump") && jumpAllowed)
            {
                Velocity += Vector3.up * jumpPower;
            }

            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));

            direction = Camera.main.transform.rotation * direction;
            direction = Vector3.ProjectOnPlane(direction, groundCheckHit.normal).normalized * direction.magnitude;
            Accelerate(direction);
        }
    }

    private void Accelerate(Vector3 direction)
    {
        Velocity += Vector3.ClampMagnitude(direction, 1.0f) * Acceleration * Time.deltaTime;
        if (Velocity.magnitude > MaxSpeed * MaxSpeedMod)
        {
            Velocity = Velocity.normalized * MaxSpeed * MaxSpeedMod;
        }
    }
}
