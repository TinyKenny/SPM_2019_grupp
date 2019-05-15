using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class for a soldier or guard type of enemy that will shoot at the player when within range, chase when withing enough distance and go alert when it notices the player but cant see. If it does not have an enemy it will patrol.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]
public class SoldierStateMachine : EnemyStateMachine
{
    public LayerMask VisionMask { get { return visionMask; } }
    public float FireRate { get { return fireRate; } }
    public float FireRateCooldownVarianceMax { get { return fireRateCooldownVarianceMax; } }
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }
    public Vector3 PlayerLastLocation { get; set; }
    public NavMeshAgent Agent { get; private set; }

    [Header("Vision obstruction layers")]
    [SerializeField] private LayerMask visionMask = 0;
    [SerializeField] private GameObject projectilePrefab = null;
    [Header("Shooting rate of fire")]
    [SerializeField] private float fireRate = 2;
    [Header("Random variance max in rate of fire")]
    [SerializeField] private float fireRateCooldownVarianceMax = 0.5f;

    protected override void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        base.Awake();
    }

    public override void SetAlerted(Vector3 lastLocation)
    {
        if (currentState is SoldierAttackState == false)
        {
            PlayerLastLocation = lastLocation;
            TransitionTo<SoldierAlertState>();
        }
          
    }
}
