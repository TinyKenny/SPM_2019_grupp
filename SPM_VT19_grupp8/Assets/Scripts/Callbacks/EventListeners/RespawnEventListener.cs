using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEventListener : EventListenerInterface
{
    public static RespawnEventListener respawnListener;
    private List<EnemyRespawnEventInfo> spawnerList = new List<EnemyRespawnEventInfo>();
    [SerializeField] private CameraController cameraMain;

    public override void Initialize()
    {
        spawnerList.Clear();
        respawnListener = this;
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<EnemyRespawnEventInfo>(RespawnObject);
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerRespawnEventInfo>(OnPlayerRespawn);
    }

    public void RespawnObject(EventInfo ei)
    {
        EnemyRespawnEventInfo EREI = (EnemyRespawnEventInfo)ei;

        EREI.GetSpawner().SpawnEnemy();
    }

    public void RegisterEnemy(EnemyRespawnEventInfo erei)
    {
        spawnerList.Add(erei);
    }

    public void OnPlayerRespawn(EventInfo eventInfo)
    {
        PlayerRespawnEventInfo PREI = (PlayerRespawnEventInfo)eventInfo;

        //Vector3 playerRotation = PREI.GO.transform.rotation.eulerAngles;
        cameraMain.transform.rotation = PREI.GO.transform.rotation;
        cameraMain.rotationY = cameraMain.transform.rotation.eulerAngles.y;
        cameraMain.rotationX = cameraMain.transform.rotation.eulerAngles.x;

        foreach (EnemyRespawnEventInfo EREI in spawnerList)
        {
            EREI.SetPlayer(PREI.GO.transform);
            EventCoordinator.CurrentEventCoordinator.ActivateEvent(EREI);
        }
    }
}
