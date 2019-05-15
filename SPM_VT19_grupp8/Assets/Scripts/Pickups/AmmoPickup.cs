using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] private int ammoAmount = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PickupAmmo(other.gameObject);
        }
    }

    private void PickupAmmo(GameObject player)
    {
        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new AmmoPickupEventInfo(player, ammoAmount));

        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerRespawnEventInfo>(EnablePickup);
        gameObject.SetActive(false);
    }

    private void EnablePickup(EventInfo eI)
    {
        gameObject.SetActive(true);
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<PlayerRespawnEventInfo>(EnablePickup);
    }
}
