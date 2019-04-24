using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{


    private static Transform checkPoint;
    public int position;
    private void Start()
    {
        checkPoint = gameObject.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Player"))
        {
            //if(position > PlayerStateMachine.respawnPoint.position)
           // PlayerStateMachine.respawnPoint = checkPoint;
        }
    }
}
