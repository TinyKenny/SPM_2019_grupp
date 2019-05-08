using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Chase State")]
public class StunbotChaseState : StunbotBaseState
{
    Vector3 nextTargetPosition;
    private bool foundPath = false;

    public override void Enter()
    {
        base.Enter();
        FindTarget();
        foundPath = true;
    }

    private void FindTarget()
    {
        nextTargetPosition = owner.transform.position;
        NavBox end = new NavBox();
        Collider[] colls;
        colls = Physics.OverlapBox(PlayerTransform.position, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, owner.NavLayer);
        if (colls.Length > 0)
            end = colls[0].GetComponent<NavBox>();
        BoxCompareNode bcnEnd = new BoxCompareNode(end, null);
        NavBox start = new NavBox();
        colls = Physics.OverlapBox(owner.transform.position, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, owner.NavLayer);
        if (colls.Length > 0)
            start = colls[0].GetComponent<NavBox>();
        BoxCompareNode bcnStart = new BoxCompareNode(start, bcnEnd);
        if (start != null && end != null)
            owner.GetComponent<AStarPathfindning>().FindPath(bcnStart, ThisTransform.position, bcnEnd);
    }

    public override void HandleUpdate()
    {
        //raycasta för att kolla om man behöver räkna ut en ny väg
        RaycastHit hit = new RaycastHit();
        LayerMask lm = owner.playerLayer | owner.EnviromentLayer;
        if ((!foundPath) && Physics.SphereCast(owner.transform.position, owner.thisCollider.radius, (owner.playerTransform.position - ThisTransform.position).normalized, out hit, Mathf.Infinity, lm) && hit.transform.GetComponent<PlayerStateMachine>() == null)
        {
            foundPath = true;
            FindTarget();
        }
        if (Vector3.Distance(nextTargetPosition, owner.transform.position) < 0.1f && owner.GetComponent<AStarPathfindning>().Paths.Count > 0)
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
        else if (owner.GetComponent<AStarPathfindning>().Paths.Count == 0)
        {
            nextTargetPosition = owner.playerTransform.position;
            foundPath = false;
        }


        #region Original
        //Vector3 direction = (nextTargetPosition - ThisTransform.position).normalized * Acceleration * Time.deltaTime;

        //// (start) rotate toward direction
        //owner.faceDirection += direction.normalized * 5.0f * Time.deltaTime;
        //if (owner.faceDirection.magnitude > 1.0f)
        //{
        //    owner.faceDirection = owner.faceDirection.normalized;
        //}
        //ThisTransform.LookAt(ThisTransform.position + owner.faceDirection);
        //// (end) rotate toward direction


        //if (Vector3.Dot(ThisTransform.forward, direction.normalized) > 0.5f)
        //{
        //    Accelerate(direction);
        //}
        #endregion

        FlyToTarget(nextTargetPosition);

        Vector3 plannedMovement = Velocity * Time.deltaTime;

        RaycastHit playerRayHit;

        bool hitPlayer = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, plannedMovement.normalized, out playerRayHit, plannedMovement.magnitude, owner.playerLayer);

        if (hitPlayer)
        {
            PlayerTransform.GetComponent<PlayerStateMachine>().TakeDamage(3.0f);
            //Accelerate(-direction.normalized * MaxSpeed * 2); // replace this with the bounce from base state
            Velocity = Velocity.normalized * MaxSpeed;
        }

        base.HandleUpdate();

        if (!CanSeeOrigin())
        {
            Debug.Log("Chase -> Idle (can't see origin)");
            owner.TransitionTo<StunbotIdleState>();
        }
        else if (!CanSeePlayer(65.0f))
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
