using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Chase State")]
public class StunbotChaseState : StunbotBaseState
{
    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        StoppingDistanceModifier = 0.0f;
    }

    public override void Enter()
    {
        base.Enter();
        if (PlayerTransform != null)
        {
            Target = PlayerTransform;
        }
        else
        {
            Target = ThisTransform;
        }
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        

        if((ThisTransform.position - PlayerTransform.position).sqrMagnitude <= (ThisCollider.radius + Speed * Time.deltaTime) * (ThisCollider.radius + Speed * Time.deltaTime) * 1.01f)
        {
            Owner.PlaySound(Owner.ShockSound);
            PlayerDamageEventInfo pDEI = new PlayerDamageEventInfo(ThisTransform.gameObject, 3.0f);
            EventCoordinator.CurrentEventCoordinator.ActivateEvent(pDEI);
            Velocity = (ThisTransform.position - PlayerTransform.position).normalized;
            Owner.TransitionTo<StunbotBoopedState>();
        }
        else if (!CanFindOrigin())
        {
            Owner.TransitionTo<StunbotIdleState>();
        }
        else if (!CanSeePlayer(65.0f))
        {
            Owner.TransitionTo<StunbotSearchState>();
        }
        else
        {
            LastPlayerLocation = PlayerTransform.position;
        }
    }
}
