using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Chase State")]
public class StunbotChaseState : StunbotBaseState
{
    private bool foundPath = false;

    public override void Enter()
    {
        base.Enter();
        FindTarget();
        foundPath = true;
    }

    public override void HandleUpdate()
    {
        //raycasta för att kolla om man behöver räkna ut en ny väg
        RaycastHit hit = new RaycastHit();
        if ((!foundPath) && Physics.SphereCast(ThisTransform.position, ThisCollider.radius, (PlayerTransform.position - ThisTransform.position).normalized, out hit, Mathf.Infinity, VisionMask))
        {
            foundPath = true;
            FindTarget();
        }

        FlyToTarget(NextTargetPosition);

        Vector3 plannedMovement = Velocity * Time.deltaTime;

        RaycastHit playerRayHit;

        bool hitPlayer = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, plannedMovement.normalized, out playerRayHit, plannedMovement.magnitude, PlayerLayer);

        if (hitPlayer)
        {
            PlayerTransform.GetComponent<PlayerStateMachine>().TakeDamage(3.0f);
            Velocity = Velocity.normalized * MaxSpeed;
        }

        base.HandleUpdate();

        if (!CanFindOrigin())
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

    protected override void NoTargetAvailable()
    {
        NextTargetPosition = PlayerTransform.position;
        foundPath = false;
    }
}
