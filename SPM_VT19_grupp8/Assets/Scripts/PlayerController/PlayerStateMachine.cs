using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public LayerMask collisionLayers;
    public PhysicsComponent physicsComponent;
    public CapsuleCollider thisCollider;
    public float skinWidth = 0.01f;
    public float groundCheckDistance = 0.01f;

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

    protected override void Awake()
    {
        base.Awake();
        physicsComponent = GetComponent<PhysicsComponent>();
        thisCollider = GetComponent<CapsuleCollider>();
        standardColliderHeight = thisCollider.height;

        Debug.Log(Time.timeScale);

        playerTimeScale = 1.0f;
        slowedPlayerTimeScale = 0.5f;
        slowedWorldTimeScale = 0.2f;
    }

    protected override void Update()
    {
        base.Update();
        UpdatePlayerRotation();
        if (Input.GetButtonDown("TimeSlowToggle"))
        {
            if(Mathf.Approximately(playerTimeScale, 1.0f))
            {
                Time.timeScale = slowedWorldTimeScale;
                playerTimeScale = slowedPlayerTimeScale;
            } else
            {
                Time.timeScale = 1.0f;
                playerTimeScale = 1.0f;
            }
        }
        Debug.Log(Time.timeScale);
    }

    /// <summary>
    /// Returns Time.unscaledDeltaTime multiplied by the players personal timescale.
    /// </summary>
    /// <returns>The players deltaTime</returns>
    public float getPlayerDeltaTime()
    {
        return Time.unscaledDeltaTime * playerTimeScale;
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
}
