﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunbotBaseState : State
{
    protected Transform PlayerTransform { get { return owner.playerTransform; } }
    protected float Acceleration { get { return owner.Acceleration; } }
    protected float Deceleration { get { return owner.Deceleration; } }
    protected float MaxSpeed { get { return owner.MaxSpeed; } }
    protected float AirResistanceCoefficient { get { return owner.AirResistanceCoefficient; } }
    protected float SkinWidth { get { return owner.SkinWidth; } }
    protected Vector3 Velocity { get { return owner.Velocity; } set { owner.Velocity = value; } }
    protected SphereCollider ThisCollider { get { return owner.thisCollider; } }
    protected Transform ThisTransform { get { return owner.transform; } }
    protected int CurrentPatrolPointIndex { get { return owner.currentPatrolPointIndex; } set { owner.currentPatrolPointIndex = value; } }

    protected StunbotStateMachine owner;

    public override void Initialize(StateMachine owner)
    {
        this.owner = (StunbotStateMachine)owner;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        ApplyMovement(Velocity * Time.deltaTime);
        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);
    }

    protected bool CanSeePlayer(float alertDistance)
    {
        return !Physics.Linecast(owner.transform.position, PlayerTransform.position, owner.visionMask)
            && Vector3.Distance(owner.transform.position, PlayerTransform.position) < alertDistance;
    }

    protected bool CanSeeOrigin()
    {
        return !Physics.Linecast(owner.transform.position, owner.patrolLocations[0].transform.position, owner.visionMask)
            && Vector3.Distance(owner.transform.position, owner.patrolLocations[0].transform.position) < owner.allowedOriginDistance;
    }

    protected void ApplyMovement(Vector3 movement)
    {
        RaycastHit rayHit;

        bool rayHasHit = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, movement.normalized, out rayHit, Mathf.Infinity, (owner.visionMask | owner.playerLayer | (1 << owner.gameObject.layer)));

        if (rayHasHit)
        {
            Vector3 hitNormal = rayHit.normal;

            float angle = (Vector3.Angle(hitNormal, movement.normalized) - 90) * Mathf.Deg2Rad;
            float snapDistanceFromHit = SkinWidth / Mathf.Sin(angle);

            Vector3 snapMovement = movement.normalized * (rayHit.distance - snapDistanceFromHit);
            snapMovement = Vector3.ClampMagnitude(snapMovement, movement.magnitude);

            movement -= snapMovement;

            ThisTransform.position += snapMovement;

            if (movement.magnitude > 0.01f)
            {
                Vector3 VelocityDelta = Velocity;
                PhysicsComponent otherPC = rayHit.transform.GetComponent<PhysicsComponent>();

                if(otherPC != null)
                {
                    VelocityDelta -= otherPC.velocity;
                }



                //Vector3 reflectDirection = Vector3.Reflect(VelocityDelta)

                Vector3 reflectDirection = Vector3.Reflect(movement.normalized, hitNormal);


                movement = reflectDirection * movement.magnitude;
                Velocity = reflectDirection * Velocity.magnitude;


                ApplyMovement(movement);
            }
        }
        else
        {
            ThisTransform.position += movement;
        }
    }

    protected void FlyToTarget(Vector3 targetPosition)
    {
        #region CopiedFromIdle
        if (Vector3.Distance(targetPosition, ThisTransform.position) > Velocity.magnitude * 0.1f)
        {
            Vector3 targetDirection = targetPosition - ThisTransform.position;

            Quaternion desiredRotation = Quaternion.LookRotation(targetDirection.normalized);
            ThisTransform.rotation = Quaternion.RotateTowards(ThisTransform.rotation, desiredRotation, 90.0f * Time.deltaTime);

            Vector3 accelerationVector = targetDirection.normalized * Acceleration * Time.deltaTime;


            if (Vector3.Dot(ThisTransform.forward, targetDirection.normalized) > 0.75f)
            {
                Velocity = Vector3.ClampMagnitude(Velocity + accelerationVector, MaxSpeed);
                if (Vector3.Dot(Velocity.normalized, targetDirection.normalized) > 0.0f)
                {
                    Vector3 testVector = targetDirection.normalized * Velocity.magnitude;
                    Velocity = Vector3.Lerp(Velocity, testVector, 1.5f * Time.deltaTime);
                }
            }
            else if (Velocity.magnitude > 0.0f)
            {
                Vector3 decelerationvector = Velocity.normalized * Deceleration * Time.deltaTime;

                if (decelerationvector.magnitude > Velocity.magnitude)
                {
                    Velocity = Vector3.zero;
                }
                else
                {
                    Velocity -= decelerationvector;
                }
            }
        }
        #endregion
    }
}
