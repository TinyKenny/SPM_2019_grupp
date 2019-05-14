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
    

    private new void Awake()
    {
        thisCollider = GetComponent<CapsuleCollider>();
        startPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        base.Awake();
        if (PatrolLocations.Length == 0)
        {
            PatrolLocations = new Transform[1];
            PatrolLocations[0] = transform;
        }
    }

    public override void SetAlerted(Vector3 lastLocation)
    {
        if (!(currentState is SoldierAttackState))
        {
            playerLastLocation = lastLocation;
            TransitionTo<SoldierAlertState>();

        }
          
    }

    private void OnDestroy()
    {
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<EnemySoundEventInfo>(PlayerSoundAlert);
    }
}
