using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Air State")]
public class PlayerAirState : PlayerBaseState
{
    public override void HandleUpdate()
    {
        RaycastHit wallRunCheck = new RaycastHit();

        Velocity += Vector3.down * Gravity * Time.deltaTime;

        CheckCollision(Velocity * Time.deltaTime);

        bool grounded = FindCollision(Vector3.down, GroundCheckDistance + SkinWidth);

        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);

        if (grounded)
        {
            owner.TransitionTo<PlayerWalkingState>();
        }
        else if (WallRun(out wallRunCheck))
        {
            if (Velocity.x < 0.3f && Velocity.z < 0.3f)
                owner.TransitionTo<PlayerVerticalWallRunState>();
            else
            {
                owner.TransitionTo<PlayerWallRunState>();
            }
        }
    }

    protected bool WallRun()
    {
        return FindCollision(Transform.right, SkinWidth * 2) || FindCollision(-Transform.right, SkinWidth * 2);
    }

    protected bool WallRun(out RaycastHit wall)
    {
        return FindCollision(Transform.right, out wall, SkinWidth * 2) || FindCollision(-Transform.right, out wall, SkinWidth * 2);
    }
}
