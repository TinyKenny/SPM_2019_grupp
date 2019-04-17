using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Ledge Grab State")]
public class PlayerLedgeGrabState : PlayerBaseState
{
    private float jumpPower = 5.0f;

    public override void Enter()
    {
        Camera.main.GetComponent<CameraController>().StopAiming();
        Velocity = Vector3.zero;
    }

    public override void HandleUpdate()
    {
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
                Velocity += wallCheckHit.normal * jumpPower;
            }

            if (Input.GetAxisRaw("Vertical") > 0.9)
            {
                Vector3 bottomPoint = Transform.position + ThisCollider.center - Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
                if (bottomPoint.y < wallCheckHit.collider.bounds.max.y)
                    Transform.position += Transform.up * Input.GetAxisRaw("Vertical") * PlayerDeltaTime;
                else
                {
                    Velocity += (Transform.up + Transform.forward).normalized * 350 * PlayerDeltaTime;
                    owner.TransitionTo<PlayerAirState>();
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1)
            {
                Transform.position += Transform.right * Input.GetAxisRaw("Horizontal") * PlayerDeltaTime;
            }
        }
        else
            owner.TransitionTo<PlayerAirState>();
    }
}
