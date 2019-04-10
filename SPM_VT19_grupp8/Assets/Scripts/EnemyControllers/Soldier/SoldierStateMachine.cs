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

    private void Awake()
    {
        thisCollider = GetComponent<CapsuleCollider>();
        startPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        base.Awake();
    }

    private void Update()
    {
        base.Update();
    }
}
