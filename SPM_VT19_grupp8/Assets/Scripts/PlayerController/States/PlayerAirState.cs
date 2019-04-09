using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Air State")]
public class PlayerAirState : PlayerBaseState
{
    public override void HandleUpdate()
    {
        RaycastHit wallRunCheck = new RaycastHit();

        Velocity += Vector3.down * Gravity * PlayerDeltaTime;

        CheckCollision(Velocity * PlayerDeltaTime);

        bool grounded = FindCollision(Vector3.down, GroundCheckDistance + SkinWidth);

        Velocity *= Mathf.Pow(AirResistanceCoefficient, PlayerDeltaTime);

        if (grounded)
        {
            owner.TransitionTo<PlayerWalkingState>();
        }
        else if (WallRun(out wallRunCheck))
        {
            LedgeGrabCheck();
            if (Mathf.Abs(Vector3.Angle(Transform.forward, wallRunCheck.normal)) > 160)
                owner.TransitionTo<PlayerVerticalWallRunState>();
            else
            {
                owner.TransitionTo<PlayerWallRunState>();
            }
        }
    }

    protected void LedgeGrabCheck()
    {
        RaycastHit wall;
        bool b = Physics.BoxCast(ThisCollider.bounds.center, Transform.localScale / 2, Transform.forward, out wall, Transform.rotation, SkinWidth * 5);
        if (b && wall.collider.bounds.max.y < ThisCollider.bounds.max.y)
            Debug.Log("Ledge grabbed!");
    }

    protected bool WallRun()
    {
        return FindCollision(Transform.forward, SkinWidth * 2);
    }

    protected bool WallRun(out RaycastHit wall)
    {
        bool wallFound = FindCollision(Transform.forward, out wall, SkinWidth * 2) || FindCollision(Transform.right, out wall, SkinWidth * 2) || FindCollision(-Transform.right, out wall, SkinWidth * 2);
        return wallFound;
    }

    protected override void HandleCollition(Vector3 hitNormal, RaycastHit raycastHit)
    {

    }
}
