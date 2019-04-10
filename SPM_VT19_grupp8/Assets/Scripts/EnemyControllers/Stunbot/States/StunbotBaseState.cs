using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunbotBaseState : State
{
    protected Transform PlayerTransform { get { return owner.playerTransform; } }
    protected float Acceleration { get { return owner.Acceleration; } }
    protected float Deceleration { get { return owner.Deceleration; } }
    protected float MaxSpeed { get { return owner.MaxSpeed; } }
    protected float AirResistanceCoefficient { get { return owner.AirResistanceCoefficient; } }
    protected float SkinWidth { get { return owner.SkinWidth; } }
    protected Vector3 Velocity { get { return owner.Velocity; } set { owner.Velocity = value; } }
    protected SphereCollider ThisCollider { get { return owner.thisCollider; } }
    protected Transform ThisTransform { get { return owner.transform; } }

    protected StunbotStateMachine owner;

    public override void Initialize(StateMachine owner)
    {
        this.owner = (StunbotStateMachine)owner;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);
    }

    protected bool CanSeePlayer()
    {
        return !Physics.Linecast(owner.transform.position, PlayerTransform.position, owner.visionMask);
    }
}
