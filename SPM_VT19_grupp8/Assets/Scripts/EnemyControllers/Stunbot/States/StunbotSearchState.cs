using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Search State")]
public class StunbotSearchState : StunbotBaseState
{
    private float searchTimer;
    Vector3 nextTargetPosition;
    Vector3 direction;

    public override void Enter()
    {
        base.Enter();
        searchTimer = 30.0f;
        FindTarget();
        direction = Vector3.zero;
    }

    private void FindTarget()
    {
        nextTargetPosition = owner.transform.position;
        NavBox end = new NavBox();
        Collider[] colls;
        colls = Physics.OverlapBox(PlayerTransform.position, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, owner.NavLayer); //end
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

        if (owner.GetComponent<AStarPathfindning>().Paths.Count > 0)
        {
            if (Vector3.Distance(nextTargetPosition, owner.transform.position) < 0.1f)
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

            direction = (nextTargetPosition - ThisTransform.position).normalized * Acceleration * Time.deltaTime;
        }

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
