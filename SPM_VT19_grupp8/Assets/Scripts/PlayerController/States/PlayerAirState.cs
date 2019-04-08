﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Air State")]
public class PlayerAirState : PlayerBaseState
{
    public override void HandleUpdate()
    {
        Velocity += Vector3.down * Gravity * Time.deltaTime;

        CheckCollision(Velocity * Time.deltaTime);

        bool grounded = findCollision(Vector3.down, GroundCheckDistance + SkinWidth);

        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);

        if (grounded)
        {
            owner.TransitionTo<PlayerWalkingState>();
        }
        else if (WallRun())
        {
            owner.TransitionTo<PlayerWallRunState>();
        }
    }

    protected bool WallRun()
    {
        return findCollision(Vector3.right, SkinWidth * 2) || findCollision(Vector3.left, SkinWidth * 2);
    }

    protected bool WallRun(out RaycastHit wall)
    {
        return findCollision(Vector3.right, out wall, SkinWidth * 2) || findCollision(Vector3.left, out wall, SkinWidth * 2);
    }
}
