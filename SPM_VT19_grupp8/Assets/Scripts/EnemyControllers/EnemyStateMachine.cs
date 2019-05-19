using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Superclass for all enemy statemachines. Used for general variables all enemies should have and functions.
/// </summary>
public class EnemyStateMachine : StateMachine
{
    public Transform[] PatrolLocations { get; set; }
    public Transform PlayerTransform { get; set; }

    private AudioSource aus;
    [SerializeField] private AudioClip alertSound = null;
    private float wallSoundAbsorbation = 0.5f;

    protected override void Awake()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerDiegeticSoundEventInfo>(PlayerSoundAlertCheck);
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerAttackEventInfo>(OnPlayerAttack);
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<EnemySaveEventInfo>(SaveEnemy);
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

    private void OnDestroy()
    {
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<PlayerDiegeticSoundEventInfo>(PlayerSoundAlertCheck);
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<PlayerAttackEventInfo>(OnPlayerAttack);
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<EnemySaveEventInfo>(SaveEnemy);
    }

    public void SaveEnemy(EventInfo eI)
    {
        EnemySaveEventInfo sEI = (EnemySaveEventInfo)eI;
        GameController.gameControllerInstance.CurrentSave.AddEnemy(transform.position, GetComponent<EnemyHealthPOC>().CurrentHealth);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, GameController.gameControllerInstance.CurrentSave);
        file.Close();
    }
}
