using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Wall run State")]
public class PlayerWallRunState : PlayerAirState
{
    private float jumpPower = 12.5f;
    private float maxVerticalVelocity = 1.5f;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        MaxSpeedMod = 1.2f;
    }

    public override void Enter()
    {
    }

    public override void HandleUpdate()
    {
        Velocity += Vector3.down * (Gravity / 2) * PlayerDeltaTime;

        CheckCollision(Velocity * PlayerDeltaTime);

        bool grounded = GroundCheck();

        Velocity *= Mathf.Pow(AirResistanceCoefficient, PlayerDeltaTime);

        RaycastHit wall = new RaycastHit();

        if (grounded)
        {
            owner.TransitionTo<PlayerWalkingState>();
        }
        else if (WallRun(out wall))
        {
            Velocity = ProjectSpeedOnSurface(wall);

            Velocity += Vector3.ClampMagnitude(new Vector3(Velocity.x, 0, Velocity.z).normalized, 1.0f) * (Acceleration * 4) * PlayerDeltaTime;

            if (Velocity.y > maxVerticalVelocity)
                Velocity = new Vector3(Velocity.x, maxVerticalVelocity, Velocity.z);

            if (Velocity.magnitude > MaxSpeed)
            {
                Velocity = Velocity.normalized * MaxSpeed;
            }

            if (Input.GetButtonDown("Jump"))
            {
                Velocity += (wall.normal + Vector3.up).normalized * jumpPower;
            }
        }
        else
        {
            owner.TransitionTo<PlayerAirState>();
        }
    }

    private Vector3 ProjectSpeedOnSurface(RaycastHit wall)
    {
        Vector3 projection = Vector3.Dot(Velocity, wall.normal) * wall.normal;
        return Velocity - projection;
    }
}
