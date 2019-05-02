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

public class EnemyRespawnEventInfo : EventInfo
{
    public EnemyRespawnEventInfo(EnemySpawner gO, string desciption = "Enemy respawning") : base(gO.gameObject, desciption)
    {
    }

    public EnemySpawner GetSpawner()
    {
        return GO.GetComponent<EnemySpawner>();
    }

    public void SetPlayer(Transform player)
    {
        GO.GetComponent<EnemySpawner>().PlayerTransform = player;
    }

    public Transform GetPlayer()
    {
        return GO.GetComponent<EnemySpawner>().PlayerTransform;
    }
}
