using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is a statemachine that (together with its states) acts as the controller for the player character.
/// This class holds several variables whose values are not meant to be stored in the states themselves.
/// </summary>
public class PlayerStateMachine : StateMachine
{
    #region "chaining" properties
    public float Acceleration { get { return PhysicsComponent.acceleration; } }
    public float Deceleration { get { return PhysicsComponent.deceleration; } }
    public float MaxSpeed { get { return PhysicsComponent.maxSpeed; } }
    public float AirResistanceCoefficient { get { return PhysicsComponent.airResistanceCoefficient; } }
    public float Gravity { get { return PhysicsComponent.gravity; } }
    public Vector3 Velocity { get { return PhysicsComponent.velocity; } set { PhysicsComponent.velocity = value; } }
    #endregion

    #region "plain" properties
    public PhysicsComponent PhysicsComponent { get; private set; }
    public CapsuleCollider ThisCollider { get; private set; }
    public float TimeSlowMultiplier  { get; private set; }
    public float StandardColliderHeight { get; private set; }
    #endregion

    #region properties for getting inspector-variables
    public LayerMask CollisionLayers { get { return collisionLayers; } }
    public float TurnSpeedModifier { get { return turnSpeedModifier; } }
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }
    public float FireRate { get { return fireRate; } }
    public float JumpPower { get { return jumpPower; } }
    public AudioClip GunShotSound { get { return gunShotSound; } }
    #endregion

    #region inspector-variables
    [SerializeField] private LayerMask collisionLayers = 0;
    [SerializeField] private float turnSpeedModifier = 0;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] private float slowedPlayerTimeScale = 0.5f;
    [SerializeField] private float slowedWorldTimeScale = 0.2f;
    [SerializeField] private float slowMotionEnergyMax = 5.0f;
    [SerializeField] private float slowMotionEnergyRegeneration = 1.0f;
    [SerializeField] private float shieldsMax = 10.0f;
    [SerializeField] private float shieldsRegeneration = 1.0f;
    [SerializeField] private float shieldsRegenerationCooldown = 4.0f;
    [SerializeField] private Slider timeSlowEnergy;
    [SerializeField] private Slider shieldAmount = null;
    [SerializeField] private float wallrunCooldownAmount = 0.5f;
    [SerializeField] private float jumpPower = 12.5f;
    [SerializeField] private AudioSource aus = null;
    [SerializeField] private AudioClip slowSound = null;
    [SerializeField] private AudioClip ammoSound = null;
    [SerializeField] private AudioClip damageSound = null;
    [SerializeField] private AudioClip deathSound = null;
    [SerializeField] private AudioClip gunShotSound = null;
    #endregion

    #region readonly values
    public readonly float skinWidth = 0.01f;
    public readonly float groundCheckDistance = 0.01f;
    #endregion

    #region private variables
    private AmmoPickup[] pickups; // get rid of this, use events and event listeners in the AmmoPickup-script instead
    private float playerTimeScale = 1.0f;
    private float currentSlowMotionEnergy = 5.0f;
    private float currentShields = 10.0f;
    private float shieldsRegenerationTimer = 0.0f;
    private float tempTimeScale;
    private float wallrunCooldown;
    private float timeScale = 1;
    #endregion



    public float fireCoolDown = 0.0f; // what to do about this one?

    public int ammo = 0; // make this private
    public Text ammoNumber; // make this private
    public Transform respawnPoint; // make this private, create a new event type ("CheckpointReachedEventInfo", maybe?) and make a listener for that event type in PlayerStateMachine




    protected override void Awake()
    {
        PhysicsComponent = GetComponent<PhysicsComponent>();
        ThisCollider = GetComponent<CapsuleCollider>();
        StandardColliderHeight = ThisCollider.height;
        TimeSlowMultiplier = 1.0f;

        base.Awake();

        timeSlowEnergy.maxValue = slowMotionEnergyMax;
        shieldAmount.maxValue = shieldsMax;
        wallrunCooldown = wallrunCooldownAmount;

        pickups = FindObjectsOfType<AmmoPickup>(); // use event-listeners instead
    }
    
    private void Start()
    {
        ResetValues();
    }

    protected override void Update()
    {
        base.Update();

        #region developer cheats
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Respawn();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ammo = 999;
            ammoNumber.text = ammo.ToString();
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            currentShields = shieldsMax;
            shieldsRegenerationTimer = 0.0f;
        }
        #endregion

        Pause();
        RegenerateShields();

        if (Mathf.Approximately(playerTimeScale, 1.0f))
        {
            currentSlowMotionEnergy = Mathf.Clamp(currentSlowMotionEnergy + slowMotionEnergyRegeneration * Time.deltaTime, 0.0f, slowMotionEnergyMax);
            timeSlowEnergy.value = currentSlowMotionEnergy;

            if (Input.GetButtonDown("TimeSlowToggle") && currentSlowMotionEnergy >= 1.0f)
            {
                Time.timeScale = slowedWorldTimeScale;
                playerTimeScale = slowedPlayerTimeScale;
                aus.PlayOneShot(slowSound);
                TimeSlowMultiplier = 1.2f;

            }
        }
        else if(Mathf.Approximately(playerTimeScale, slowedPlayerTimeScale))
        {
            currentSlowMotionEnergy = Mathf.Clamp(currentSlowMotionEnergy - Time.unscaledDeltaTime, 0.0f, slowMotionEnergyMax);
            timeSlowEnergy.value = currentSlowMotionEnergy;

            if (Input.GetButtonDown("TimeSlowToggle") || currentSlowMotionEnergy <= MathHelper.floatEpsilon)
            {
                Time.timeScale = 1.0f;
                playerTimeScale = 1.0f;
                TimeSlowMultiplier = 1;
            }
        }
        wallrunCooldown -= getPlayerDeltaTime();
    }

    /// <summary>
    /// Returns Time.unscaledDeltaTime multiplied by the players personal timescale.
    /// </summary>
    /// <returns>The players deltaTime</returns>
    public float getPlayerDeltaTime()
    {
        if (Mathf.Approximately(playerTimeScale, 1.0f)) // make this prettier
        {
            return Time.deltaTime;
        }
        return Time.unscaledDeltaTime * playerTimeScale;
    }

    public void TakeDamage(float damage)
    {
        if (currentShields <= MathHelper.floatEpsilon)
        {
            aus.PlayOneShot(deathSound);
            Debug.Log("Player took fatal damage");
            Respawn();
        }
        else
        {
            aus.PlayOneShot(damageSound);
            currentShields = Mathf.Clamp(currentShields - damage, 0.0f, shieldsMax);
            shieldsRegenerationTimer = shieldsRegenerationCooldown;
        }
    }

    public void Pause()
    {
        if (Input.GetButtonDown("Pause") && Time.timeScale > 0)
        {
            tempTimeScale = playerTimeScale;
            timeScale = Time.timeScale;
            Time.timeScale = 0;
            playerTimeScale = 0;
        }
        else if (Input.GetButtonDown("Pause") && Time.timeScale == 0)
        {

            Time.timeScale = timeScale;
            playerTimeScale = tempTimeScale;
        }
    }

    //public void UnPause()
    //{
    //    Time.timeScale = timeScale;
    //    playerTimeScale = tempTimeScale;
    //    GameController.gameControllerInstance.PausePanel.SetActive(false);
    //}

    /// <summary>
    /// needs re-writing
    /// 
    /// Transitions the player to PlayerAirState and sets the players position to the respawn point.
    /// Resets variables related to life, slow-mo, attacking, ect. (example: deactivating slow-mo and refilling shields)
    /// </summary>
    public void Respawn()
    {
        //Quaternion q = respawnPoint.rotation;
        //q.x = 0;
        //q.z = 0;
        //TransitionTo<PlayerAirState>();
        //transform.position = respawnPoint.position;
        //transform.rotation = q;
        //Velocity = Vector3.zero;
        //Time.timeScale = 1.0f;
        //playerTimeScale = 1.0f;
        //shieldsRegenerationTimer = 0.0f;
        //currentShields = shieldsMax;
        //currentSlowMotionEnergy = slowMotionEnergyMax;
        //fireCoolDown = 0.0f;

        //ammo = 0;
        //ammoNumber.text = ammo.ToString();

        ResetValues();


        PlayerRespawnEventInfo PREI = new PlayerRespawnEventInfo(gameObject);
        EventCoordinator.CurrentEventCoordinator.ActivateEvent(PREI);


        //replace the stuff below, to be triggered by events

        foreach (AmmoPickup pickup in pickups)
            pickup.gameObject.SetActive(true);
    }

    private void ResetValues()
    {
        Velocity = Vector3.zero;
        TransitionTo<PlayerAirState>();
        transform.position = respawnPoint.position;
        transform.rotation = Quaternion.Euler(0.0f, respawnPoint.rotation.eulerAngles.y, 0.0f);
        Time.timeScale = 1.0f; // create stop-slow method?
        playerTimeScale = 1.0f;
        currentShields = shieldsMax;
        currentSlowMotionEnergy = slowMotionEnergyMax;
        fireCoolDown = 0.0f;
        ammo = 0;
        ammoNumber.text = ammo.ToString();
    }

    public void AddAmmo(int ammo)
    {
        this.ammo += ammo;
        ammoNumber.text = this.ammo.ToString();
        aus.PlayOneShot(ammoSound);
    }

    public void ResetWallrunCooldown()
    {
        wallrunCooldown = wallrunCooldownAmount;
    }

    public bool WallrunAllowed()
    {
        return wallrunCooldown < 0f;
    }

    private void RegenerateShields()
    {
        if (shieldsRegenerationTimer <= 0.0f)
        {
            currentShields = Mathf.Clamp(currentShields + shieldsRegeneration * Time.deltaTime, 0.0f, shieldsMax);
        }
        else
        {
            shieldsRegenerationTimer -= Time.deltaTime;
        }

        shieldAmount.value = currentShields;
    }
}
