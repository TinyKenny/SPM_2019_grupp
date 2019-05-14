using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Instantiates an enemy and sets their patrollocations as well as playerTransform after players respawn. Should be
/// placed instead of enemies in the levels otherwise they will not respawn.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    private GameObject currentGO = null;
    public Transform PlayerTransform;
    public Transform[] PatrolLocations;

    private void Awake()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<EnemyRespawnEventInfo>(SpawnEnemy);
    }

    /// <summary>
    /// Removes enemy if they have not already been removed then instantiates a new enemy.
    /// Automatically sets the playertransform and the patrollocations set on the <see cref="EnemySpawner"/> object.
    /// </summary>
    /// <param name="EI">A <see cref="EnemyRespawnEventInfo"/> where the player is the gameobject.</param>
    public void SpawnEnemy(EventInfo EI)
    {
        EnemyRespawnEventInfo EREI = (EnemyRespawnEventInfo)EI;
        PlayerTransform = EREI.GO.transform;

        if (currentGO != null)
        {
            Destroy(currentGO);
        }

        currentGO = Instantiate(enemy, gameObject.transform);

        if (currentGO.GetComponent<EnemyStateMachine>() != null)
        {
            currentGO.GetComponent<EnemyStateMachine>().PlayerTransform = PlayerTransform;
            if (PatrolLocations.Length > 0)
                currentGO.GetComponent<EnemyStateMachine>().PatrolLocations = PatrolLocations;
        }
    }
}
