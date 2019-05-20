using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private bool triggerd = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && triggerd == false)
        {
            GameController.GameControllerInstance.CurrentSave.PlayerPosition = new PositionInfo(transform.position);
            GameController.GameControllerInstance.CurrentSave.PlayerRotationY = transform.rotation.eulerAngles.y;

            triggerd = true;

            EventCoordinator.CurrentEventCoordinator.ActivateEvent(new EnemySaveEventInfo(gameObject));

            SaveFile.ClearSave();

            SaveFile.CreateSave();

            GameController.GameControllerInstance.SavePlayerVariables();
        }
    }


}
