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
