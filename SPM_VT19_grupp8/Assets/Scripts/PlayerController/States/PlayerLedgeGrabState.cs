using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Ledge Grab State")]
public class PlayerLedgeGrabState : PlayerBaseState
{
    private float climbUpSpeed = 12.5f;
    private float climbForwardSpeed = 25f;
    private float climbForwardLength = 0.5f;
    private bool climbed;
    private Vector3 wallNormal;
    private Vector3 wallPoint;
    private Vector3 forwardClimbPosition;

    public override void Enter()
    {
        Debug.Log("Ledgegrabbing!");
        climbed = false;
        MainCameraController.StopAiming();
        Velocity = Vector3.zero;
        Animator.SetBool("LedgeGrab", true);

        RaycastHit hit;

        FindCollision(Transform.forward, out hit, SkinWidth * 5);

        wallNormal = hit.normal;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        ClimbLedge();
    }

    private void MovementInput()
    {
        
        //RaycastHit wallCheckHit;

        //bool wallHit = FindCollision(Transform.forward, out wallCheckHit, SkinWidth * 5);


        //if (wallHit && wallCheckHit.transform.CompareTag("Grabable"))
        //{
        //    wallPoint = wallCheckHit.point;
            
        //    if (Input.GetButtonDown("Jump"))
        //    {
        //        Velocity = wallNormal * jumpPower;
        //        Owner.TransitionTo<PlayerAirState>();
        //    }

        //    if (Input.GetAxisRaw("Vertical") > 0.9)
        //    {

        //        Vector3 bottomPoint = Transform.position + ThisCollider.center - Transform.up * (ThisCollider.height / 2 + ThisCollider.radius);
        //        if (bottomPoint.y < (wallCheckHit.collider.bounds.max.y + SkinWidth))
        //        {
        //            Transform.position += Transform.up * 2 * PlayerDeltaTime;
        //        }
        //        else
        //        {
        //            Transform.position += (Transform.up + Transform.forward).normalized * MaxSpeed * PlayerDeltaTime;
        //        }
        //    }
        //    else if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1)
        //    {
        //        Transform.position += Transform.right * Input.GetAxisRaw("Horizontal") * PlayerDeltaTime;
        //    }
        //    else if (Input.GetAxisRaw("Vertical") < -0.9)
        //    {
        //        Vector3 bottomPoint = Transform.position + ThisCollider.center - Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
        //        Vector3 topPoint = Transform.position + ThisCollider.center + Transform.up * (ThisCollider.height / 2 + ThisCollider.radius);
        //        if (bottomPoint.y < (wallCheckHit.collider.bounds.max.y + SkinWidth) && topPoint.y > (wallCheckHit.collider.bounds.max.y + SkinWidth))
        //        {
        //            Transform.position -= Transform.up * 2 * PlayerDeltaTime;
        //        }
        //    }
        //}
        //else
        //{
        //    Vector3 bottomPoint = Transform.position + ThisCollider.center - Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
        //    if (!climbed)
        //    {
        //        Transform.position += Transform.up * 2 * PlayerDeltaTime;
        //        if (wallPoint.y < bottomPoint.y)
        //            climbed = true;
        //    }
        //    else
        //    {
        //        if (wallCheckHit.collider == null && (Vector3.Dot((wallPoint - Transform.position).normalized, Transform.forward) < 0 || wallPoint.y < bottomPoint.y))
        //            Transform.position += Transform.forward * 2 * PlayerDeltaTime;
        //        else
        //            Owner.TransitionTo<PlayerAirState>();

        //        if (GroundCheck())
        //            Owner.TransitionTo<PlayerAirState>();
        //    }
        //}
    }

    private void ClimbLedge()
    {
        RaycastHit wallCheckHit;

        if (FindCollision(Transform.forward, out wallCheckHit, SkinWidth * 5))
        {

            Vector3 bottomPoint = Transform.position + ThisCollider.center - Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
            Vector3 topPoint = Transform.position + ThisCollider.center + Transform.up * (ThisCollider.height / 2 + ThisCollider.radius);
            float wallTop = wallCheckHit.collider.bounds.max.y + SkinWidth * 10;

            if (wallCheckHit.transform.CompareTag("Grabable") && bottomPoint.y < wallTop && topPoint.y + ThisCollider.height / 4 > wallTop)
            {
                Velocity += Transform.up * climbUpSpeed * Time.deltaTime;
            }
            else
            {
                forwardClimbPosition = Transform.position;
                ClimbForward();
            }
        }
        else
        {
            ClimbForward();

            if (Vector3.Distance(forwardClimbPosition, Transform.position) > climbForwardLength)
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
