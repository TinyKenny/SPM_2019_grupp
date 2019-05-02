using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    private GameObject currentGO = null;
    public Transform PlayerTransform;
    public Transform[] PatrolLocations;

    void Start()
    {
        EnemyRespawnEventInfo EREI = new EnemyRespawnEventInfo(this);
        RespawnEventListener.respawnListener.RegisterEnemy(EREI);
    }

    public void SpawnEnemy()
    {
        if (currentGO != null)
        {
            Destroy(currentGO);
        }

        currentGO = Instantiate(enemy, gameObject.transform);

        if (currentGO.GetComponent<SoldierStateMachine>() != null)
        {
            currentGO.GetComponent<SoldierStateMachine>().playerTransform = PlayerTransform;
            if (PatrolLocations.Length > 0)
                currentGO.GetComponent<SoldierStateMachine>().patrolLocations = PatrolLocations;
        }
        else if (currentGO.GetComponent<StunbotStateMachine>() != null)
        {
            currentGO.GetComponent<StunbotStateMachine>().playerTransform = PlayerTransform;
            if (PatrolLocations.Length > 0)
                currentGO.GetComponent<StunbotStateMachine>().patrolLocations = PatrolLocations;
        }
    }
}
