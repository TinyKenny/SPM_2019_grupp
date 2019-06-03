﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Booped State")]
public class SoldierBoopedState : SoldierBaseState
{
    [SerializeField, Min(0.0f)] private float gravity = 15.0f;
    [SerializeField, Min(0.0f)] private float deceleration = 20;

    private CapsuleCollider thisCollider;
    private bool grounded;
    private RaycastHit groundCheckHit;
    private float skinwidth = 0.02f;
    private float groundCheckDistance = 0.01f;
    private float BoopStrength { get { return Owner.BoopStrength; } }
    private Vector3 Velocity { get { return Owner.BoopVelocity; } set { Owner.BoopVelocity = value; } }

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        thisCollider = owner.GetComponent<CapsuleCollider>();
    }

    public override void Enter()
    {
        base.Enter();
        Agent.enabled = false;
        PlayerLastLocation = PlayerPosition;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        Vector3 topPoint = Owner.transform.position + thisCollider.center + Owner.transform.up * (thisCollider.height / 2 - thisCollider.radius);
        Vector3 bottomPoint = Owner.transform.position + thisCollider.center - Owner.transform.up * (thisCollider.height / 2 - thisCollider.radius);
        if (Physics.CapsuleCast(topPoint, bottomPoint, thisCollider.radius, Vector3.down, out groundCheckHit, skinwidth + groundCheckDistance, VisionMask, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Dot(groundCheckHit.normal, Vector3.up) > 0.8f && Mathf.Abs(Vector3.Dot(groundCheckHit.normal, Velocity.normalized)) < 0.1f)
            {
                grounded = true;
                Decelerate();
            }
        }

        Velocity += Vector3.down * gravity * Time.deltaTime;

        CheckCollision(Velocity * Time.deltaTime);



        if (grounded && Velocity.sqrMagnitude < Agent.speed * Agent.speed * MathHelper.FloatEpsilon)
        {
            if (PlayerVisionCheck(80))
            {
                Owner.TransitionTo<SoldierChaseState>();
            }
            else
            {
                Owner.TransitionTo<SoldierAlertState>();
            }
        }
    }

    public override void Exit()
    {
        Velocity = Vector3.zero;
        Agent.enabled = true;
        base.Exit();
    }

    private void Decelerate()
    {
        Vector3 decelerationVector = Velocity.normalized * deceleration * Time.deltaTime;

        if (decelerationVector.sqrMagnitude > Velocity.sqrMagnitude)
        {
            Velocity = Vector3.zero;
        }
        else
        {
            Velocity -= decelerationVector;
        }
    }

    private void CheckCollision(Vector3 movement)
    {
        Vector3 topPoint = Owner.transform.position + thisCollider.center + Owner.transform.up * (thisCollider.height / 2 - thisCollider.radius);
        Vector3 bottomPoint = Owner.transform.position + thisCollider.center - Owner.transform.up * (thisCollider.height / 2 - thisCollider.radius);

        bool castHasHit = Physics.CapsuleCast(topPoint, bottomPoint, thisCollider.radius, movement.normalized, out RaycastHit raycastHit, Mathf.Infinity, VisionMask, QueryTriggerInteraction.Ignore);

        if (castHasHit)
        {
            Vector3 hitNormal = raycastHit.normal;

            float angle = (Vector3.Angle(hitNormal, movement.normalized) - 90) * Mathf.Deg2Rad;
            float snapDistanceFromHit = skinwidth / Mathf.Sin(angle);

            Vector3 snapMovement = movement.normalized * (raycastHit.distance - snapDistanceFromHit);
            snapMovement = Vector3.ClampMagnitude(snapMovement, movement.magnitude);

            movement -= snapMovement;

            Owner.transform.position += snapMovement;

            if (movement.sqrMagnitude > MathHelper.FloatEpsilon * MathHelper.FloatEpsilon)
            {
                if (hitNormal.y < 0.5f || Vector3.Project(Velocity, hitNormal).sqrMagnitude > BoopStrength * BoopStrength / 2)
                {
                    // bounce
                    Vector3 reflectDirection = Vector3.Reflect(Velocity.normalized, hitNormal);

                    movement = reflectDirection * movement.magnitude;
                    Velocity = reflectDirection * Velocity.magnitude * 0.8f;
                }
                else
                {
                    // dont bounce
                    Vector3 hitNormalMovement = MathHelper.NormalForce(movement, hitNormal);
                    movement += hitNormalMovement;

                    Vector3 hitNormalForce = MathHelper.NormalForce(Velocity, hitNormal);
                    Velocity += hitNormalForce;
                    
                }

                CheckCollision(movement);
            }
        }
        else
        {
            Owner.transform.position += movement;
        }
    }

}
