using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierStateMachine : StateMachine
{
    public Transform playerTransform;
    public LayerMask visionMask;
    public LayerMask playerLayer;
    public CapsuleCollider thisCollider;
    public GameObject projectilePrefab;

    public Vector3 startPosition;

    private void Awake()
    {
        thisCollider = GetComponent<CapsuleCollider>();
        startPosition = transform.position;
        base.Awake();
    }

    private void Update()
    {
        base.Update();
    }
}
