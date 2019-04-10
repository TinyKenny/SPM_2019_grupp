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

    private void Awake()
    {
        thisCollider = GetComponent<CapsuleCollider>();
        base.Awake();
    }

    private void Update()
    {
        base.Update();
    }
}
