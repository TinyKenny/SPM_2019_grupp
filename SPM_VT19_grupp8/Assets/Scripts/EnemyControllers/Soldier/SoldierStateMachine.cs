using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierStateMachine : StateMachine
{
    public Transform playerTransform;
    public LayerMask visionMask;
    public LayerMask playerLayer;
    public CapsuleCollider thisCollider;
    public GameObject projectilePrefab;

    public Vector3 startPosition;
    public NavMeshAgent agent;
    public float currentCoolDown = 0;
    public float maxCoolDown = 2;
    public Vector3 playerLastLocation;
    public Transform[] patrolLocations;

    private void Awake()
    {
        thisCollider = GetComponent<CapsuleCollider>();
        startPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        base.Awake();
        if (patrolLocations.Length == 0)
        {
            patrolLocations = new Transform[1];
            patrolLocations[0] = transform;
        }
    }

    private void Update()
    {
        base.Update();
    }
}
