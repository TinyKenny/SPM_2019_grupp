using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunbotStateMachine : StateMachine
{
    public Transform playerTransform;
    public LayerMask visionMask;
    public LayerMask playerLayer;
    public SphereCollider thisCollider;
    public float turningModifier = 1.0f;

    private PhysicsComponent physicsComponent;

    
    public float Acceleration { get { return physicsComponent.acceleration; } }
    public float Deceleration { get { return physicsComponent.deceleration; } }
    public float MaxSpeed { get { return physicsComponent.maxSpeed; } }
    public float AirResistanceCoefficient { get { return physicsComponent.airResistanceCoefficient; } }
    public float SkinWidth { get { return physicsComponent.skinWidth; } }
    public Vector3 Velocity { get { return physicsComponent.velocity; } set { physicsComponent.velocity = value; } }
    

    protected override void Awake()
    {
        base.Awake();
        physicsComponent = GetComponent<PhysicsComponent>();
        thisCollider = GetComponent<SphereCollider>();
    }

    protected override void Update()
    {
        base.Update();
        if (Velocity.magnitude > MathHelper.floatEpsilon)
        {
            transform.LookAt(transform.position + Velocity.normalized);
        }
    }
}
