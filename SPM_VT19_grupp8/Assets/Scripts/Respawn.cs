using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    private bool triggerd = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && triggerd == false)
        {
            GameController.GameControllerInstance.CurrentSave.LevelIndex = SceneManager.GetActiveScene().buildIndex;

            triggerd = true;

            EventCoordinator.CurrentEventCoordinator.ActivateEvent(new SaveEventInfo(gameObject));
        }
    }


}
