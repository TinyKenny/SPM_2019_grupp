using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Chase State")]
public class StunbotChaseState : StunbotBaseState
{
    Vector3 nextTargetPosition;

    public override void Enter()
    {
        nextTargetPosition = owner.transform.position;
        NavBox end = Physics.OverlapBox(PlayerTransform.position, new Vector3(0.01f, 0.01f, 0.01f), Quaternion.identity, 1 << 14)[0].GetComponent<NavBox>();
        BoxCompareNode bcnEnd = new BoxCompareNode(end, null);
        NavBox start = Physics.OverlapBox(owner.transform.position, new Vector3(0.01f, 0.01f, 0.01f), Quaternion.identity, 1 << 14)[0].GetComponent<NavBox>();
        BoxCompareNode bcnStart = new BoxCompareNode(start, bcnEnd);
        owner.GetComponent<AStarPathfindning>().FindPath(bcnStart, ThisTransform.position, bcnEnd);
    }

    public override void HandleUpdate()
    {
        if (Vector3.Distance(nextTargetPosition, owner.transform.position) < 1)
        {
            float f = 0;
            foreach (KeyValuePair<float, Vector3> pos in owner.GetComponent<AStarPathfindning>().Paths)
            {
                nextTargetPosition = pos.Value;
                f = pos.Key;
                break;
            }

            owner.GetComponent<AStarPathfindning>().Paths.Remove(f);
        }

        Vector3 direction = (nextTargetPosition - ThisTransform.position).normalized * Acceleration * Time.deltaTime;

        // (start) rotate toward direction
        owner.faceDirection += direction.normalized * 5.0f * Time.deltaTime;
        if (owner.faceDirection.magnitude > 1.0f)
        {
            owner.faceDirection = owner.faceDirection.normalized;
        }
        ThisTransform.LookAt(ThisTransform.position + owner.faceDirection);
        // (end) rotate toward direction


        if (Vector3.Dot(ThisTransform.forward, direction.normalized) > 0.5f)
        {
            Accelerate(direction);
        }
        
        Vector3 plannedMovement = Velocity * Time.deltaTime;

        RaycastHit playerRayHit;

        bool hitPlayer = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, plannedMovement.normalized, out playerRayHit, plannedMovement.magnitude, owner.playerLayer);

        if (hitPlayer)
        {
            PlayerTransform.GetComponent<PlayerStateMachine>().TakeDamage(3.0f);
            Accelerate(-direction.normalized * MaxSpeed * 2);
        }

        base.HandleUpdate();

        if (!CanSeeOrigin())
        {
            Debug.Log("Chase -> Idle (can't see origin)");
            owner.TransitionTo<StunbotIdleState>();
        }
        else if (!CanSeePlayer(21.0f))
        {
            Debug.Log("Chase -> Search (lost player)");
            owner.TransitionTo<StunbotSearchState>();
        }
        else
        {
            owner.lastPlayerLocation = PlayerTransform.position;
        }
    }

    private void Accelerate(Vector3 accelerationVector)
    {
        /*
        Vector3 newVelocity = Velocity + accelerationVector;

        if(newVelocity.magnitude > MaxSpeed)
        {
            float magPercent = Velocity.magnitude / newVelocity.magnitude;
            Velocity *= magPercent;
        }

        Velocity += accelerationVector;
        */
        Velocity = Vector3.ClampMagnitude(Velocity + accelerationVector, MaxSpeed);
    }
}
