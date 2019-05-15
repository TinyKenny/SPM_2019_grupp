using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is a statemachine that (together with its states) acts as the controller for the player character.
/// This class holds several variables whose values are not meant to be stored in the states themselves.
/// It also contains some behaviours that do not depend on which state the player is in, such as the slowmotion-ability.
/// </summary>
public class PlayerStateMachine : StateMachine
{
    #region "chaining" properties
    public Vector3 Velocity { get { return physicsComponent.velocity; } set { physicsComponent.velocity = value; } }
    public float Acceleration { get { return physicsComponent.acceleration; } }
    public float Deceleration { get { return physicsComponent.deceleration; } }
    public float MaxSpeed { get { return physicsComponent.maxSpeed; } }
    public float AirResistanceCoefficient { get { return physicsComponent.airResistanceCoefficient; } }
    public float Gravity { get { return physicsComponent.gravity; } }
    #endregion

    #region "plain" properties
    public CapsuleCollider ThisCollider { get; private set; }
    public CameraController MainCameraController { get; private set; }
    public Animator Animator { get; private set; }
    public float StandardColliderHeight { get; private set; }
    public int Ammo { get; private set; }
    #endregion

    #region properties for getting private variables
    public LayerMask CollisionLayers { get { return collisionLayers; } }
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }
    public AudioClip GunShotSound { get { return gunShotSound; } }
    public float TurnSpeedModifier { get { return turnSpeedModifier; } }
    public float FireRate { get { return fireRate; } }
    public float JumpPower { get { return jumpPower; } }
    public float MovementSoundRange { get { return movementSoundRange; } }
    public float ShootSoundRange { get { return shootSoundRange; } }
    #endregion

    #region serialized private variables
    [SerializeField] private LayerMask collisionLayers = 0;
    [SerializeField] private float turnSpeedModifier = 0;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] private float shieldsMax = 10.0f;
    [SerializeField] private float shieldsRegeneration = 1.0f;
    [SerializeField] private float shieldsRegenerationCooldown = 4.0f;
    [SerializeField] private Slider shieldAmount = null;
    [SerializeField] private float wallrunCooldownAmount = 0.5f;
    [SerializeField] private float jumpPower = 12.5f;
    [Range(0, 100)]
    [SerializeField] private float movementSoundRange = 20.0f;
    [Range(0, 100)]
    [SerializeField] private float shootSoundRange = 50;
    [SerializeField] private Text ammoNumber = null;
    [SerializeField] private AudioSource aus = null;
    [SerializeField] private AudioClip slowSound = null;
    [SerializeField] private AudioClip ammoSound = null;
    [SerializeField] private AudioClip damageSound = null;
    [SerializeField] private AudioClip deathSound = null;
    [SerializeField] private AudioClip gunShotSound = null;
    #endregion

    #region non-serialized private variables
    private PhysicsComponent physicsComponent = null;
    private float currentShields = 10.0f;
    private float shieldsRegenerationTimer = 0.0f;
    private float wallrunCooldown;
    private float fireCoolDown = 0.0f; // currently has a public property with both get and set, do something about that
    #endregion

    #region readonly values
    public readonly float skinWidth = 0.01f;
    public readonly float groundCheckDistance = 0.01f;
    #endregion




    #region time-stuff
    public float TimeSlowMultiplier  { get; private set; }
    [SerializeField] private float slowedPlayerTimeScale = 0.5f;
    [SerializeField] private float slowedWorldTimeScale = 0.2f;
    [SerializeField] private float slowMotionEnergyMax = 5.0f;
    [SerializeField] private float slowMotionEnergyRegeneration = 1.0f;
    [SerializeField] private Slider timeSlowEnergy = null;
    [SerializeField] private float slowMotionCooldown = 1.0f;
    private float playerTimeScale = 1.0f;
    private float currentSlowMotionEnergy = 5.0f;
    private float tempTimeScale;
    private float timeScale = 1;
    private float slowMotionCooldownTimer = 0.0f;
    #endregion



    public Transform respawnPoint; // make this private, create a new event type ("CheckpointReachedEventInfo", maybe?) and make a listener for that event type in PlayerStateMachine




    protected override void Awake()
    {
        physicsComponent = GetComponent<PhysicsComponent>();
        ThisCollider = GetComponent<CapsuleCollider>();
        Animator = GetComponentInChildren<Animator>();
        MainCameraController = Camera.main.GetComponent<CameraController>();
        StandardColliderHeight = ThisCollider.height;
        TimeSlowMultiplier = 1.0f;

        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<AmmoPickupEventInfo>(AddAmmo);
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerDiegeticSoundEventInfo>(PlayerDiegeticSound);

        base.Awake();

        timeSlowEnergy.maxValue = slowMotionEnergyMax;
        shieldAmount.maxValue = shieldsMax;
        wallrunCooldown = wallrunCooldownAmount;
    }
    
    private void Start()
    {
        Respawn();
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
            Ammo = 999;
            ammoNumber.text = Ammo.ToString();
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            currentShields = shieldsMax;
            shieldsRegenerationTimer = 0.0f;
        }
        #endregion

        Pause();
        RegenerateShields();
        wallrunCooldown -= getPlayerDeltaTime();

        #region the new slowmotion
        //commented out because it has not been tested yet, and will probably require a bit of fixing.
        //SlowMotion();
        #endregion


        #region the old slowmotion
        // when the pause-functionality and the new slowmotion have been made compatible with eachother (and the new slowmo has been properly tested), the code in this region can be removed
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
        #endregion
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

    /// <summary>
    /// Reduces the players current "shields" by a specified ammount.
    /// If the players current "shields" are lower than <see cref="MathHelper.floatEpsilon"/> when this method is called, the player dies and respawns.
    /// </summary>
    /// <param name="damage">The ammount to be subtracted from the players shields.</param>
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

    /// <summary>
    /// If the player gives the specified input, pauses or unpauses the game, depending on whether the game is paused or not.
    /// </summary>
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

    /// <summary>
    /// needs testing.
    /// 
    /// Calls <see cref="ResetValues"/> and then triggers a "player respawn"-event.
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
    }

    /// <summary>
    /// Needs testing.
    /// 
    /// Transitions the player to PlayerAirState and sets the players position to the respawn point.
    /// Resets variables related to shields, slow-mo, attacking, ect. (example: refilling slow-mo energy and shields)
    /// </summary>
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
        Ammo = 0;
        ammoNumber.text = Ammo.ToString();
    }

    /// <summary>
    /// Adds the specified ammount of ammunition to the players reserves.
    /// </summary>
    /// <param name="ammo"></param>
    public void AddAmmo(EventInfo eI)
    {
        AmmoPickupEventInfo aPEI = (AmmoPickupEventInfo)eI;

        Ammo += aPEI.AmmoAmount;
        ammoNumber.text = Ammo.ToString();
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

    /// <summary>
    /// If the players shields are not full, and shield regeneration is not on cooldown, regenerates the players shields.
    /// </summary>
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

    /// <summary>
    /// Slows down time, based on how much the player is holding down the "designated" trigger.
    /// This is currently not really compatible with the current implementation of the pause feature.
    /// </summary>
    private void SlowMotion()
    {
        float timeLerpValue = Input.GetAxisRaw("Shoot");
        if (timeLerpValue > 0.0f && slowMotionCooldownTimer <= MathHelper.floatEpsilon)
        {
            playerTimeScale = Mathf.Lerp(1.0f, slowedPlayerTimeScale, timeLerpValue);
            Time.timeScale = Mathf.Lerp(1.0f, slowedWorldTimeScale, timeLerpValue);
            TimeSlowMultiplier = Mathf.Lerp(1.0f, 1.2f, timeLerpValue);

            currentSlowMotionEnergy = Mathf.Clamp(currentSlowMotionEnergy - (Time.deltaTime / Time.timeScale), 0.0f, slowMotionEnergyMax);

            if (currentSlowMotionEnergy <= MathHelper.floatEpsilon)
            {
                slowMotionCooldownTimer = slowMotionCooldown;
            }
        }
        else
        {
            slowMotionCooldownTimer -= Time.deltaTime;
            currentSlowMotionEnergy = Mathf.Clamp(currentSlowMotionEnergy + slowMotionEnergyRegeneration * Time.deltaTime, 0.0f, slowMotionEnergyMax);
        }
    }

    public void Shoot()
    {
        if (Input.GetAxisRaw("Aim") == 1f)
        {
            MainCameraController.Aiming();

            if (Input.GetAxisRaw("Shoot") == 1f && fireCoolDown < 0 && Ammo > 0 && Time.timeScale > 0)
            {
                Ammo--;

                Vector3 reticleLocation = new Vector3(MainCameraController.MainCamera.pixelWidth / 2, MainCameraController.MainCamera.pixelHeight / 2, 0.0f);

                Ray aimRay = MainCameraController.MainCamera.ScreenPointToRay(reticleLocation); // make it so that CameraController has a reference to its camera

                float projectileRange = ProjectilePrefab.GetComponent<ProjectileBehaviour>().distanceToTravel;

                bool rayHasHit = Physics.Raycast(aimRay, out RaycastHit rayHit, projectileRange, ~(1 << gameObject.layer));

                Vector3 pointHit = rayHit.point;
                if (!rayHasHit)
                {
                    pointHit = aimRay.GetPoint(projectileRange);
                }

                GameObject projectile = Instantiate(ProjectilePrefab, transform.position + (MainCameraController.MainCamera.transform.rotation * Vector3.forward), MainCameraController.MainCamera.transform.rotation);
                projectile.transform.LookAt(pointHit);
                projectile.GetComponent<ProjectileBehaviour>().SetInitialValues(1 << gameObject.layer);
                fireCoolDown = FireRate;
                ammoNumber.text = Ammo.ToString();
                EventCoordinator.CurrentEventCoordinator.ActivateEvent(new PlayerDiegeticSoundEventInfo(gameObject, ShootSoundRange, GunShotSound));
            }
        }
        else
        {
            MainCameraController.StopAiming();
        }

        fireCoolDown -= getPlayerDeltaTime();
    }

    /// <summary>
    /// Plays a one shot diegetic playersound and checks if any enemies are within range to hear it.
    /// </summary>
    /// <param name="eI"><see cref="PlayerDiegeticSoundEventInfo"/> representing the player, also needs an audioclip. If the range is more than zero enemies might hear the player.</param>
    public void PlayerDiegeticSound(EventInfo eI)
    {
        PlayerDiegeticSoundEventInfo playerSound = (PlayerDiegeticSoundEventInfo)eI;

        if (playerSound.AC != null)
            playerSound.GO.GetComponent<AudioSource>().PlayOneShot(playerSound.AC);
    }
}
