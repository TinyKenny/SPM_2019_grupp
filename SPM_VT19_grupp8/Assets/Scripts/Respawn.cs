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
            player.GetComponent<PlayerStateMachine>().respawnPoint = transform;
            triggerd = true;
            EventCoordinator.CurrentEventCoordinator.ActivateEvent(new EnemySaveEventInfo(gameObject));

            SaveFile.ClearSave();

            SaveFile.CreateSave();

            GameController.GameControllerInstance.SavePlayerVariables();
        }
    }


}
