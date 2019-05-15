using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class PlayerRespawnEventInfo : EventInfo
{

    public PlayerRespawnEventInfo(GameObject gO, string description = "Player has died") : base(gO, description)
    {

    }
}

public class PlayerSoundEventInfo : EventInfo
{
    public float Range;
    public AudioClip AC;

    public PlayerSoundEventInfo(GameObject gO, float range, AudioClip ac = null, string description = "Player sound event") : base(gO, description)
    {
        Range = range;
        AC = ac;
    }
}

public class EnemySoundEventInfo : EventInfo
{
    public float Range;

    public EnemySoundEventInfo(GameObject source, float range, string description = "Enemy sound event") : base(source, description)
    {
        Range = range;
    }
}

public class AmmoPickupEventInfo : EventInfo
{
    public int AmmoAmount { get; private set; }

    public AmmoPickupEventInfo(GameObject gO, int amount, string description = "Player picked up ammo") : base(gO, description)
    {
        AmmoAmount = amount;
    }
}
