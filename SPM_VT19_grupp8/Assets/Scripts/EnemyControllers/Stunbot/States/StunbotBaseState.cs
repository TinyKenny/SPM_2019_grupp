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
    protected Transform Target { get { return Owner.Target; } set { Owner.Target = value; } }
    protected Transform ThisTransform { get { return Owner.transform; } }
    protected LayerMask VisionMask { get { return Owner.VisionMask; } }
    protected Transform PlayerTransform { get { return Owner.PlayerTransform; } }
    protected Transform[] PatrolLocations { get { return Owner.PatrolLocations; } }
    protected SphereCollider ThisCollider { get { return Owner.ThisCollider; } }
    protected float Speed { get { return Owner.Speed; } }

    private Path CurrentPath { get { return Owner.CurrentPath; } set { Owner.CurrentPath = value; } }
    private float AllowedOriginDistance { get { return Owner.allowedOriginDistance; } }
    private bool FollowingPath { get { return Owner.FollowingPath; } set { Owner.FollowingPath = value; } }
    private float StoppingDistance { get { return stoppingDistance * stoppingDistanceModifier; } }

    protected float stoppingDistanceModifier = 1.0f;

    private int pathIndex; // value changes in methods
    private float speedPercent; // value changes in methods
    private float timeUntillNextRequest = 0.0f; // value changes in methods
    private Vector3 targetPositionOld; // value changes in methods

    private const float pathUpdateMoveThreshold = 0.75f;
    private const float stoppingDistance = 2.0f;
    private const float turnSpeed = 5.0f;
    private const float requestCooldown = 0.4f;
    private const float turnDst = 1.0f;

    protected StunbotStateMachine Owner { get; private set; }

    public override void Initialize(StateMachine owner)
    {
        Owner = (StunbotStateMachine)owner;
    }

    public override void Enter()
    {
        base.Enter();
        FollowingPath = false;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        UpdatePath();
        FollowPath();
    }

    /// <summary>
    /// If the current target is the stunbot itself, the stunbot is not following a path or the current target has moved far enough since the current path was found, requests a new path to the target.
    /// Also calls <see cref="UpdateTarget"/> if the current target is the stunbot itself or if the stunbot is not following a path.
    /// </summary>
    private void UpdatePath()
    {
        if (Time.timeSinceLevelLoad > 0.3f)
        {
            timeUntillNextRequest -= Time.deltaTime;
            if (timeUntillNextRequest <= 0.0f && (FollowingPath == false || Target == ThisTransform || (Target.position - targetPositionOld).sqrMagnitude > pathUpdateMoveThreshold * pathUpdateMoveThreshold))
            {
                if (FollowingPath == false || Target == ThisTransform)
                {
                    UpdateTarget();
                }
                RequestPath();
            }
        }
    }

    /// <summary>
    /// Adds a request, to the <see cref="PathRequestManager"/>, for a path to the current target.
    /// </summary>
    private void RequestPath()
    {
        PathRequestManager.RequestPath(ThisTransform.position, Target.position, ThisCollider.radius, OnPathFound);
        targetPositionOld = Target.position;
        timeUntillNextRequest = requestCooldown;
    }

    /// <summary>
    /// Used as a callback for the <see cref="PathRequestManager"/>, when it has fullfilled the path request.
    /// If the pathfinding was successful, this method creates a new path-object, representing the path, and sets the variables related to following a path.
    /// </summary>
    /// <param name="waypoints">The points, in world space, that the found path passes through.</param>
    /// <param name="pathSuccess">Whether or not the pathfinding was successful.</param>
    private void OnPathFound(Vector3[] waypoints, bool pathSuccess)
    {
        if (pathSuccess)
        {
            CurrentPath = new Path(waypoints, ThisTransform.position, turnDst, StoppingDistance);
            FollowingPath = true;
            pathIndex = 0;
            speedPercent = 1.0f;
        }
    }

    /// <summary>
    /// If the stunbot has a path to follow, this method moves the stunbot along that path.
    /// </summary>
    private void FollowPath()
    {
        if (FollowingPath)
        {
            while (CurrentPath.turnBoundaries[pathIndex].HasCrossedLine(ThisTransform.position))
            {
                if (pathIndex == CurrentPath.finishLineIndex)
                {
                    FollowingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (FollowingPath)
            {
                if (pathIndex >= CurrentPath.slowDownIndex && StoppingDistance > 0)
                {
                    speedPercent = Mathf.Clamp01(0.1f + CurrentPath.turnBoundaries[CurrentPath.finishLineIndex].DistanceFromPoint(ThisTransform.position) / StoppingDistance);
                    if (speedPercent < 0.05f)
                    {
                        FollowingPath = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(CurrentPath.lookPoints[pathIndex] - ThisTransform.position);
                ThisTransform.rotation = Quaternion.Lerp(ThisTransform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                ThisTransform.Translate(Vector3.forward * Time.deltaTime * Speed * speedPercent, Space.Self);
            }
        }
    }

    /// <summary>
    /// Method to be overridden in subclasses.
    /// Is called each time that the stunbot reaches its target.
    /// </summary>
    protected virtual void UpdateTarget() { }

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
}
