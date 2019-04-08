using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Wall run State")]
public class PlayerWallRunState : PlayerAirState
{
    private float wallRunAmount = 1.05f;
    private float jumpPower = 10.0f;
    private float wallRunMaxSpeed;

    public override void Enter()
    {
        wallRunMaxSpeed = MaxSpeed / 5;
    }

    public override void HandleUpdate()
    {
        Velocity += Vector3.down * (Gravity/2) * Time.deltaTime;

        CheckCollision(Velocity * Time.deltaTime);

        bool grounded = findCollision(Vector3.down, GroundCheckDistance + SkinWidth);

        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);

        RaycastHit wall = new RaycastHit();

        if (grounded)
        {
            owner.TransitionTo<PlayerWalkingState>();
        }
        else if (WallRun(out wall))
        {
            Velocity = ProjectSpeedOnSurface(wall);

            if (Velocity.magnitude > wallRunMaxSpeed)
            {
                Velocity = Velocity.normalized * wallRunMaxSpeed;
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
        return (Velocity - projection) * wallRunAmount;
    }
}
