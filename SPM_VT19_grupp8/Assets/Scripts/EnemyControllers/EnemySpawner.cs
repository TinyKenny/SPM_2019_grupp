using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiates an enemy and sets their patrollocations as well as playerTransform after players respawn. Should be
/// placed instead of enemies in the levels otherwise they will not respawn.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy = null;
    [SerializeField] private Transform[] PatrolLocations = null;
    private Transform PlayerTransform;
    private GameObject currentGO = null;

    private void Awake()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerRespawnEventInfo>(SpawnEnemy);
    }

    /// <summary>
    /// Removes enemy if they have not already been removed then instantiates a new <see cref="enemy"/>.
    /// Automatically sets the <see cref="PlayerTransform"/> and the <see cref="PatrolLocations"/> set on <see cref="currentGO"/>.
    /// </summary>
    /// <param name="EI">A <see cref="PlayerRespawnEventInfo"/> where the player is the gameobject.</param>
    private void SpawnEnemy(EventInfo EI)
    {
        PlayerRespawnEventInfo PREI = (PlayerRespawnEventInfo)EI;
        PlayerTransform = PREI.GO.transform;

        Vector3 position = transform.position;

        if (GameController.GameControllerInstance.CurrentSave.EnemyInfoList.ContainsKey(gameObject.name))
        {
            position = GameController.GameControllerInstance.CurrentSave.EnemyInfoList[gameObject.name].Position;
        }

        if (currentGO != null)
        {
            Destroy(currentGO);
        }

        currentGO = Instantiate(enemy, gameObject.transform);

        if (currentGO.GetComponent<EnemyStateMachine>() != null)
        {
            currentGO.GetComponent<EnemyStateMachine>().PlayerTransform = PlayerTransform;

            if (PatrolLocations.Length == 0)
            {
                PatrolLocations = new Transform[1];
                PatrolLocations[0] = transform;
            }
            currentGO.GetComponent<EnemyStateMachine>().PatrolLocations = PatrolLocations;
        }

        currentGO.transform.position = position;
    }

    private void OnDestroy()
    {
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<PlayerRespawnEventInfo>(SpawnEnemy);
    }
}
