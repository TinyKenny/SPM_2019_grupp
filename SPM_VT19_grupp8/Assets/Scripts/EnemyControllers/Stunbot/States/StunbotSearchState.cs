using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Search State")]
public class StunbotSearchState : StunbotBaseState
{
    private float searchTimer;
    private Transform lastPlayerLocationTransform;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        lastPlayerLocationTransform = new GameObject().transform;
    }

    public override void Enter()
    {
        base.Enter();
        searchTimer = 10.0f;
        lastPlayerLocationTransform.position = LastPlayerLocation;
        Target = lastPlayerLocationTransform;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        if(searchTimer >= 0.0f)
        {
            searchTimer -= Time.deltaTime;
        }

        if (CanSeePlayer(55.0f))
        {
            Owner.TransitionTo<StunbotChaseState>();
        }
        if ((lastPlayerLocationTransform.position - ThisTransform.position).sqrMagnitude < 1.0f || searchTimer <= 0.0f || !CanFindOrigin())
        {
            Owner.TransitionTo<StunbotIdleState>();
        }
    }
}
