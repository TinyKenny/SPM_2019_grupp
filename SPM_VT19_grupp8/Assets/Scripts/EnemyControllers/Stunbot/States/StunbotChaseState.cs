using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Chase State")]
public class StunbotChaseState : StunbotBaseState
{
    private bool foundPath;

    public override void Enter()
    {
        base.Enter();
        if (PlayerTransform != null)
        {
            FindTarget(PlayerTransform.position);
            foundPath = true;
        }
        else
        {
            FindTarget(ThisTransform.position);
            foundPath = false;
        }
    }

    public override void HandleUpdate()
    {
        //raycasta för att kolla om man behöver räkna ut en ny väg
        if (PlayerTransform != null)
        {
            RaycastHit hit = new RaycastHit();
            if ((!foundPath) && Physics.SphereCast(ThisTransform.position, ThisCollider.radius, (PlayerTransform.position - ThisTransform.position).normalized, out hit, Mathf.Infinity, VisionMask))
            {
                Debug.Log(foundPath);
                foundPath = true;
                FindTarget(PlayerTransform.position);
            }
        }

        if (Paths.Count > 0)
        {
            FlyToTarget(Paths[0]);
        }

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
        if (PlayerTransform != null)
        {
            FindTarget(PlayerTransform.position);
        }
        else
        {
            FindTarget(ThisTransform.position);
        }
    }
}
