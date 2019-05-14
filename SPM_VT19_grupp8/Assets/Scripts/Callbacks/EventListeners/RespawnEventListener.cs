using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Eventlistener for handling events triggered by the player respawn like enemy respawns.
/// </summary>
public class RespawnEventListener : EventListenerInterface
{
    [SerializeField] private CameraController cameraMain;
    
    void Awake()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerRespawnEventInfo>(OnPlayerRespawn);
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerRespawnEventInfo>(cameraMain.GetComponent<CameraController>().OnPlayerRespawn); // remove this line
    }

    /// <summary>
    /// Respawns the player at the latest checkpoint, then respawns all enemies.
    /// </summary>
    /// <param name="eventInfo">A <see cref="PlayerRespawnEventInfo"/> representing the player.</param>
    public void OnPlayerRespawn(EventInfo eventInfo)
    {
        PlayerRespawnEventInfo PREI = (PlayerRespawnEventInfo)eventInfo;

        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new EnemyRespawnEventInfo(PREI.GO));

    }
}
