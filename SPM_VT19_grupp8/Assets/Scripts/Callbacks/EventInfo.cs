using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for alll types of events or <see cref="EventInfo"/>. All events needs to use an EventInfo class that inherits from this class.
/// </summary>
public abstract class EventInfo
{
    public readonly GameObject GO;
    public string EventDescription;

    public EventInfo(GameObject gO, string description)
    {
        GO = gO;
        EventDescription = description;
    }
}

/// <summary>
/// <see cref="EventInfo"/> class for when a player respawns. Used to reset all objects in levels and the player.
/// </summary>
public class PlayerRespawnEventInfo : EventInfo
{

    public PlayerRespawnEventInfo(GameObject gO, string description = "Player has died") : base(gO, description)
    {

    }
}

/// <summary>
/// <see cref="EventInfo"/> class for when a player makes diegetic sounds. All objects that should be able to hear the player like enemies can listen to these events.
/// </summary>
public class PlayerDiegeticSoundEventInfo : EventInfo
{
    public float Range;
    public AudioClip AC;

    /// <summary>
    /// Constructor for diegetic sound events.
    /// </summary>
    /// <param name="gO">The player gameobject that is the source of the sound.</param>
    /// <param name="range">The range of how far away the sound can be heard.</param>
    /// <param name="ac">The Audioclip to be played.</param>
    /// <param name="description">Optional field</param>
    public PlayerDiegeticSoundEventInfo(GameObject gO, float range, AudioClip ac = null, string description = "Player sound event") : base(gO, description)
    {
        Range = range;
        AC = ac;
    }
}

/// <summary>
/// <see cref="EventInfo"/> class for when an ammopickup is picked up. Triggers events related to ammopickups like update player ammo.
/// </summary>
public class AmmoPickupEventInfo : EventInfo
{
    public int AmmoAmount { get; private set; }

    /// <summary>
    /// Constructor for ammo pickup events.
    /// </summary>
    /// <param name="gO">The gameobject/player that picked up the ammo.</param>
    /// <param name="amount">How much ammo should be added.</param>
    /// <param name="description">Optional field</param>
    public AmmoPickupEventInfo(GameObject gO, int amount, string description = "Player picked up ammo") : base(gO, description)
    {
        AmmoAmount = amount;
    }
}

/// <summary>
/// <see cref="EventInfo"/> class for when the player attacks. Every enemy should have a listener for this event type.
/// </summary>
public class PlayerAttackEventInfo : EventInfo
{
    public Vector3 Origin { get; private set; }
    public Vector3 Direction { get; private set; }
    public float Angle { get; private set; }
    public float Range { get; private set; }

    /// <summary>
    /// Constructor for player attack events.
    /// </summary>
    /// <param name="gO">The gameobject of the player.</param>
    /// <param name="origin">The origin point of the attack, typically the position of the player</param>
    /// <param name="direction">The "forward" direction of the attack.</param>
    /// <param name="angle">
    ///     How aligned an object must be with <see cref="Direction"/> in order to be hit.
    ///     Ranges from 0 to 1, where:
    ///         0 means the attack only hits objects directly in front of it
    ///         0.5 means the attack hits objects in a half-sphere in front of it
    ///         1 means it hits objects in a full sphere around it.
    /// </param>
    /// <param name="range">The maximum distance at which the attack can hit an object.</param>
    /// <param name="description">Optional description field.</param>
    public PlayerAttackEventInfo(GameObject gO, Vector3 origin, Vector3 direction, float angle, float range, string description = "Player attacked") : base(gO, description)
    {
        Origin = origin;
        Direction = direction;
        Angle = Mathf.Lerp(1.0f, -1.0f, angle);
        Range = range;
    }
}

public class EnemySaveEventInfo : EventInfo
{
    public EnemySaveEventInfo(GameObject gO, string description = "Save current state of enemies") : base(gO, description)
    {
    }
}
