using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    private GameObject currentGO = null;
    public Transform PlayerTransform;
    public Transform[] PatrolLocations;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this != null)
        {
            EnemyRespawnEventInfo EREI = new EnemyRespawnEventInfo(this);
            RespawnEventListener.respawnListener.RegisterEnemy(EREI);
        }
    }

    public void SpawnEnemy()
    {
        if (currentGO != null)
        {
            Destroy(currentGO);
        }

        currentGO = Instantiate(enemy, gameObject.transform);

        if (currentGO.GetComponent<EnemyStateMachine>() != null)
        {
            currentGO.GetComponent<EnemyStateMachine>().playerTransform = PlayerTransform;
            if (PatrolLocations.Length > 0)
                currentGO.GetComponent<EnemyStateMachine>().patrolLocations = PatrolLocations;
        }
        //else if (currentGO.GetComponent<StunbotStateMachine>() != null)
        //{
        //    currentGO.GetComponent<StunbotStateMachine>().playerTransform = PlayerTransform;
        //    if (PatrolLocations.Length > 0)
        //        currentGO.GetComponent<StunbotStateMachine>().patrolLocations = PatrolLocations;
        //}
    }
}
