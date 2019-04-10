using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateMachine : StateMachine
{
    public LayerMask collisionLayers;
    public PhysicsComponent physicsComponent;
    public CapsuleCollider thisCollider;
    public float skinWidth = 0.01f;
    public float groundCheckDistance = 0.01f;
    public GameObject projectilePrefab;
    public float fireRate = 1.0f;
    public float fireCoolDown = 0.0f;

    [HideInInspector]
    public float standardColliderHeight;

    public float Acceleration { get { return physicsComponent.acceleration; } set { physicsComponent.acceleration = value; } }
    public float Deceleration { get { return physicsComponent.deceleration; } set { physicsComponent.deceleration = value; } }
    public float MaxSpeed { get { return physicsComponent.maxSpeed; } set { physicsComponent.maxSpeed = value; } }
    public float FrictionCoefficient { get { return physicsComponent.frictionCoefficient; } set { physicsComponent.frictionCoefficient = value; } }
    public float AirResistanceCoefficient { get { return physicsComponent.airResistanceCoefficient; } set { physicsComponent.airResistanceCoefficient = value; } }
    public float Gravity { get { return physicsComponent.gravity; } set { physicsComponent.gravity = value; } }
    public Vector3 Velocity { get { return physicsComponent.velocity; } set { physicsComponent.velocity = value; } }

    private float playerTimeScale;
    private float slowedPlayerTimeScale;
    private float slowedWorldTimeScale;
    public float currentSlowMotionEnergy = 5.0f; // make this private
    public float slowMotionEnergyMax = 5.0f; // make this private
    public float slowMotionEnergyRegeneration = 1.0f; // make this private
    public float currentShields = 10.0f; // make this private
    public float shieldsMax = 10.0f; // make this private
    public float shieldsRegeneration = 1.0f; // make this private
    public float shieldsRegenerationCooldown = 4.0f; // make this private
    public float shieldsRegenerationTimer = 0.0f;
    public int ammo = 0;
    public Text ammoNumber;
    public Slider timeSlowEnergy;
    public Slider shieldAmount;
    public Transform respawnPoint;

    protected override void Awake()
    {
        base.Awake();
        physicsComponent = GetComponent<PhysicsComponent>();
        thisCollider = GetComponent<CapsuleCollider>();
        standardColliderHeight = thisCollider.height;

        Respawn();

        playerTimeScale = 1.0f;
        slowedPlayerTimeScale = 0.5f;
        slowedWorldTimeScale = 0.2f;
        timeSlowEnergy.maxValue = slowMotionEnergyMax;
        shieldAmount.maxValue = shieldsMax;
    }

    protected override void Update()
    {
        base.Update();
        UpdatePlayerRotation();

        // developer cheats start here

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ammo = 999;
            ammoNumber.text = ammo.ToString();
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Respawn();
        }


        // developer cheats end here

        shieldAmount.value = currentShields;

        if (shieldsRegenerationTimer <= 0.0f)
        {
            //currentShields += shieldsRegeneration * Time.deltaTime;
            currentShields = Mathf.Clamp(currentShields + shieldsRegeneration * Time.deltaTime, 0.0f, shieldsMax);
        }
        else
        {
            shieldsRegenerationTimer -= Time.deltaTime;
        }

        if (Mathf.Approximately(playerTimeScale, 1.0f))
        {
            //currentSlowMotionEnergy += slowMotionEnergyRegeneration * Time.deltaTime;
            currentSlowMotionEnergy = Mathf.Clamp(currentSlowMotionEnergy + slowMotionEnergyRegeneration * Time.deltaTime, 0.0f, slowMotionEnergyMax);
            timeSlowEnergy.value = currentSlowMotionEnergy;

            if (Input.GetButtonDown("TimeSlowToggle") && currentSlowMotionEnergy >= 1.0f)
            {
                Time.timeScale = slowedWorldTimeScale;
                playerTimeScale = slowedPlayerTimeScale;
            }
        }
        else
        {
            //currentSlowMotionEnergy -= Time.unscaledDeltaTime;
            currentSlowMotionEnergy = Mathf.Clamp(currentSlowMotionEnergy - Time.unscaledDeltaTime, 0.0f, slowMotionEnergyMax);
            timeSlowEnergy.value = currentSlowMotionEnergy;

            if (Input.GetButtonDown("TimeSlowToggle") || currentSlowMotionEnergy <= MathHelper.floatEpsilon)
            {
                Time.timeScale = 1.0f;
                playerTimeScale = 1.0f;
            }
        }
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
            Debug.Log("Player took fatal damage");
            Respawn();
        }
        else
        {
            currentShields = Mathf.Clamp(currentShields - damage, 0.0f, shieldsMax);
            shieldsRegenerationTimer = shieldsRegenerationCooldown;
        }
    }

    /// <summary>
    /// Transitions the player to PlayerAirState and sets the players position to the respawn point.
    /// Resets variables related to life, slow-mo, attacking, ect. (example: deactivating slow-mo and refilling shields)
    /// </summary>
    private void Respawn()
    {
        TransitionTo<PlayerAirState>();
        transform.position = respawnPoint.position;
        Velocity = Vector3.zero;
        playerTimeScale = 1.0f;
        shieldsRegenerationTimer = 0.0f;
        currentShields = shieldsMax;
        currentSlowMotionEnergy = slowMotionEnergyMax;
        fireCoolDown = 0.0f;

        ammo = 0;
        ammoNumber.text = ammo.ToString();

        Debug.Log("Reset pickups and enemies when respawning!");
    }

    /// <summary>
    /// Rotates the player-GameObject so that its Z-axis (also known as "forward", example: transform.forward)
    /// to be pointing in the direction of Velocity.
    /// </summary>
    private void UpdatePlayerRotation()
    {
        // make this better later
        transform.LookAt(transform.position + new Vector3(Velocity.x, 0.0f, Velocity.z).normalized);
    }

    public void AddAmmo(int ammo)
    {
        this.ammo += ammo;
        ammoNumber.text = this.ammo.ToString();
    }
}
