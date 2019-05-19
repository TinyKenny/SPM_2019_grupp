using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Air State")]
public class PlayerAirState : PlayerBaseState
{
    private Vector3 direction;
    protected float MinimumYVelocity = -10f;
    protected float maxYVelocity = 12.5f;
    protected static float jumpPower = 12.5f; // get rid of this somehow?

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        RaycastHit wallRunCheck = new RaycastHit();

        Shoot();

        MovementInput();

        Velocity += (Vector3.down * Gravity) * PlayerDeltaTime;

        //Sänk dot om man vill påverka mindre, kanske ha en koefficient som man gångrar med för att minska värdet på dot så man kan åka framåt mer?
        float dot = Vector3.Dot(Velocity.normalized, direction);
        float directionForce = Gravity - (Gravity * dot);

        if (directionForce < 0)
            directionForce = Gravity;

        Velocity += (direction * directionForce) * PlayerDeltaTime;

        CheckCollision(Velocity * PlayerDeltaTime);

        bool grounded = GroundCheck();

        Velocity *= Mathf.Pow(AirResistanceCoefficient, PlayerDeltaTime);

        if (Velocity.y > maxYVelocity)
        {
            Velocity = new Vector3(Velocity.x, maxYVelocity, Velocity.z);
        }

        if (grounded)
        {
            TransitionToWalkingState();
        }
        else if (WallRun(out wallRunCheck))
        {
            Jump(wallRunCheck.normal);
            LedgeGrabCheck();

            if (Input.GetButton("Wallrun") && Velocity.y > MinimumYVelocity && wallRunCheck.normal.y > -0.5f && Owner.WallrunAllowed()/* && Mathf.Abs(Vector3.Dot(wallRunCheck.normal, Vector3.up)) < MathHelper.floatEpsilon*/)
            {
                Animator.SetBool("WallRunning", true);
                if (Mathf.Abs(Vector3.Angle(Transform.forward, wallRunCheck.normal)) > 160)
                {
                    Owner.ResetWallrunCooldown();
                    Owner.TransitionTo<PlayerVerticalWallRunState>();
                }
                else
                {
                    Owner.ResetWallrunCooldown();
                    Owner.TransitionTo<PlayerWallRunState>();
                }
            }
        }
    }

    protected void LedgeGrabCheck()
    {
        RaycastHit wall;
        Vector3 topPoint = Transform.position + ThisCollider.center + Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
        bool b = Physics.BoxCast(topPoint, new Vector3(ThisCollider.radius / 2, ThisCollider.height / 4, ThisCollider.radius / 2), Transform.forward, out wall, Transform.rotation, ThisCollider.radius + SkinWidth * 10, CollisionLayers) && wall.transform.CompareTag("Grabable");
        if (b && wall.collider.bounds.max.y < ThisCollider.bounds.max.y)
            Owner.TransitionTo<PlayerLedgeGrabState>();
    }

    protected bool WallRun()
    {
        return FindCollision(Transform.forward, SkinWidth * 2);
    }

    protected bool WallRun(out RaycastHit wall)
    {
        bool wallFound = FindCollision(Transform.forward, out wall, SkinWidth * 10) || FindCollision(Transform.right, out wall, SkinWidth * 10) || FindCollision(-Transform.right, out wall, SkinWidth * 10);
        return wallFound;
    }

    protected override void HandleCollition(Vector3 hitNormal, RaycastHit raycastHit)
    {

    }

    private void MovementInput()
    {
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        direction = MainCameraController.transform.rotation * direction;
        direction = Vector3.ProjectOnPlane(direction, Vector3.up);
    }

    protected void Jump(Vector3 normal)
    {

        if (Input.GetButtonDown("Jump"))
        {
            Velocity = Vector3.Slerp(Vector3.ClampMagnitude(Velocity, jumpPower), (normal + Vector3.up) * jumpPower, 0.4f);

            jumpPower *= 0.5f;
            Animator.SetTrigger("Jump");
            Owner.TransitionTo<PlayerAirState>();
        }
    }

    protected override void UpdatePlayerRotation()
    {
        if (Time.timeScale > 0)
        {
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
            direction = MainCameraController.transform.rotation * direction;

            Transform.LookAt(Transform.position + new Vector3(direction.x, 0.0f, direction.z).normalized);
        }
    }

    protected void TransitionToWalkingState()
    {
        jumpPower = 12.5f;
        Owner.TransitionTo<PlayerWalkingState>();
    }
}
