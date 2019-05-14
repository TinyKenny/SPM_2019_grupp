using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunbotStateMachine : EnemyStateMachine
{
    public LayerMask visionMask;
    public LayerMask playerLayer;
    public SphereCollider thisCollider;
    public float turningModifier = 1.0f;
    public int currentPatrolPointIndex;
    public Vector3 lastPlayerLocation;

    private PhysicsComponent physicsComponent;
    [HideInInspector]
    public Vector3 faceDirection;
    [HideInInspector]
    public readonly float allowedOriginDistance = 40.0f;

    public float Acceleration { get { return physicsComponent.acceleration; } }
    public float Deceleration { get { return physicsComponent.deceleration; } }
    public float MaxSpeed { get { return physicsComponent.maxSpeed; } }
    public float AirResistanceCoefficient { get { return physicsComponent.airResistanceCoefficient; } }
    public float SkinWidth { get { return physicsComponent.skinWidth; } }
    public Vector3 Velocity { get { return physicsComponent.velocity; } set { physicsComponent.velocity = value; } }
    public LayerMask NavLayer;
    public LayerMask EnviromentLayer;
    [HideInInspector]
    public AStarPathfindning PathFinder;
    

    protected override void Awake()
    {
        physicsComponent = GetComponent<PhysicsComponent>();
        thisCollider = GetComponent<SphereCollider>();
        PathFinder = GetComponent<AStarPathfindning>();
        faceDirection = transform.forward;
        currentPatrolPointIndex = 0;
        if(PatrolLocations.Length == 0)
        {
            PatrolLocations = new Transform[1];
            PatrolLocations[0] = transform;
        }
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }
}
