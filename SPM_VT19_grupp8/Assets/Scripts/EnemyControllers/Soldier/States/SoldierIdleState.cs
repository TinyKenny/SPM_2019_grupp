using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Idle State")]
public class SoldierIdleState : SoldierBaseState
{
    public override void Enter()
    {
        owner.agent.SetDestination(owner.startPosition);
    }

    public override void HandleUpdate()
    {
        //if (Vector3.Distance(owner.transform.position, owner.startPosition) > MathHelper.floatEpsilon)
        //    owner.transform.position += 10f * (owner.startPosition - owner.transform.position).normalized * Time.deltaTime;
            

        if (PlayerVisioCheck(30))
        {
            owner.TransitionTo<SoldierChaseState>();
        }
    }
}
