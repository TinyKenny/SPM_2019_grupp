using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Idle State")]
public class StunbotIdleState : StunbotBaseState
{
    public override void HandleUpdate()
    {
        Velocity *= 0.5f;

        if (Vector3.Distance(owner.patrolLocations[0].position, ThisTransform.position) > MaxSpeed * 0.1f)
        {
            Vector3 direction = Vector3.ClampMagnitude(owner.patrolLocations[0].position - ThisTransform.position, 1.0f) * Acceleration * Time.deltaTime;

            owner.faceDirection += direction.normalized * 5.0f * Time.deltaTime;
            if (owner.faceDirection.magnitude > 1.0f)
            {
                owner.faceDirection = owner.faceDirection.normalized;
            }
            ThisTransform.LookAt(ThisTransform.position + owner.faceDirection);

            if (Vector3.Dot(ThisTransform.forward, direction.normalized) > 0.5f)
            {
                Velocity += Vector3.ClampMagnitude(Velocity + direction, MaxSpeed);
            }
        }

        base.HandleUpdate();

        if (CanSeePlayer(20.0f) &&
            Vector3.Distance(ThisTransform.position, owner.patrolLocations[0].position) < MaxSpeed * 0.1f)
            //Vector3.Distance(PlayerTransform.position, owner.patrolLocations[0].position) < owner.allowedOriginDistance)
        {
            Debug.Log("Idle -> Chase (player found)");
            owner.TransitionTo<StunbotChaseState>();
        }
    }
}
