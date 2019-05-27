using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Superclass for all enemy statemachines. Used for general variables all enemies should have and functions.
/// </summary>
public class EnemyStateMachine : StateMachine
{
    public Transform[] PatrolLocations { get; set; }
    public Transform PlayerTransform { get; set; }
    public string SpawnerName { private get; set; }
    public Vector3 LastPlayerLocation { get; set; }
    public int CurrentPatrolPointIndex { get; set; }

    private AudioSource aus;
    [SerializeField] private AudioClip alertSound = null;
    [SerializeField] private AudioClip deathSound = null;
    private float wallSoundAbsorbation = 0.5f;

    protected override void Awake()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerDiegeticSoundEventInfo>(PlayerSoundAlertCheck);
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerAttackEventInfo>(OnPlayerAttack);
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<SaveEventInfo>(SaveEnemy);
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<EnemyDamageEventInfo>(EnemyDamageEvent);
        aus = GetComponent<AudioSource>();
        base.Awake();
    }

    /// <summary>
    /// Checks if a sound generated is within hearing range and sets enemy as alerted.
    /// </summary>
    /// <param name="eI"><see cref="PlayerDiegeticSoundEventInfo"/> representing the player and a diegetic sound with range it caused.</param>
    public void PlayerSoundAlertCheck(EventInfo eI)
    {
        PlayerDiegeticSoundEventInfo enemyEvent = (PlayerDiegeticSoundEventInfo)eI;
        if (currentState.GetType() == typeof(SoldierIdleState) || currentState.GetType() == typeof(StunbotIdleState))
        {
            if (Vector3.Distance(transform.position, enemyEvent.GO.transform.position) < enemyEvent.Range)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, PlayerTransform.position, out hit, 1 << 9))
                {
                    if (Vector3.Distance(transform.position, enemyEvent.GO.transform.position) < (enemyEvent.Range - hit.distance) * wallSoundAbsorbation + hit.distance)
                        HeardPlayer(enemyEvent.GO);
                }
                else
                    HeardPlayer(enemyEvent.GO);
            }
        }
    }

    /// <summary>
    /// Determines wether the enemy is hit a player attack or not.
    /// If the enemy is hit, calls the virtual method 
    /// </summary>
    /// <param name="eI"></param>
    private void OnPlayerAttack(EventInfo eI)
    {
        PlayerAttackEventInfo pAEI = (PlayerAttackEventInfo)eI;

        Vector3 positionDifference = transform.position - pAEI.Origin;

        if(positionDifference.sqrMagnitude < pAEI.Range * pAEI.Range && Vector3.Dot(positionDifference.normalized, pAEI.Direction) >= pAEI.Angle)
        {
            HitByPlayerAttack(pAEI);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pAEI"></param>
    protected virtual void HitByPlayerAttack(PlayerAttackEventInfo pAEI)
    {

    }

    /// <summary>
    /// Plays an alertsound and sets alerted with the last know position of the parameter gameobject.
    /// </summary>
    /// <param name="player">Gameobject that should always be the player.</param>
    private void HeardPlayer(GameObject player)
    {
        Debug.Log("Heard player!");
        SetAlerted(player.transform.position);
        aus.PlayOneShot(alertSound);
    }

    /// <summary>
    /// Sets alerted, needs to be ovverided by sub classes.
    /// </summary>
    /// <param name="position">Last known position of the player.</param>
    public virtual void SetAlerted(Vector3 position)
    {

    }

    public void EnemyDamageEvent(EventInfo ei)
    {
        if (ei.GO.Equals(gameObject))
        {
            if (deathSound != null)
            {
                AudioSource aS = Instantiate(aus).GetComponent<AudioSource>();
                aS.PlayOneShot(deathSound);
                Destroy(aS, deathSound.length);
            }
            GameController.GameControllerInstance.CurrentSave.EnemyInfoList.Remove(SpawnerName);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<PlayerDiegeticSoundEventInfo>(PlayerSoundAlertCheck);
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<PlayerAttackEventInfo>(OnPlayerAttack);
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<SaveEventInfo>(SaveEnemy);
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<EnemyDamageEventInfo>(EnemyDamageEvent);
    }

    public void SaveEnemy(EventInfo eI)
    {
        SaveEventInfo sEI = (SaveEventInfo)eI;
        GameController.GameControllerInstance.CurrentSave.AddEnemy(transform.position, transform.rotation.eulerAngles, LastPlayerLocation, transform.parent.name, currentState.Index, CurrentPatrolPointIndex);
    }
}
