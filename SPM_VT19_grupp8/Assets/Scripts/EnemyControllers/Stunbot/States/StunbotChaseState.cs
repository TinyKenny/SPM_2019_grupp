using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Chase State")]
public class StunbotChaseState : StunbotBaseState
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void HandleUpdate()
    {
        Vector3 direction = (PlayerTransform.position - ThisTransform.position).normalized * Acceleration * Time.deltaTime;
        Accelerate(direction);

        Vector3 plannedMovement = Velocity * Time.deltaTime;

        RaycastHit playerRayHit;

        bool hitPlayer = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, plannedMovement.normalized, out playerRayHit, plannedMovement.magnitude, owner.playerLayer);

        if (hitPlayer)
        {
            PlayerTransform.GetComponent<PlayerStateMachine>().TakeDamage(3.0f);
            Accelerate(-direction.normalized * MaxSpeed * 2);
        }

        ApplyMovement();

        base.HandleUpdate();

        if (!CanSeePlayer())
        {
            Debug.Log("Stop chasing player!");
            owner.TransitionTo<StunbotIdleState>();
        }
    }

    private void Accelerate(Vector3 accelerationVector)
    {
        Velocity = Vector3.ClampMagnitude(Velocity + accelerationVector, MaxSpeed);
    }

    
    private void ApplyMovement()
    {
        Vector3 movement = Velocity * Time.deltaTime;
        RaycastHit rayHit;

        bool rayHasHit = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, movement.normalized, out rayHit, Mathf.Infinity, owner.visionMask);

        if (rayHasHit)
        {
            Vector3 hitNormal = rayHit.normal;

            float angle = (Vector3.Angle(hitNormal, movement.normalized) - 90) * Mathf.Deg2Rad;
            float snapDistanceFromHit = SkinWidth / Mathf.Sin(angle);

            Vector3 snapMovement = movement.normalized * (rayHit.distance - snapDistanceFromHit);

            if (movement.magnitude > snapMovement.magnitude)
            {
                Velocity = Vector3.zero;
            }

            snapMovement = Vector3.ClampMagnitude(snapMovement, movement.magnitude);

            movement = snapMovement;
        }

        ThisTransform.position += movement;
    }
}
