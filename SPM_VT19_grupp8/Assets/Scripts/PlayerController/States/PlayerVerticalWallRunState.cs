using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Vertical Wall Run State")]
public class PlayerVerticalWallRunState : PlayerAirState
{
    private float jumpPower = 10.0f;
    private float maxVerticalVelocity = 10f;

    public override void Enter()
    {
    }

    public override void HandleUpdate()
    {
        Velocity += Vector3.down * Gravity * Time.deltaTime;

        CheckCollision(Velocity * Time.deltaTime);

        bool grounded = GroundCheck();

        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);

        RaycastHit wall = new RaycastHit();

        if (grounded)
        {
            owner.TransitionTo<PlayerWalkingState>();
        }
        else if (WallRun(out wall))
        {
            Velocity += Vector3.ClampMagnitude(new Vector3(0, Velocity.y, 0).normalized, 1.0f) * (Acceleration / 2) * Time.deltaTime;

            if (Velocity.magnitude > maxVerticalVelocity)
            {
                Velocity = Velocity.normalized * maxVerticalVelocity;
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
}
