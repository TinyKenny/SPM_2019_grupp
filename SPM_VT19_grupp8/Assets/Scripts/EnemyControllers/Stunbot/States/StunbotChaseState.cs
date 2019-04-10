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

        //ApplyMovement();

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
}
