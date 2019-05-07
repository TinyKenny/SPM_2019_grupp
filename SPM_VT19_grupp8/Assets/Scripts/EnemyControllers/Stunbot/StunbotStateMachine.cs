using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunbotStateMachine : EnemyStateMachine
{
    //public Transform playerTransform;
    public LayerMask visionMask;
    public LayerMask playerLayer;
    public SphereCollider thisCollider;
    public float turningModifier = 1.0f;
    //public Transform[] patrolLocations;
    public int currentPatrolPointIndex;
    public Vector3 lastPlayerLocation;

    private PhysicsComponent physicsComponent;
    [HideInInspector]
    public Vector3 faceDirection;
    [HideInInspector]
    public readonly float allowedOriginDistance = 400.0f;

    public float Acceleration { get { return physicsComponent.acceleration; } }
    public float Deceleration { get { return physicsComponent.deceleration; } }
    public float MaxSpeed { get { return physicsComponent.maxSpeed; } }
    public float AirResistanceCoefficient { get { return physicsComponent.airResistanceCoefficient; } }
    public float SkinWidth { get { return physicsComponent.skinWidth; } }
    public Vector3 Velocity { get { return physicsComponent.velocity; } set { physicsComponent.velocity = value; } }
    public LayerMask NavLayer;
    public LayerMask EnviromentLayer;
    

    protected override void Awake()
    {
        base.Awake();
        physicsComponent = GetComponent<PhysicsComponent>();
        thisCollider = GetComponent<SphereCollider>();
        faceDirection = transform.forward;
        currentPatrolPointIndex = 0;
        if(patrolLocations.Length == 0)
        {
            patrolLocations = new Transform[1];
            patrolLocations[0] = transform;
        }
    }

    protected override void Update()
    {
        base.Update();

        /*
        if (Velocity.magnitude > MathHelper.floatEpsilon)
        {
            faceDirection += Velocity.normalized * Time.deltaTime * 5.0f;
            if(faceDirection.magnitude > 1)
            {
                faceDirection = faceDirection.normalized;
            }


            transform.LookAt(transform.position + faceDirection);
        }
        */
    }
}
