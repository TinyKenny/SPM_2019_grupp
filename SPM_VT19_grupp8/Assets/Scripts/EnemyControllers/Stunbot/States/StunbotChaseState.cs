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

        // (start) rotate toward direction
        owner.faceDirection += direction.normalized * 5.0f * Time.deltaTime;
        if (owner.faceDirection.magnitude > 1)
        {
            owner.faceDirection = owner.faceDirection.normalized;
        }
        ThisTransform.LookAt(ThisTransform.position + owner.faceDirection);
        // (end) rotate toward direction


        if (Vector3.Dot(ThisTransform.forward, (PlayerTransform.position - ThisTransform.position).normalized) > 0.5)
        {
            Accelerate(direction);
        }
        

        Vector3 plannedMovement = Velocity * Time.deltaTime;

        RaycastHit playerRayHit;

        bool hitPlayer = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, plannedMovement.normalized, out playerRayHit, plannedMovement.magnitude, owner.playerLayer);

        if (hitPlayer)
        {
            PlayerTransform.GetComponent<PlayerStateMachine>().TakeDamage(3.0f);
            Accelerate(-direction.normalized * MaxSpeed * 2);
        }

        //ApplyMovement();

        base.HandleUpdate();

        if (!CanSeePlayer(21.0f))
        {
            Debug.Log("Stop chasing player!");
            owner.TransitionTo<StunbotIdleState>();
        }
    }

    private void Accelerate(Vector3 accelerationVector)
    {
        /*
        Vector3 newVelocity = Velocity + accelerationVector;

        if(newVelocity.magnitude > MaxSpeed)
        {
            float magPercent = Velocity.magnitude / newVelocity.magnitude;
            Velocity *= magPercent;
        }

        Velocity += accelerationVector;
        */
        Velocity = Vector3.ClampMagnitude(Velocity + accelerationVector, MaxSpeed);
    }
}
