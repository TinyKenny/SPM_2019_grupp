using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Booped State")]
public class StunbotBoopedState : StunbotBaseState
{
    private LayerMask PlayerLayer { get { return Owner.PlayerLayer; } }
    private float Deceleration { get { return Owner.Deceleration; } }
    private float SkinWidth { get { return Owner.SkinWidth; } }
    public float BoopStrength { get { return Owner.oopStrength; } }

    //private float boopMultiplier = 1.5f;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        LastPlayerLocation = PlayerTransform.position;
        Velocity = Velocity.normalized * Speed * boopMultiplier;
    }

    public override void HandleUpdate()
    {
        Decelerate();
        ApplyMovement(Velocity * Time.deltaTime);

        if(Velocity.sqrMagnitude <= Speed * Speed * MathHelper.floatEpsilon)
        {
            if (CanSeePlayer(60.0f))
            {
                Owner.TransitionTo<StunbotChaseState>();
            }
            else
            {
                Owner.TransitionTo<StunbotSearchState>();
            }
        }
    }

    /// <summary>
    /// Attempts to apply the specified movement to the stunbot, while obeying the physics-simulation rules of the stunbot.
    /// The physics-simulation rules of the stunbot is that if it collides with anything during its movement, it will bounce off of that object.
    /// </summary>
    /// <param name="movement">The desired movement</param>
    protected void ApplyMovement(Vector3 movement)
    {
        RaycastHit rayHit;

        bool rayHasHit = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, movement.normalized, out rayHit, Mathf.Infinity, (VisionMask | PlayerLayer | (1 << ThisTransform.gameObject.layer)));

        if (rayHasHit)
        {
            Vector3 hitNormal = rayHit.normal;

            float angle = (Vector3.Angle(hitNormal, movement.normalized) - 90) * Mathf.Deg2Rad;
            float snapDistanceFromHit = SkinWidth / Mathf.Sin(angle);

            Vector3 snapMovement = movement.normalized * (rayHit.distance - snapDistanceFromHit);
            snapMovement = Vector3.ClampMagnitude(snapMovement, movement.magnitude);

            movement -= snapMovement;

            ThisTransform.position += snapMovement;

            if (movement.magnitude > 0.01f)
            {
                Vector3 reflectDirection = Vector3.Reflect(Velocity.normalized, hitNormal);

                movement = reflectDirection * movement.magnitude;
                Velocity = reflectDirection * Velocity.magnitude;

                StunbotStateMachine otherStunBot = rayHit.transform.GetComponent<StunbotStateMachine>();

                if (otherStunBot != null)
                {
                    Vector3 otherReflectDirection = Vector3.Reflect(otherStunBot.Velocity.normalized, -hitNormal);
                    otherStunBot.Velocity = otherReflectDirection * otherStunBot.Velocity.magnitude;
                }
                ApplyMovement(movement);
            }
        }
        else
        {
            ThisTransform.position += movement;
        }
    }

    /// <summary>
    /// Linearly decelerates, untill the magnitude of Velocity zero.
    /// </summary>
    protected void Decelerate()
    {
        Vector3 decelerationvector = Velocity.normalized * Deceleration * Time.deltaTime;

        if (decelerationvector.magnitude > Velocity.magnitude)
        {
            Velocity = Vector3.zero;
        }
        else
        {
            Velocity -= decelerationvector;
        }
    }
}
