using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Idle State")]
public class StunbotIdleState : StunbotBaseState
{
    Vector3 nextTargetPosition;

    public override void Enter()
    {
        FindTarget();
    }

    private void FindTarget()
    {
        nextTargetPosition = owner.transform.position;
        NavBox end = new NavBox();
        Collider[] colls;
        colls = Physics.OverlapBox(owner.patrolLocations[0].position, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, owner.NavLayer); //end
        if (colls.Length > 0)
            end = colls[0].GetComponent<NavBox>();
        BoxCompareNode bcnEnd = new BoxCompareNode(end, null);
        NavBox start = new NavBox();
        colls = Physics.OverlapBox(owner.transform.position, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, owner.NavLayer); //start
        if (colls.Length > 0)
            start = colls[0].GetComponent<NavBox>();
        BoxCompareNode bcnStart = new BoxCompareNode(start, bcnEnd);
        if (start != null && end != null)
            owner.GetComponent<AStarPathfindning>().FindPath(bcnStart, ThisTransform.position, bcnEnd);
    }

    public override void HandleUpdate()
    {
        if (Vector3.Distance(nextTargetPosition, owner.transform.position) < 0.5f && owner.GetComponent<AStarPathfindning>().Paths.Count > 0)
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

        Velocity *= 0.05f * Time.deltaTime;

        if (Vector3.Distance(owner.patrolLocations[0].position, ThisTransform.position) > MaxSpeed * 0.1f)
        {
            Vector3 direction = Vector3.ClampMagnitude(nextTargetPosition - ThisTransform.position, 1.0f) * Acceleration * Time.deltaTime;

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
