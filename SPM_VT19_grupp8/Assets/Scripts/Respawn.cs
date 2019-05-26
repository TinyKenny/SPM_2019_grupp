using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    private bool triggerd = false;

    private void Awake()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<SaveEventInfo>(SaveCheckPointStatus);
    }

    private void Start()
    {
        SaveFile save = GameController.GameControllerInstance.CurrentSave;
        if (save.CheckpointPickupList.ContainsKey(name))
        {
            triggerd = save.CheckpointPickupList[name];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && triggerd == false)
        {
            GameController.GameControllerInstance.CurrentSave.LevelIndex = SceneManager.GetActiveScene().buildIndex;

            triggerd = true;

            EventCoordinator.CurrentEventCoordinator.ActivateEvent(new SaveEventInfo(gameObject));
        }
    }

    private void SaveCheckPointStatus(EventInfo eI)
    {
        GameController.GameControllerInstance.CurrentSave.AddCheckpoint(name, triggerd);
    }

    private void OnDestroy()
    {
        SaveFile save = GameController.GameControllerInstance.CurrentSave;
        if (save.CheckpointPickupList.ContainsKey(name))
            GameController.GameControllerInstance.CurrentSave.RemoveCheckpoint(name);
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<SaveEventInfo>(SaveCheckPointStatus);
    }
}
