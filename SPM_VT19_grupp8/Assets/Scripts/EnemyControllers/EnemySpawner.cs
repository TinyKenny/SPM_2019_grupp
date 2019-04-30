using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    private GameObject currentGO = null;

    void Start()
    {
        SpawnEnemy();
        RespawnEventListener.respawnListener.RegisterEnemy(new EnemyRespawnEventInfo(this));
    }

    public void SpawnEnemy()
    {
        if (currentGO != null)
        {
            Destroy(currentGO);
        }

        currentGO = Instantiate(enemy);
    }
}
