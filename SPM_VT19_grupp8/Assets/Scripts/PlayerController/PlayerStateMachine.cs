using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public LayerMask collisionLayers;
    public PhysicsComponent physicsComponent;
    public CapsuleCollider thisCollider;
    public float skinWidth = 0.01f;
    public float groundCheckDistance = 0.01f;


    public float Acceleration { get { return physicsComponent.acceleration; } set { physicsComponent.acceleration = value; } }
    public float Deceleration { get { return physicsComponent.deceleration; } set { physicsComponent.deceleration = value; } }
    public float MaxSpeed { get { return physicsComponent.maxSpeed; } set { physicsComponent.maxSpeed = value; } }
    public float FrictionCoefficient { get { return physicsComponent.frictionCoefficient; } set { physicsComponent.frictionCoefficient = value; } }
    public float AirResistanceCoefficient { get { return physicsComponent.airResistanceCoefficient; } set { physicsComponent.airResistanceCoefficient = value; } }
    public float Gravity { get { return physicsComponent.gravity; } set { physicsComponent.gravity = value; } }
    public Vector3 Velocity { get { return physicsComponent.velocity; } set { physicsComponent.velocity = value; } }


    protected override void Awake()
    {
        base.Awake();
        physicsComponent = GetComponent<PhysicsComponent>();
        thisCollider = GetComponent<CapsuleCollider>();
    }

    protected override void Update()
    {
        base.Update();
        UpdatePlayerRotation();
    }

    /// <summary>
    /// Rotates the player-GameObject so that its Z-axis (also known as "forward", example: transform.forward)
    /// to be pointing in the direction of Velocity.
    /// </summary>
    private void UpdatePlayerRotation()
    {
        // make this better later
        transform.LookAt(transform.position + new Vector3(Velocity.x, 0.0f, Velocity.z).normalized);
    }
}
