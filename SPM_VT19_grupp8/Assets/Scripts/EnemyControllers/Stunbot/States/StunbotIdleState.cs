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
        colls = Physics.OverlapBox(owner.patrolLocations[CurrentPatrolPointIndex].position, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, owner.NavLayer); //end
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
        if (owner.GetComponent<AStarPathfindning>().Paths.Count > 0)
        {
            // this is copied from HandleUpdate()
            float f = 0;
            foreach (KeyValuePair<float, Vector3> pos in owner.GetComponent<AStarPathfindning>().Paths)
            {
                nextTargetPosition = pos.Value;
                f = pos.Key;
                break;
            }

            owner.GetComponent<AStarPathfindning>().Paths.Remove(f);
        }
    }

    public override void HandleUpdate()
    {
        if (Vector3.Distance(nextTargetPosition, owner.transform.position) < Mathf.Max(Velocity.magnitude * 0.1f, 0.1f) && owner.GetComponent<AStarPathfindning>().Paths.Count > 0)
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

            nextTargetPosition = owner.patrolLocations[CurrentPatrolPointIndex].position;

            if(Vector3.Distance(nextTargetPosition, ThisTransform.position) < Mathf.Max(Velocity.magnitude * 0.1f, 0.1f))
            {
                if(owner.patrolLocations.Length == 1)
                {
                    if (Velocity.magnitude > 0.1f)
                    {
                        Velocity = Vector3.zero;
                    }
                }
                else
                {
                    CurrentPatrolPointIndex = (CurrentPatrolPointIndex + 1) % owner.patrolLocations.Length;
                    FindTarget();
                }
            }
        }

        Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);

        if (Vector3.Distance(nextTargetPosition, ThisTransform.position) > Velocity.magnitude *  0.1f)
        {
            Vector3 targetDirection = nextTargetPosition - ThisTransform.position;

            Quaternion desiredRotation = Quaternion.LookRotation(targetDirection.normalized);
            ThisTransform.rotation = Quaternion.RotateTowards(ThisTransform.rotation, desiredRotation, 90.0f * Time.deltaTime);

            Vector3 accelerationVector = targetDirection.normalized * Acceleration * Time.deltaTime;


            if (Vector3.Dot(ThisTransform.forward, targetDirection.normalized) > 0.75f)
            {
                Velocity = Vector3.ClampMagnitude(Velocity + accelerationVector, MaxSpeed);
            }
            else if(Velocity.magnitude > 0.0f)
            {
                Vector3 decelerationvector = Velocity.normalized * Deceleration * Time.deltaTime;

                if(decelerationvector.magnitude > Velocity.magnitude)
                {
                    Velocity = Vector3.zero;
                }
                else
                {
                    Velocity -= decelerationvector;
                }
            }
        }

        base.HandleUpdate();

        if (false && CanSeePlayer(60.0f) &&
            Vector3.Distance(ThisTransform.position, owner.patrolLocations[CurrentPatrolPointIndex].position) < MaxSpeed * 0.1f)
            //Vector3.Distance(PlayerTransform.position, owner.patrolLocations[0].position) < owner.allowedOriginDistance)
        {
            Debug.Log("Idle -> Chase (player found)");
            owner.TransitionTo<StunbotChaseState>();
        }
    }
}
