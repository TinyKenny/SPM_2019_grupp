using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles ammo pickups and triggers <see cref="AmmoPickupEventInfo"/> event when a player enters its collider. Adds <see cref="ammoAmount"/> ammo to the player and deactivates the gameobject until player respawns.
/// Requires a collider.
/// </summary>
[RequireComponent(typeof(Collider))]
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

    /// <summary>
    /// Acitvates ammopickupevents to update player ammo amount and registers to reset the pickup when player respawns.
    /// </summary>
    /// <param name="player">The players gameobject</param>
    private void PickupAmmo(GameObject player)
    {
        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new AmmoPickupEventInfo(player, ammoAmount));

        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerRespawnEventInfo>(EnablePickup);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Activation event that should be fired when a player respawns if the ammopickup has been picked up in the level.
    /// </summary>
    /// <param name="eI"><see cref="PlayerRespawnEventInfo"/> triggered through the <see cref="EventCoordinator"/></param>
    private void EnablePickup(EventInfo eI)
    {
        gameObject.SetActive(true);
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<PlayerRespawnEventInfo>(EnablePickup);
    }
}
