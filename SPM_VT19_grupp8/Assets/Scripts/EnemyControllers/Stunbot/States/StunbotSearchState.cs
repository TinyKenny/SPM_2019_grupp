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
    }
    public override void HandleUpdate()
    {
        Vector3 direction = (owner.lastPlayerLocation - ThisTransform.position).normalized * Acceleration * Time.deltaTime;

        owner.faceDirection += direction.normalized * 5.0f * Time.deltaTime;
        if(owner.faceDirection.magnitude > 1.0f)
        {
            owner.faceDirection = owner.faceDirection.normalized;
        }
        ThisTransform.LookAt(ThisTransform.position + owner.faceDirection);

        if (Vector3.Dot(ThisTransform.forward, direction.normalized) > 0.5f)
        {
            Velocity = Vector3.ClampMagnitude(Velocity + direction, MaxSpeed);
        }

        //Vector3 plannedMovement = Velocity * Time.deltaTime;

        //RaycastHit playerRayHit;

        //bool hitPlayer = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, plannedMovement.normalized, out playerRayHit, plannedMovement.magnitude, owner.playerLayer);

        //if (hitPlayer)
        //{

        //}
        Vector3 previousPosition = ThisTransform.position;

        base.HandleUpdate();

        if(searchTimer >= 0.0f)
        {
            searchTimer -= Time.deltaTime;
        }

        if (CanSeePlayer(60.0f))
        {
            Debug.Log("Search -> Chase (found player)");
            owner.TransitionTo<StunbotChaseState>();
        }
        if (Vector3.Distance(owner.lastPlayerLocation, ThisTransform.position) < 1.0f || searchTimer <= 0.0f)
        {
            Debug.Log("Search -> Idle (player not found)");
            owner.TransitionTo<StunbotIdleState>();
        }
        if (!CanSeeOrigin())
        {
            Debug.Log("Search -> Idle (can't see origin)");
            ThisTransform.position = previousPosition;
            owner.TransitionTo<StunbotIdleState>();
        }
    }
}
