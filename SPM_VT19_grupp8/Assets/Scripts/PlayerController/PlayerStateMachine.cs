﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is a statemachine that (together with its states) acts as the controller for the player character.
/// This class holds several variables whose values are not meant to be stored in the states themselves.
/// It also contains some behaviours that do not depend on which state the player is in, such as the slowmotion-ability.
/// </summary>
[RequireComponent(typeof(TimeController), typeof(PhysicsComponent), typeof(CapsuleCollider))]
public class PlayerStateMachine : StateMachine
{
    #region "chaining" properties
    public Vector3 Velocity { get { return physicsComponent.velocity; } set { physicsComponent.velocity = value; } }
    public float Acceleration { get { return physicsComponent.acceleration; } }
    public float Deceleration { get { return physicsComponent.deceleration; } }
    public float MaxSpeed { get { return physicsComponent.maxSpeed; } }
    public float AirResistanceCoefficient { get { return physicsComponent.airResistanceCoefficient; } }
    public float SkinWidth { get { return physicsComponent.skinWidth; } }
    public float Gravity { get { return physicsComponent.gravity / timeController.TimeSlowMultiplier; } }
    public float PlayerDeltaTime { get { return timeController.GetPlayerDeltaTime(); } } // optimize this?
    //public float TimeSlowMultiplier { get { return timeController.TimeSlowMultiplier; } } // change this to be gravity reduction?
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
    [SerializeField, Range(0.0f, 1.0f)] private float attackAngle = 0.25f;
    [SerializeField, Min(0.0f)] private float attackRange = 10.0f;
    [SerializeField, Range(0, 100)] private float movementSoundRange = 20.0f;
    [SerializeField, Range(0, 100)] private float shootSoundRange = 50;
    [SerializeField] private Text ammoNumber = null;
    [SerializeField] private AudioSource aus = null;
    [SerializeField] private AudioClip slowSound = null; // this is no longer played when time is slowed down
    [SerializeField] private AudioClip ammoSound = null;
    [SerializeField] private AudioClip damageSound = null;
    [SerializeField] private AudioClip deathSound = null;
    [SerializeField] private AudioClip gunShotSound = null;
    #endregion

    #region non-serialized private variables
    private PhysicsComponent physicsComponent = null;
    private TimeController timeController = null;
    private float currentShields = 10.0f;
    private float shieldsRegenerationTimer = 0.0f;
    private float wallrunCooldown;
    private float fireCoolDown = 0.0f; // currently has a public property with both get and set, do something about that
    private bool semiAutoAttackLock;
    #endregion

    #region readonly values
    //public readonly float skinWidth = 0.01f;
    public readonly float groundCheckDistance = 0.01f;
    #endregion

    public Transform respawnPoint; // make this private, create a new event type ("CheckpointReachedEventInfo", maybe?) and make a listener for that event type in PlayerStateMachine

    protected override void Awake()
    {
        physicsComponent = GetComponent<PhysicsComponent>();
        timeController = GetComponent<TimeController>();
        ThisCollider = GetComponent<CapsuleCollider>();
        Animator = GetComponentInChildren<Animator>();
        MainCameraController = Camera.main.GetComponent<CameraController>();
        StandardColliderHeight = ThisCollider.height;

        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<AmmoPickupEventInfo>(AddAmmo);
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerDiegeticSoundEventInfo>(PlayerDiegeticSound);

        base.Awake();

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

        RegenerateShields();
        wallrunCooldown -= PlayerDeltaTime;
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
    /// needs testing.
    /// 
    /// Calls <see cref="ResetValues"/> and then triggers a "player respawn"-event.
    /// </summary>
    public void Respawn()
    {
        ResetValues();

        PlayerRespawnEventInfo PREI = new PlayerRespawnEventInfo(gameObject);
        EventCoordinator.CurrentEventCoordinator.ActivateEvent(PREI);

        SaveFile.LoadSave();
    }

    /// <summary>
    /// Needs testing, make this compatible with TimeController
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
        currentShields = shieldsMax;
        fireCoolDown = 0.0f;
        Ammo = PlayerPrefs.GetInt("playerAmmo");
        ammoNumber.text = Ammo.ToString();
        timeController.ResetValues();
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

    public void Shoot()
    {
        if(Mathf.Approximately(Input.GetAxisRaw("Shoot"), 1.0f))
        {
            if(fireCoolDown < 0 && Ammo > 0 && Time.timeScale > 0 && semiAutoAttackLock == false)
            {
                PlayerAttackEventInfo pAEI = new PlayerAttackEventInfo(gameObject, transform.position, MainCameraController.transform.forward, attackAngle, attackRange);
                EventCoordinator.CurrentEventCoordinator.ActivateEvent(pAEI);

                Ammo--;
                ammoNumber.text = Ammo.ToString();
                fireCoolDown = fireRate;
                semiAutoAttackLock = true;
            }
        }
        else
        {
            semiAutoAttackLock = false;
        }


        //if (Input.GetAxisRaw("Aim") == 1f)
        //{
        //    //MainCameraController.Aiming();

        //    if (Input.GetAxisRaw("Shoot") == 1f && fireCoolDown < 0 && Ammo > 0 && Time.timeScale > 0)
        //    {
        //        Ammo--;

        //        Vector3 reticleLocation = new Vector3(MainCameraController.MainCamera.pixelWidth / 2, MainCameraController.MainCamera.pixelHeight / 2, 0.0f);

        //        Ray aimRay = MainCameraController.MainCamera.ScreenPointToRay(reticleLocation); // make it so that CameraController has a reference to its camera

        //        float projectileRange = ProjectilePrefab.GetComponent<ProjectileBehaviour>().distanceToTravel;

        //        bool rayHasHit = Physics.Raycast(aimRay, out RaycastHit rayHit, projectileRange, ~(1 << gameObject.layer));

        //        Vector3 pointHit = rayHit.point;
        //        if (!rayHasHit)
        //        {
        //            pointHit = aimRay.GetPoint(projectileRange);
        //        }

        //        GameObject projectile = Instantiate(ProjectilePrefab, transform.position + (MainCameraController.MainCamera.transform.rotation * Vector3.forward), MainCameraController.MainCamera.transform.rotation);
        //        projectile.transform.LookAt(pointHit);
        //        projectile.GetComponent<ProjectileBehaviour>().SetInitialValues(1 << gameObject.layer);
        //        fireCoolDown = FireRate;
        //        ammoNumber.text = Ammo.ToString();
        //        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new PlayerDiegeticSoundEventInfo(gameObject, ShootSoundRange, GunShotSound));
        //    }
        //}
        //else
        //{
        //    MainCameraController.StopAiming();
        //}

        fireCoolDown -= PlayerDeltaTime;
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

    public void TransitToAirState()
    {
        TransitionTo<PlayerAirState>();
    }
}
