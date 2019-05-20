using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{


    private GameObject player;
    private bool triggerd = false;
    private void Awake()
    {
       
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && triggerd == false)
        {
            player.GetComponent<PlayerStateMachine>().respawnPoint = transform.position;
            player.GetComponent<PlayerStateMachine>().respawnRotation = transform.eulerAngles;
            GameController.GameControllerInstance.CurrentSave.PlayerPosition = new PositionInfo(transform.position);
            GameController.GameControllerInstance.CurrentSave.PlayerRotation = new PositionInfo(transform.eulerAngles);

            triggerd = true;

            EventCoordinator.CurrentEventCoordinator.ActivateEvent(new EnemySaveEventInfo(gameObject));

            SaveFile.ClearSave();

            SaveFile.CreateSave();

            GameController.GameControllerInstance.SavePlayerVariables();
        }
    }


}
