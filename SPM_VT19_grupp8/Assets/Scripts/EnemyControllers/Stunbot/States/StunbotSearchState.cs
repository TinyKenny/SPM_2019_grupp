using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Search State")]
public class StunbotSearchState : StunbotBaseState
{
    private float searchTimer;
    Vector3 direction;

    public override void Enter()
    {
        base.Enter();
        searchTimer = 30.0f;
        FindTarget();
        direction = Vector3.zero;
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
            Debug.Log("Search -> Chase (found player)");
            owner.TransitionTo<StunbotChaseState>();
        }
        if (Vector3.Distance(owner.lastPlayerLocation, ThisTransform.position) < 1.0f || searchTimer <= 0.0f)
        {
            Debug.Log("Search -> Idle (player not found)");
            owner.TransitionTo<StunbotIdleState>();
        }
        if (!CanFindOrigin())
        {
            Debug.Log("Search -> Idle (can't see origin)");
            ThisTransform.position = previousPosition;
            owner.TransitionTo<StunbotIdleState>();
        }
    }
}
