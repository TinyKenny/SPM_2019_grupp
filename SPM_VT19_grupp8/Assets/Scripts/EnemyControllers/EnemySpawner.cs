using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private bool CheckSpawnSaveStatus()
    {
        bool spawnEnemy = false;
        if (GameController.GameControllerInstance.CurrentSave.IsEmpty == false)
            spawnEnemy = GameController.GameControllerInstance.CurrentSave.EnemyInfoList.ContainsKey(name);
        else
            spawnEnemy = true;

        return spawnEnemy;
    }

    /// <summary>
    /// Removes enemy if they have not already been removed then instantiates a new <see cref="enemy"/>.
    /// Automatically sets the <see cref="PlayerTransform"/> and the <see cref="PatrolLocations"/> set on <see cref="currentGO"/>.
    /// </summary>
    /// <param name="EI">A <see cref="PlayerRespawnEventInfo"/> where the player is the gameobject.</param>
    private void SpawnEnemy(EventInfo EI)
    {
        if (CheckSpawnSaveStatus())
        {
            PlayerRespawnEventInfo PREI = (PlayerRespawnEventInfo)EI;
            PlayerTransform = PREI.GO.transform;

            Vector3 position = transform.position;
            Vector3 rotation = transform.rotation.eulerAngles;
            int currentState = 0;

            if (GameController.GameControllerInstance.CurrentSave.EnemyInfoList.ContainsKey(gameObject.name))
            {
                System.Tuple<PositionInfo, PositionInfo, int> savedInfo = GameController.GameControllerInstance.CurrentSave.EnemyInfoList[gameObject.name];
                position = savedInfo.Item1.Position;
                rotation = savedInfo.Item2.Position;
                currentState = savedInfo.Item3;
            }

            if (currentGO != null)
            {
                Destroy(currentGO);
            }

            currentGO = Instantiate(enemy, gameObject.transform);

            if (currentGO.GetComponent<EnemyStateMachine>() != null)
            {
                EnemyStateMachine eSM = currentGO.GetComponent<EnemyStateMachine>();
                eSM.PlayerTransform = PlayerTransform;
                eSM.SpawnerName = name;

                if (PatrolLocations.Length == 0)
                {
                    PatrolLocations = new Transform[1];
                    PatrolLocations[0] = transform;
                }
                eSM.PatrolLocations = PatrolLocations;
                eSM.SetSavedState(currentState);
            }

            currentGO.transform.position = position;
            currentGO.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    private void OnDestroy()
    {
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<PlayerRespawnEventInfo>(SpawnEnemy);
    }
}
