using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Stunbot/Chase State")]
public class StunbotChaseState : StunbotBaseState
{
    private bool foundPath = false;

    public override void Enter()
    {
        base.Enter();
        FindTarget();
        foundPath = true;
    }

    public override void HandleUpdate()
    {
        //raycasta för att kolla om man behöver räkna ut en ny väg
        RaycastHit hit = new RaycastHit();
        LayerMask lm = owner.playerLayer | owner.EnviromentLayer;
        if ((!foundPath) && Physics.SphereCast(owner.transform.position, owner.thisCollider.radius, (PlayerTransform.position - ThisTransform.position).normalized, out hit, Mathf.Infinity, lm) && hit.transform.GetComponent<PlayerStateMachine>() == null)
        {
            foundPath = true;
            FindTarget();
        }

        FlyToTarget(NextTargetPosition);

        Vector3 plannedMovement = Velocity * Time.deltaTime;

        RaycastHit playerRayHit;

        bool hitPlayer = Physics.SphereCast(ThisTransform.position, ThisCollider.radius, plannedMovement.normalized, out playerRayHit, plannedMovement.magnitude, owner.playerLayer);

        if (hitPlayer)
        {
            PlayerTransform.GetComponent<PlayerStateMachine>().TakeDamage(3.0f);
            //Accelerate(-direction.normalized * MaxSpeed * 2); // replace this with the bounce from base state
            Velocity = Velocity.normalized * MaxSpeed;
        }

        base.HandleUpdate();

        if (!CanFindOrigin())
        {
            Debug.Log("Chase -> Idle (can't see origin)");
            owner.TransitionTo<StunbotIdleState>();
        }
        else if (!CanSeePlayer(65.0f))
        {
            Debug.Log("Chase -> Search (lost player)");
            owner.TransitionTo<StunbotSearchState>();
        }
        else
        {
            owner.lastPlayerLocation = PlayerTransform.position;
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

    protected override void NoTargetAvailable()
    {
        NextTargetPosition = PlayerTransform.position;
        foundPath = false;
    }
}
