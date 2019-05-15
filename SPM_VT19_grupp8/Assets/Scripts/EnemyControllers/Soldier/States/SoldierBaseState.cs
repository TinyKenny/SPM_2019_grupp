using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Base state for all guard or soldier type of enemies. Used as a superclass for all soldier states and holds all variables as properties that are needed between states.
/// </summary>
public class SoldierBaseState : State
{
    protected SoldierStateMachine owner { get; private set; }
    protected Transform PlayerTransform { get { return owner.PlayerTransform; } }
    protected Transform[] PatrolLocations { get { return owner.PatrolLocations; } }
    protected LayerMask VisionMask { get { return owner.VisionMask; } }
    protected Vector3 PlayerLastLocation { get { return owner.PlayerLastLocation; } set { owner.PlayerLastLocation = value; } }
    protected NavMeshAgent Agent { get { return owner.Agent; } }
    protected Vector3 Position { get { return owner.transform.position; } set { owner.transform.position = value; } }
    protected Vector3 PlayerPosition { get { return owner.PlayerTransform.position; } }

    public override void Initialize(StateMachine owner)
    {
        this.owner = (SoldierStateMachine)owner;
    }

    /// <summary>
    /// Checks if the player can be seen within a set range.
    /// </summary>
    /// <param name="alertDistance">How far the soldier should be able to see.</param>
    /// <returns>True if it could see the player within range, false if it couldnt see the player within range.</returns>
    protected bool PlayerVisionCheck(float alertDistance)
    {
        return Physics.Linecast(Position, PlayerPosition, VisionMask) == false && Vector3.Distance(Position, PlayerPosition) < alertDistance && Vector3.Dot(owner.transform.forward, (PlayerPosition - Position)) > 0.3f;
    }
}
