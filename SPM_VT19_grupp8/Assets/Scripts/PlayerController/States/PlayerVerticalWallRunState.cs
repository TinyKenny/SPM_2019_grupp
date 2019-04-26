using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Vertical Wall Run State")]
public class PlayerVerticalWallRunState : PlayerAirState
{
    private float jumpPower = 15.0f;
    private float maxVerticalVelocity = 10f;
    private Vector3 wallNormal;
    private float verticalSpeedMultiplier = 2;

    public override void Enter()
    {
        RaycastHit wall = new RaycastHit();

        WallRun(out wall);

        wallNormal = wall.normal;

        if(Velocity.y < 0)
            Velocity = new Vector3(Velocity.x, 0f, Velocity.z);

        Velocity = ProjectSpeedOnSurface();
    }

    public override void HandleUpdate()
    {
        Velocity += Vector3.down * Gravity * PlayerDeltaTime;

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
            LedgeGrabCheck();
            //Velocity += Vector3.RotateTowards(MathHelper.NormalForce(Velocity, wallNormal), Vector3.up, 1, 1) * (Acceleration/5) /*Vector3.ClampMagnitude(new Vector3(0, Velocity.y, 0).normalized, 1.0f) * (Acceleration / 2)*/ * PlayerDeltaTime;


            if (Velocity.magnitude > maxVerticalVelocity)
            {
                Velocity = Velocity.normalized * maxVerticalVelocity;
            }

            if (Input.GetButtonDown("Jump"))
            {
                Velocity += (wallNormal * 2 + Vector3.up).normalized * jumpPower;
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
        float magnitude = projection.magnitude * verticalSpeedMultiplier;
        return new Vector3(Velocity.x, Velocity.y + magnitude, Velocity.z) - projection;
    }
}
