using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    public Transform[] PatrolLocations { get { return patrolLocations; } set { patrolLocations = value; } }
    public Transform PlayerTransform { get { return playerTransform; } set { playerTransform = value; } }

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform[] patrolLocations;

    private AudioSource aus;
    [SerializeField] private AudioClip alertSound;
    private float wallSoundAbsorbation = 0.8f;

    private void Awake()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<EnemySoundEventInfo>(PlayerSoundAlert);
        aus = GetComponent<AudioSource>();
    }

    public void PlayerSoundAlert(EventInfo eI)
    {
        EnemySoundEventInfo enemyEvent = (EnemySoundEventInfo)eI;
        if (currentState.GetType() == typeof(SoldierIdleState) || currentState.GetType() == typeof(StunbotIdleState))
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position, playerTransform.position, out hit, 1 << 9))
            {
                if (Vector3.Distance(transform.position, enemyEvent.GO.transform.position) < (enemyEvent.Range - hit.distance) * wallSoundAbsorbation + hit.distance)
                    HeardPlayer(enemyEvent.GO);
            }
            else if (Vector3.Distance(transform.position, enemyEvent.GO.transform.position) < enemyEvent.Range)
            {
                HeardPlayer(enemyEvent.GO);
            }
        }
    }

    private void HeardPlayer(GameObject player)
    {
        Debug.Log("Heard player!");
        SetAlerted(player.transform.position);
        aus.PlayOneShot(alertSound);
    }

    public virtual void SetAlerted(Vector3 position)
    {

    }
}
