using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Idle State")]
public class StunbotIdleState : StunbotBaseState
{
    // Start is called before the first frame update

    // Update is called once per frame
    public override void HandleUpdate()
    {
        base.HandleUpdate();
        Velocity *= 0.8f;
        if (CanSeePlayer(20.0f))
        {
            Debug.Log("Start chasing player");
            owner.TransitionTo<StunbotChaseState>();
        }
    }
}
