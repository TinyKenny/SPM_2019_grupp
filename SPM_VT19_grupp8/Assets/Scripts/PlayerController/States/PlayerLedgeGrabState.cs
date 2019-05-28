using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Ledge Grab State")]
public class PlayerLedgeGrabState : PlayerBaseState
{
    private float climbUpSpeed = 12.5f;
    private float climbForwardSpeed = 25f;
    private float climbForwardLength = 0.6f;
    private Vector3 forwardClimbPosition;
    private Vector3 wallNormal;
    private bool grabable;

    public override void Enter()
    {
        Velocity = Vector3.zero;
        Animator.SetBool("LedgeGrab", true);
        FindCollision(Transform.forward, out RaycastHit hit, SkinWidth * 5);
        wallNormal = hit.normal;

        Transform.LookAt(Transform.position + (-wallNormal * 5));
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        ClimbLedge();
    }

    private void ClimbLedge()
    {
        RaycastHit wallCheckHit;

        bool climbUp = FindCollision(Transform.forward, out wallCheckHit, SkinWidth * 5);

        if (wallCheckHit.collider != null)
            grabable = wallCheckHit.transform.CompareTag("Grabable");

        if (climbUp && grabable)
        {
            forwardClimbPosition = Vector3.zero;
            Vector3 bottomPoint = Transform.position + ThisCollider.center - Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
            Vector3 topPoint = Transform.position + ThisCollider.center + Transform.up * (ThisCollider.height / 2 + ThisCollider.radius);
            float wallTop = wallCheckHit.collider.bounds.max.y;

            if (bottomPoint.y < wallTop + SkinWidth * 10 && topPoint.y + ThisCollider.height / 4 > wallTop)
            {
                Velocity += Transform.up * climbUpSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (forwardClimbPosition.Equals(Vector3.zero))
            {
                forwardClimbPosition = Transform.position;
            }

            float distanceForward = Vector3.Distance(forwardClimbPosition, Transform.position);

            ClimbForward();

            Vector3 topPoint = Transform.position + ThisCollider.center + Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
            Vector3 bottomPoint = Transform.position + ThisCollider.center - Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
            
            if ((Physics.CapsuleCast(topPoint, bottomPoint, ThisCollider.radius, Vector3.down, SkinWidth * 12, CollisionLayers) && distanceForward > 0.2f) || grabable == false)
            {
                Owner.TransitionTo<PlayerAirState>();
            }
        }

        CheckCollision(Velocity * PlayerDeltaTime);
    }

    private void ClimbForward()
    {
        Velocity -= Vector3.ProjectOnPlane(Velocity, Transform.forward);
        Velocity += Transform.forward * climbForwardSpeed * Time.deltaTime;
    }

    protected override void UpdatePlayerRotation()
    {

    }

    public override void Exit()
    {
        base.Exit();
        Animator.SetBool("LedgeGrab", false);
    }
}
