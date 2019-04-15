using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerStateMachine player = other.GetComponent<PlayerStateMachine>();

        if (player != null)
        {
            player.Respawn();
        }
    }
}
