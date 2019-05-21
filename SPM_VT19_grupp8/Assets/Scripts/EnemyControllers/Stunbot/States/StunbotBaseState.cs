using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base state for the stunbot.
/// </summary>
public class StunbotBaseState : State
{
    protected int CurrentPatrolPointIndex { get { return Owner.CurrentPatrolPointIndex; } set { Owner.CurrentPatrolPointIndex = value; } }
    protected Vector3 Velocity { get { return Owner.Velocity; } set { Owner.Velocity = value; } }
    protected Vector3 LastPlayerLocation { get { return Owner.LastPlayerLocation; } set { Owner.LastPlayerLocation = value; } }
    protected Transform ThisTransform { get { return Owner.transform; } }
    protected SphereCollider ThisCollider { get { return Owner.ThisCollider; } }
    protected LayerMask VisionMask { get { return Owner.VisionMask; } }
    protected LayerMask PlayerLayer { get { return Owner.PlayerLayer; } }
    protected Transform PlayerTransform { get { return Owner.PlayerTransform; } }
    protected Transform[] PatrolLocations { get { return Owner.PatrolLocations; } }
    protected float AllowedOriginDistance { get { return Owner.allowedOriginDistance; } }
    protected float Acceleration { get { return Owner.Acceleration; } }
    protected float Deceleration { get { return Owner.Deceleration; } }
    protected float MaxSpeed { get { return Owner.MaxSpeed; } }
    protected float AirResistanceCoefficient { get { return Owner.AirResistanceCoefficient; } }
    protected float SkinWidth { get { return Owner.SkinWidth; } }


    protected bool hasPath { get; set; } // use this to make sure the entire path is followed?
    protected List<Vector3> Paths { get; private set; } = new List<Vector3>();

    #region pathfollowtest
    private Path path; // needs value
    private bool followingPath = false; // needs value
    private int pathIndex = 0; // needs value
    private float speedPercent; // needs value
    private float timeUntillNextRequest = 0.0f;
    private Vector3 targetPositionOld;
    private float pathUpdateMoveThreshold = 0.75f;

    protected Transform Target { get { return Owner.Target; } set { Owner.Target = value; } }

    protected float stoppingDst = 2.0f; // might need a different value
    private float turnSpeed = 3.0f; // might need a different value
    private float requestCooldown = 1.0f; // might need a different value
    private float turnDst = 5.0f; // might need a different value
    #endregion


    protected StunbotStateMachine Owner { get; private set; }

    public override void Initialize(StateMachine owner)
    {
        Owner = (StunbotStateMachine)owner;
        hasPath = false;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        if(Time.timeSinceLevelLoad > 0.3f)
        {
            timeUntillNextRequest -= Time.deltaTime;
            if(timeUntillNextRequest <= 0.0f && (Target == ThisTransform || (Target.position - targetPositionOld).sqrMagnitude > pathUpdateMoveThreshold * pathUpdateMoveThreshold))
            {
                if(Target == ThisTransform)
                {
                    UpdateTarget();
                }
                RequestPath();
            }
        }

        if(followingPath)
        {
            while (path.turnBoundaries[pathIndex].HasCrossedLine(ThisTransform.position))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(ThisTransform.position) / stoppingDst);
                    if (speedPercent < 0.05f)
                    {
                        followingPath = false;
                    }
                }
                
                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - ThisTransform.position);
                ThisTransform.rotation = Quaternion.Lerp(ThisTransform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                ThisTransform.Translate(Vector3.forward * Time.deltaTime * MaxSpeed * speedPercent, Space.Self);
                
            }
        }
        if(followingPath == false)
        {
            UpdateTarget();
            RequestPath();
        }



        //ApplyMovement(Velocity * Time.deltaTime);
        //Velocity *= Mathf.Pow(AirResistanceCoefficient, Time.deltaTime);

        //if (Paths.Count > 0)
        //{
        //    #region debugging

        //    Color pathDrawColor = new Color32(255, 165, 0, 255);
        //    Vector3 lastPosition = ThisTransform.position;

        //    foreach(Vector3 pos in Paths)
        //    {
        //        Debug.DrawLine(lastPosition, pos, pathDrawColor);
        //        lastPosition = pos;
        //    }

        //    #endregion

        //    if (Vector3.Distance(Paths[0], ThisTransform.position) < Mathf.Max(Velocity.magnitude * 0.1f, 0.1f))
        //    {
        //        Paths.RemoveAt(0);
        //    }
        //}
        //else if (Paths.Count == 0)
        //{
        //    NoTargetAvailable();
        //}
    }

    private void RequestPath()
    {
        PathRequestManager.RequestPath(ThisTransform.position, Target.position, OnPathFound);
        targetPositionOld = Target.position;
        timeUntillNextRequest = requestCooldown;
    }

    private void OnPathFound(Vector3[] waypoints, bool pathSuccess)
    {
        if (pathSuccess)
        {
            path = new Path(waypoints, ThisTransform.position, turnDst, stoppingDst);
            followingPath = true;
            pathIndex = 0;
            speedPercent = 1.0f;
        }
    }

    protected virtual void UpdateTarget()
    {

    }

    /// <summary>
    /// Checks if the player is close enough to the stunbot, and the stunbots patrolpoint, for the stunbot to be able to see the player.
    /// Also checks if there is anything obstructing the stunbots view of the player.
    /// </summary>
    /// <param name="alertDistance">The maximum distance at which the stunbot can see the player</param>
    /// <returns>True if the player is within the specified ranges of the specified positions, and the stunbots view of the player is not obstructed.</returns>
    protected bool CanSeePlayer(float alertDistance)
    {
        return (ThisTransform.position - PlayerTransform.position).sqrMagnitude < alertDistance * alertDistance
            && (PlayerTransform.position - PatrolLocations[CurrentPatrolPointIndex].position).sqrMagnitude < AllowedOriginDistance *AllowedOriginDistance
            && !Physics.Linecast(ThisTransform.position, PlayerTransform.position, VisionMask);
    }

    /// <summary>
    /// Checks if the stunbots position is within <see cref="StunbotStateMachine.allowedOriginDistance"/> units range of its current patrol points position.
    /// </summary>
    /// <returns>Whether the stunbot is within the specified range or not</returns>
    protected bool CanFindOrigin()
    {
        return (ThisTransform.position - PatrolLocations[CurrentPatrolPointIndex].position).sqrMagnitude < AllowedOriginDistance * AllowedOriginDistance;
    }

    /// <summary>
    /// Attempts to apply the specified movement to the stunbot, while obeying the physics-simulation rules of the stunbot.
    /// The physics-simulation rules of the stunbot is that if it collides with anything during its movement, it will bounce off of that object.
    /// </summary>
    /// <param name="movement">The desired movement</param>
    protected void ApplyMovement(Vector3 movement)
    {
        RaycastHit rayHit;

        bool rayHasHit = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, movement.normalized, out rayHit, Mathf.Infinity, (VisionMask | PlayerLayer | (1 << ThisTransform.gameObject.layer)));

        if (rayHasHit)
        {
            Vector3 hitNormal = rayHit.normal;

            float angle = (Vector3.Angle(hitNormal, movement.normalized) - 90) * Mathf.Deg2Rad;
            float snapDistanceFromHit = SkinWidth / Mathf.Sin(angle);

            Vector3 snapMovement = movement.normalized * (rayHit.distance - snapDistanceFromHit);
            snapMovement = Vector3.ClampMagnitude(snapMovement, movement.magnitude);

            movement -= snapMovement;

            ThisTransform.position += snapMovement;

            if (movement.magnitude > 0.01f)
            {
                Vector3 reflectDirection = Vector3.Reflect(Velocity.normalized, hitNormal);

                movement = reflectDirection * movement.magnitude;
                Velocity = reflectDirection * Velocity.magnitude;

                StunbotStateMachine otherStunBot = rayHit.transform.GetComponent<StunbotStateMachine>();

                if (otherStunBot != null)
                {
                    Vector3 otherReflectDirection = Vector3.Reflect(otherStunBot.Velocity.normalized, -hitNormal);
                    otherStunBot.Velocity = otherReflectDirection * otherStunBot.Velocity.magnitude;
                }
                ApplyMovement(movement);
            }
        }
        else
        {
            ThisTransform.position += movement;
        }
    }

    /// <summary>
    /// Rotates the stunbot towards facing the target position.
    /// If the stunbot is close enough to facing the target position, it will accelerate linearly, and "turn", towards the target position.
    /// if the stunbot is not close enough to facing the target position, it will decelerate linearly.
    /// </summary>
    /// <param name="targetPosition">The position to fly to</param>
    protected void FlyToTarget(Vector3 targetPosition)
    {
        if (Vector3.Distance(targetPosition, ThisTransform.position) > Velocity.magnitude * 0.1f) // the distance between the target and stunbot is far enough for movement to be worth it
        {
            Vector3 targetDirection = (targetPosition - ThisTransform.position).normalized;

            Quaternion desiredRotation = Quaternion.LookRotation(targetDirection);
            ThisTransform.rotation = Quaternion.RotateTowards(ThisTransform.rotation, desiredRotation, 90.0f * Time.deltaTime);

            if (Vector3.Dot(ThisTransform.forward, targetDirection) > 0.75f) // stunbot is close enough to facing the target position
            {
                AccelerateInDirection(targetDirection);
            }
            else if (Velocity.magnitude > 0.0f)
            {
                Decelerate();
            }
        }
    }

    /// <summary>
    /// "Turns" and linearly accelerates towards the specified direction.
    /// </summary>
    /// <param name="direction">The desired movement direction</param>
    private void AccelerateInDirection(Vector3 direction)
    {
        Vector3 accelerationVector = direction * Acceleration * Time.deltaTime;

        Velocity = Vector3.ClampMagnitude(Velocity + accelerationVector, MaxSpeed);
        if (Vector3.Dot(Velocity.normalized, direction) > 0.0f)
        {
            Vector3 lerpTargetVector = direction * Velocity.magnitude;
            Velocity = Vector3.Slerp(Velocity, lerpTargetVector, 3.0f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Linearly decelerates, untill the magnitude of Velocity zero.
    /// </summary>
    protected void Decelerate()
    {
        Vector3 decelerationvector = Velocity.normalized * Deceleration * Time.deltaTime;

        if (decelerationvector.magnitude > Velocity.magnitude)
        {
            Velocity = Vector3.zero;
        }
        else
        {
            Velocity -= decelerationvector;
        }
    }

    /// <summary>
    /// Uses A* pathfinding to find the "best" path to the position of the player character.
    /// If no working path was found, the stunbot will fly straight towards the position of the player character.
    /// </summary>
    protected virtual void FindTarget(Vector3 target)
    {
        // pathfinding doesnt go all the way, missing final "step"?
        // stunbot skips the final point of the path

        #region in development
        //bool cantFlyDirectly;
        //RaycastHit raycastHit;

        //cantFlyDirectly = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, (target - ThisTransform.position).normalized, out raycastHit, VisionMask);
        #endregion


        //Paths = AStarPathfindning.FindPath(ThisTransform.position, target);

        if (Paths == null)
        {
            Debug.Log("no path found!");
            Paths = new List<Vector3>();
            Paths.Add(target);
        }
    }

    protected virtual void NoTargetAvailable()
    {

    }
}
