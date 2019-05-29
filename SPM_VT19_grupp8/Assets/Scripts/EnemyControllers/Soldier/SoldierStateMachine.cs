﻿using System.Collections;
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
    public NavMeshAgent Agent { get; private set; }
    public AudioClip ShootSound { get { return shootSound; } private set { shootSound = value; } }

    [Header("Vision obstruction layers")]
    [SerializeField] private LayerMask visionMask = 0;
    [SerializeField] private GameObject projectilePrefab = null;
    [Header("Shooting rate of fire")]
    [SerializeField] private float fireRate = 2;
    [Header("Random variance max in rate of fire")]
    [SerializeField] private float fireRateCooldownVarianceMax = 0.5f;
    [Header("Lazer shot sound")]
    [SerializeField] private AudioClip shootSound = null;

    public Animator Anim { get; private set; } = null;


    #region boop-values, move these
    public Vector3 BoopVelocity { get { return boopVelocity; } set { boopVelocity = value; } }
    private Vector3 boopVelocity = Vector3.zero;
    public float BoopStrength { get { return boopStrength; } }
    private float boopStrength = 18.5f;
    #endregion

    protected override void Awake()
    {
        Anim = GetComponentInChildren<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        base.Awake();
    }
    
    protected override void Update()
    {
        Anim.SetFloat("Speed", Agent.velocity.magnitude / Agent.speed);
        base.Update();
    }

    public override void SetAlerted(Vector3 lastLocation)
    {
        if (currentState is SoldierAttackState == false)
        {
            LastPlayerLocation = lastLocation;
            TransitionTo<SoldierAlertState>();
        }
    }

    protected override void HitByPlayerAttack(PlayerAttackEventInfo pAEI)
    {
        Vector3 directionFromAttackOrigin = (transform.position - pAEI.Origin).normalized;
        directionFromAttackOrigin = Vector3.Slerp(directionFromAttackOrigin, pAEI.Direction, pAEI.DirectionWeight);

        boopVelocity = Vector3.ClampMagnitude(boopVelocity + directionFromAttackOrigin * boopStrength, boopStrength);

        TransitionTo<SoldierBoopedState>();
    }

    public override void RemoveEnemy()
    {
        TransitionTo<SoldierDeathState>();
    }
}
