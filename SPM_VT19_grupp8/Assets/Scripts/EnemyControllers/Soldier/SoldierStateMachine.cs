using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierStateMachine : EnemyStateMachine
{
    public LayerMask visionMask;
    public LayerMask playerLayer;
    public CapsuleCollider thisCollider;
    public GameObject projectilePrefab;

    public Vector3 startPosition;
    public NavMeshAgent agent;
    public float currentCoolDown = 0;
    public float maxCoolDown = 2;
    public float cooldownVarianceMax = 0.5f;
    public Vector3 playerLastLocation;

    public AudioSource ausSoldier;
    public AudioClip soldierAlertSound;

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

    public void SetAlerted(Vector3 lastLocation)
    {
        ausSoldier.PlayOneShot(soldierAlertSound);  
        if (!(currentState is SoldierAttackState))
        {
            playerLastLocation = lastLocation;
            TransitionTo<SoldierAlertState>();

        }
          
    }
}
