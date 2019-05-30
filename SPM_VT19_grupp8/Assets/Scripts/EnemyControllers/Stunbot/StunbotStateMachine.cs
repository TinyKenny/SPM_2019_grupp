using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(PhysicsComponent))]
public class StunbotStateMachine : EnemyStateMachine
{
    #region "chaining" properties
    public Vector3 Velocity { get { return physicsComponent.velocity; } set { physicsComponent.velocity = value; } }
    public float Deceleration { get { return physicsComponent.deceleration; } }
    public float Speed { get { return physicsComponent.maxSpeed; } }
    public float SkinWidth { get { return physicsComponent.skinWidth; } }
    #endregion

    #region "plain" properties
    public SphereCollider ThisCollider { get; private set; }
    public Path CurrentPath { get; set; }
    public bool FollowingPath { get; set; }
    public Transform Target { get; set; }
    #endregion

    #region properties for getting private variables
    public LayerMask VisionMask { get { return visionMask; } }
    public LayerMask PlayerLayer { get { return playerLayer; } }
    public AudioClip ShockSound { get { return shockSound; } private set { shockSound = value; } }
    public float BoopStrength { get { return boopStrength; } }
    #endregion

    #region serialized private variables
    [SerializeField] private LayerMask visionMask = 0;
    [SerializeField] private LayerMask playerLayer = 0;
    [Header("Attack sound")]
    [SerializeField] private AudioClip shockSound = null;
    [Header("Boop-power against this stunbot")]
    [SerializeField, Min(0.0f)] private float boopStrength = 18.5f;
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
        CurrentPatrolPointIndex = 0;

        base.Awake();
    }

    protected override void HitByPlayerAttack(PlayerAttackEventInfo pAEI)
    {
        base.HitByPlayerAttack(pAEI);

        Vector3 directionFromAttackOrigin = (transform.position - pAEI.Origin).normalized;

        directionFromAttackOrigin = Vector3.Slerp(directionFromAttackOrigin, pAEI.Direction, pAEI.DirectionWeight);

        Velocity = directionFromAttackOrigin;
        TransitionTo<StunbotBoopedState>();
    }

    private void OnDrawGizmos()
    {
        if (FollowingPath)
        {
            Vector3 previousPosition = transform.position;
            foreach(Vector3 point in CurrentPath.lookPoints)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(point, 0.35f);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(previousPosition, point);
                previousPosition = point;
            }
        }
    }
}
