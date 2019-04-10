using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierBaseState : State
{
    protected SoldierStateMachine owner;

    private void Awake()
    {
        
    }

    public override void Initialize(StateMachine owner)
    {
        this.owner = (SoldierStateMachine)owner;
    }

    protected bool PlayerVisionCheck(float alertDistance)
    {
        return !(Physics.Linecast(owner.transform.position, owner.playerTransform.position, owner.visionMask)) && Vector3.Distance(owner.transform.position, owner.playerTransform.position) < alertDistance && Vector3.Dot(owner.transform.forward, (owner.playerTransform.position - owner.transform.position)) > 0.3f;
    }
}
