using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Ledge Grab State")]
public class PlayerLedgeGrabState : PlayerBaseState
{
    private float jumpPower = 12.5f;
    private bool climbed;

    public override void Enter()
    {
        climbed = false;
        Camera.main.GetComponent<CameraController>().StopAiming();
        Velocity = Vector3.zero;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        MovementInput();
    }

    private void MovementInput()
    {
        RaycastHit wallCheckHit;

        bool wallHit = FindCollision(Transform.forward, out wallCheckHit, SkinWidth * 5);
        if (wallHit)
        {
            if (Input.GetButtonDown("Jump"))
            {
                Velocity = wallCheckHit.normal * jumpPower;
                owner.TransitionTo<PlayerAirState>();
            }

            if (Input.GetAxisRaw("Vertical") > 0.9)
            {

                Vector3 bottomPoint = Transform.position + ThisCollider.center - Transform.up * (ThisCollider.height / 2 + ThisCollider.radius);
                if (bottomPoint.y < (wallCheckHit.collider.bounds.max.y + SkinWidth))
                {
                    //Transform.position += Transform.forward * 2 + Vector3.up * (wallCheckHit.collider.bounds.max.y - (Transform.position.y - ThisCollider.bounds.extents.y) + SkinWidth);
                    Transform.position += Transform.up * 2 * owner.getPlayerDeltaTime();
                }
                //Transform.position += Transform.up * 1 * PlayerDeltaTime;
                else
                {
                    Velocity += (Transform.up + Transform.forward).normalized * MaxSpeed * PlayerDeltaTime;
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1)
            {
                Transform.position += Transform.right * Input.GetAxisRaw("Horizontal") * PlayerDeltaTime;
            }
            else if (Input.GetAxisRaw("Vertical") < -0.9)
            {
                Vector3 bottomPoint = Transform.position + ThisCollider.center - Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
                Vector3 topPoint = Transform.position + ThisCollider.center + Transform.up * (ThisCollider.height / 2 + ThisCollider.radius);
                if (bottomPoint.y < (wallCheckHit.collider.bounds.max.y + SkinWidth) && topPoint.y > (wallCheckHit.collider.bounds.max.y + SkinWidth))
                {
                    //Transform.position += Transform.forward * 2 + Vector3.up * (wallCheckHit.collider.bounds.max.y - (Transform.position.y - ThisCollider.bounds.extents.y) + SkinWidth);
                    Transform.position -= Transform.up * 2 * owner.getPlayerDeltaTime();
                }
            }
        }
        else
        {
            if (!climbed)
            {
                climbed = true;
                Transform.position += Transform.up * 2* owner.getPlayerDeltaTime();
            }
            Transform.position += Transform.forward * 2 * PlayerDeltaTime;

            if (GroundCheck())
                owner.TransitionTo<PlayerAirState>();
        }
    }

    protected override void UpdatePlayerRotation()
    {

    }
}
