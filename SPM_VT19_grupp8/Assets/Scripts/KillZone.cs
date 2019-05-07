using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KillZone : MonoBehaviour
{
    public AudioSource ausKillZone;
    public AudioClip killZoneSound;
    private void OnTriggerEnter(Collider other)
    {
        PlayerStateMachine player = other.GetComponent<PlayerStateMachine>();
            ausKillZone.PlayOneShot(killZoneSound);
        if (player != null)
        {

            player.Respawn();
        }
    }
}
