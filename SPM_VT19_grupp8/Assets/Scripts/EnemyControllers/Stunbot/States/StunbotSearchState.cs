using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Search State")]
public class StunbotSearchState : StunbotBaseState
{
    private float searchTimer;

    public override void Enter()
    {
        base.Enter();
        searchTimer = 30.0f;
        FindTarget();
    }

    public override void HandleUpdate()
    {
        FlyToTarget(NextTargetPosition);

        Vector3 previousPosition = ThisTransform.position;

        base.HandleUpdate();

        if(searchTimer >= 0.0f)
        {
            searchTimer -= Time.deltaTime;
        }

        if (CanSeePlayer(55.0f))
        {
            Owner.TransitionTo<StunbotChaseState>();
        }
        if ((LastPlayerLocation - ThisTransform.position).sqrMagnitude < 1.0f || searchTimer <= 0.0f || !CanFindOrigin())
        {
            Owner.TransitionTo<StunbotIdleState>();
        }
    }
}
