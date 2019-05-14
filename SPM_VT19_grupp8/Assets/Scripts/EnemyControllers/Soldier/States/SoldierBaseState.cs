using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierBaseState : State
{
    protected SoldierStateMachine owner;
    protected Transform PlayerTransform { get { return owner.PlayerTransform; } }
    protected Transform[] PatrolLocations { get { return owner.PatrolLocations; } }

    private void Awake()
    {
        
    }

    public override void Initialize(StateMachine owner)
    {
        this.owner = (SoldierStateMachine)owner;
    }

    protected bool PlayerVisionCheck(float alertDistance)
    {
        return !(Physics.Linecast(owner.transform.position, PlayerTransform.position, owner.visionMask)) && Vector3.Distance(owner.transform.position, PlayerTransform.position) < alertDistance && Vector3.Dot(owner.transform.forward, (PlayerTransform.position - owner.transform.position)) > 0.3f;
    }
}
