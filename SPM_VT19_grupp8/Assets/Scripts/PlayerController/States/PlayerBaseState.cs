using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBaseState : State
{
    protected PlayerStateMachine owner;


    public float Acceleration { get { return owner.Acceleration; } set { owner.Acceleration = value; } }
    public float Deceleration { get { return owner.Deceleration; } set { owner.Deceleration = value; } }
    public float MaxSpeed { get { return owner.MaxSpeed; } set { owner.MaxSpeed = value; } }
    public float FrictionCoefficient { get { return owner.FrictionCoefficient; } set { owner.FrictionCoefficient = value; } }
    public float AirResistanceCoefficient { get { return owner.AirResistanceCoefficient; } set { owner.AirResistanceCoefficient = value; } }
    public float Gravity { get { return owner.Gravity; } set { owner.Gravity = value; } }
    
    public Vector3 Velocity { get { return owner.Velocity; } set { owner.Velocity = value; } }

    protected Transform Transform { get { return owner.transform; } }
    protected LayerMask CollisionLayers { get { return owner.collisionLayers; } }
    protected CapsuleCollider ThisCollider { get { return owner.thisCollider; } }
    protected float SkinWidth { get { return owner.skinWidth; } }
    protected float GroundCheckDistance { get { return owner.groundCheckDistance; } }


    public override void Initialize(StateMachine owner)
    {

        this.owner = (PlayerStateMachine)owner;
    }

    protected bool findCollision(Vector3 direction, float maxDistance)
    {
        RaycastHit raycastHit;
        return findCollision(direction, out raycastHit, maxDistance);
    }

    protected bool findCollision(Vector3 direction, out RaycastHit raycastHit, float maxDistance)
    {
        Vector3 topPoint = Transform.position + ThisCollider.center + Vector3.up * (ThisCollider.height / 2 - ThisCollider.radius);
        Vector3 bottomPoint = Transform.position + ThisCollider.center + Vector3.down * (ThisCollider.height / 2 - ThisCollider.radius);

        return Physics.CapsuleCast(topPoint, bottomPoint, ThisCollider.radius, direction.normalized, out raycastHit, maxDistance, CollisionLayers, QueryTriggerInteraction.Ignore);
    }

    protected bool GroundCheck()
    {
        RaycastHit raycastHit;
        return GroundCheck(out raycastHit);
    }

    protected bool GroundCheck(out RaycastHit raycastHit)
    {
        return findCollision(Vector3.down, out raycastHit, GroundCheckDistance + SkinWidth);
    }

    protected void CheckCollision(Vector3 movement)
    {
        RaycastHit raycastHit;

        bool castHasHit = findCollision(movement.normalized, out raycastHit, Mathf.Infinity);

        if (castHasHit)
        {
            Vector3 hitNormal = raycastHit.normal;

            float angle = (Vector3.Angle(hitNormal, movement.normalized) - 90) * Mathf.Deg2Rad;
            float snapDistanceFromHit = SkinWidth / Mathf.Sin(angle);

            Vector3 snapMovement = movement.normalized * (raycastHit.distance - snapDistanceFromHit);
            snapMovement = Vector3.ClampMagnitude(snapMovement, movement.magnitude);

            movement -= snapMovement;

            Vector3 hitNormalMovement = MathHelper.NormalForce(movement, hitNormal);
            movement += hitNormalMovement;

            if (hitNormalMovement.magnitude > MathHelper.floatEpsilon)
            {
                Vector3 hitNormalForce = MathHelper.NormalForce(Velocity, hitNormal);
                PhysicsComponent otherPhysicsComponent = raycastHit.collider.GetComponent<PhysicsComponent>();

                Velocity += hitNormalForce;
                CalculateFriction(hitNormalForce.magnitude, otherPhysicsComponent);
            }

            Transform.position += snapMovement;

            if (movement.magnitude > MathHelper.floatEpsilon)
            {
                CheckCollision(movement);
            }
        }

        else if (movement.magnitude > MathHelper.floatEpsilon)
        {
            castHasHit = findCollision(Vector3.down, out raycastHit, SkinWidth + GroundCheckDistance);

            if (castHasHit)
            {
                PhysicsComponent otherPhysicsComponent = raycastHit.collider.GetComponent<PhysicsComponent>();
                CalculateFriction(Gravity * Time.deltaTime, otherPhysicsComponent);
            }
            Transform.position += movement;
        }
    }

    protected void CalculateFriction(float normalForceMagnitude, PhysicsComponent otherPhysicsComponent)
    {
        float realFrictionCoefficient = FrictionCoefficient;
        Vector3 velocityDelta = Velocity;

        if (otherPhysicsComponent != null)
        {
            realFrictionCoefficient = (realFrictionCoefficient + otherPhysicsComponent.frictionCoefficient) / 2;
            velocityDelta -= otherPhysicsComponent.velocity;
        }

        float frictionForceMagnitude = realFrictionCoefficient * normalForceMagnitude;

        if (velocityDelta.magnitude < frictionForceMagnitude)
        {
            if (otherPhysicsComponent != null)
            {
                Velocity = otherPhysicsComponent.velocity;
            }
            else
            {
                Velocity = Vector3.zero;
            }
        }
        else
        {
            frictionForceMagnitude *= 0.7f;
            Vector3 frictionForce = velocityDelta.normalized * frictionForceMagnitude;
            Velocity -= frictionForce;
        }
    }
}
