using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEventListener : MonoBehaviour
{
    public static RespawnEventListener respawnListener;
    private List<EnemyRespawnEventInfo> spawnerList;

    private void Start()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<EnemyRespawnEventInfo>(RespawnObjects);
    }

    public void RespawnObjects(EventInfo ei)
    {

    }

    public void RegisterEnemy(EnemyRespawnEventInfo erei)
    {
        spawnerList.Add(erei);
    }
}
