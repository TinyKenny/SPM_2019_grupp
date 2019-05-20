using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(PhysicsComponent), typeof(AStarPathfindning))]
public class StunbotStateMachine : EnemyStateMachine
{
    #region "chaining" properties
    public Vector3 Velocity { get { return physicsComponent.velocity; } set { physicsComponent.velocity = value; } }
    public float Acceleration { get { return physicsComponent.acceleration; } }
    public float Deceleration { get { return physicsComponent.deceleration; } }
    public float MaxSpeed { get { return physicsComponent.maxSpeed; } }
    public float AirResistanceCoefficient { get { return physicsComponent.airResistanceCoefficient; } }
    public float SkinWidth { get { return physicsComponent.skinWidth; } }
    #endregion

    #region "plain" properties
    public int CurrentPatrolPointIndex { get; set; } // do something about this?
    public Vector3 LastPlayerLocation { get; set; } // do something about this?
    public SphereCollider ThisCollider { get; private set; }
    public AStarPathfindning PathFinder { get; private set; }
    #endregion

    #region properties for getting private variables
    public LayerMask VisionMask { get { return visionMask; } }
    public LayerMask PlayerLayer { get { return playerLayer; } }
    #endregion

    #region serialized private variables
    [SerializeField] private LayerMask visionMask = 0;
    [SerializeField] private LayerMask playerLayer = 0;
    #endregion

    #region non-serialized private variables
    private PhysicsComponent physicsComponent;
    #endregion

    #region readonly values
    public readonly float allowedOriginDistance = 40.0f;
    #endregion

    protected override void Awake()
    {
        physicsComponent = GetComponent<PhysicsComponent>();
        ThisCollider = GetComponent<SphereCollider>();
        PathFinder = GetComponent<AStarPathfindning>();
        CurrentPatrolPointIndex = 0;

        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void HitByPlayerAttack(PlayerAttackEventInfo pAEI)
    {
        base.HitByPlayerAttack(pAEI);

        Vector3 directionFromAttackOrigin = (transform.position - pAEI.Origin).normalized;

        directionFromAttackOrigin = Vector3.Slerp(directionFromAttackOrigin, pAEI.Direction, pAEI.DirectionWeight);

        Velocity = directionFromAttackOrigin;
        TransitionTo<StunbotBoopedState>();
    }
}
