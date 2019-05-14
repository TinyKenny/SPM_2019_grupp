using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEventListener : EventListenerInterface
{
    public static RespawnEventListener respawnListener;
    [SerializeField] private CameraController cameraMain;
    
    public override void Initialize()
    {
        respawnListener = this;
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerRespawnEventInfo>(OnPlayerRespawn);
    }

    /// <summary>
    /// Respawns the player at the latest checkpoint, then respawns all enemies.
    /// </summary>
    /// <param name="eventInfo">A <see cref="PlayerRespawnEventInfo"/> representing the player.</param>
    public void OnPlayerRespawn(EventInfo eventInfo)
    {
        PlayerRespawnEventInfo PREI = (PlayerRespawnEventInfo)eventInfo;

        //Vector3 playerRotation = PREI.GO.transform.rotation.eulerAngles;
        //cameraMain.transform.rotation = PREI.GO.transform.rotation;
        //cameraMain.rotationY = playerRotation.y;
        //cameraMain.rotationX = playerRotation.x;

        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new EnemyRespawnEventInfo(PREI.GO));

    }
}
