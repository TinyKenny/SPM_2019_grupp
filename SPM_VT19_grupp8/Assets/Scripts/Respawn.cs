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

    private void Update()
    {
        if (!triggerd)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 4.0f)
            {
                Debug.Log("Changed");
                player.GetComponent<PlayerStateMachine>().respawnPoint = transform;
                triggerd = true;
            }
        }
    }


}
