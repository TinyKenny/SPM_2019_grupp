﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Wall run State")]
public class PlayerWallRunState : PlayerAirState
{
    private float maxVerticalVelocity = 10f;
    private Vector3 wallNormal;
    private float wallRunMultiplier = 2f;
    private float wallRunCooldown = 0.5f;
    private float currentCooldown;

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

        Transform.LookAt(Transform.position + new Vector3(Velocity.x, 0.0f, Velocity.z).normalized);

        currentCooldown = wallRunCooldown;
    }

    public override void HandleUpdate()
    {
        if (Velocity.y > maxVerticalVelocity)
            Velocity = new Vector3(Velocity.x, maxVerticalVelocity, Velocity.z);


        if (Velocity.magnitude > MaxSpeed)
        {
            Velocity = Velocity.normalized * MaxSpeed;
        }

        Velocity += Vector3.down * (Gravity / 2) * PlayerDeltaTime;

        CheckCollision(Velocity * PlayerDeltaTime);

        bool grounded = GroundCheck();

        Velocity *= Mathf.Pow(AirResistanceCoefficient, PlayerDeltaTime) * 2;

        RaycastHit wall = new RaycastHit();

        Jump(wallNormal);

        if (grounded)
        {
            TransitionToWalkingState();
        }
        else if (WallRun(out wall) && Velocity.y > MinimumYVelocity && Input.GetButton("Wallrun"))
        {
            //Velocity += Vector3.ClampMagnitude(new Vector3(Velocity.x, 0, Velocity.z).normalized, 1.0f) * Acceleration * PlayerDeltaTime;
            currentCooldown = wallRunCooldown;
        }
        else
        {
            if (currentCooldown < 0)
                owner.TransitionTo<PlayerAirState>();
            currentCooldown -= owner.getPlayerDeltaTime();
        }
    }

    private Vector3 ProjectSpeedOnSurface()
    {
        Vector3 projection = Vector3.Dot(Velocity, wallNormal) * wallNormal;
        Vector3 tempVelocity = Velocity - projection;
        Vector3 magnitude = projection.magnitude * tempVelocity.normalized;
        return ((tempVelocity + magnitude) *wallRunMultiplier);
    }
}
