using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Wall run State")]
public class PlayerWallRunState : PlayerAirState
{
    private float jumpPower = 12.5f;
    private float maxVerticalVelocity = 1.5f;
    private Vector3 wallNormal;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        MaxSpeedMod = 1f;
    }

    public override void Enter()
    {
        RaycastHit wall = new RaycastHit();

        WallRun(out wall);

        wallNormal = wall.normal;

        Velocity = ProjectSpeedOnSurface();
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
        else if (WallRun(out wall) && Velocity.y > MinimumYVelocity && Input.GetButton("Wallrun"))
        {
            //Velocity += Vector3.ClampMagnitude(new Vector3(Velocity.x, 0, Velocity.z).normalized, 1.0f) * Acceleration * PlayerDeltaTime;

            if (Velocity.y > maxVerticalVelocity)
                Velocity = new Vector3(Velocity.x, maxVerticalVelocity, Velocity.z);

            if (Velocity.magnitude > MaxSpeed)
            {
                Velocity = Velocity.normalized * MaxSpeed;
            }

            if (Input.GetButtonDown("Jump"))
            {
                Velocity += (wallNormal + Vector3.up).normalized * jumpPower;
            }
        }
        else
        {
            owner.TransitionTo<PlayerAirState>();
        }
    }

    private Vector3 ProjectSpeedOnSurface()
    {
        Vector3 projection = Vector3.Dot(Velocity, wallNormal) * wallNormal;
        return Velocity - projection;
    }
}
