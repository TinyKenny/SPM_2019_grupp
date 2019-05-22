using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateMachine player = other.GetComponent<PlayerStateMachine>();
            player.Respawn();
        }
        else if (other.CompareTag("Enemy Hitbox"))
        {
            EnemyDamageEventInfo eDEI = new EnemyDamageEventInfo(other.gameObject);
            EventCoordinator.CurrentEventCoordinator.ActivateEvent(eDEI);
        }
    }
}
