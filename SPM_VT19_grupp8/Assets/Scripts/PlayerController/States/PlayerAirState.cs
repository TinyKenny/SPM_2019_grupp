using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Air State")]
public class PlayerAirState : PlayerBaseState
{
    private Vector3 direction;
    protected float MinimumYVelocity = -10f;
    protected static float JumpPowerAirState = 15f; // get rid of this somehow?
    private float forwardWallrunMagnitudeLimit = 8.0f;

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

        if (grounded)
        {
            TransitionToWalkingState();
        }
        else if (WallRun(out wallRunCheck))
        {
            Jump(wallRunCheck.normal);
            LedgeGrabCheck();

            if ((Input.GetKey(PrimaryWallrunKey) || Input.GetKey(SecondaryWallrunKey)) && Velocity.y > MinimumYVelocity && wallRunCheck.normal.y > -0.5f && Owner.WallrunAllowed())
            {
                Vector3 projectionOnForward = Vector3.ProjectOnPlane(Velocity, Transform.forward);
                float forwardMagnitude = (Velocity - projectionOnForward).magnitude;
                float angleFromWall = Vector3.SignedAngle(Transform.forward, -wallRunCheck.normal, Transform.up);
                if (Mathf.Abs(angleFromWall) < 30 && Vector3.Dot(wallRunCheck.normal, Transform.forward) < -0.8)
                {
                    Animator.SetFloat("WallDirection", 0);
                    Owner.ResetWallrunCooldown();
                    Owner.TransitionTo<PlayerVerticalWallRunState>();
                }
                else if (forwardMagnitude > forwardWallrunMagnitudeLimit)
                {
                    Animator.SetFloat("WallDirection", Mathf.Sign(angleFromWall));
                    Animator.SetBool("WallRunning", true);
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
        RaycastHit wall;
        return WallRun(out wall);
    }

    protected bool WallRun(out RaycastHit wall)
    {
        bool wallFound = FindCollision(Transform.forward, out wall, SkinWidth * 10) 
                      || FindCollision(Transform.right, out wall, SkinWidth * 10) 
                      || FindCollision(-Transform.right, out wall, SkinWidth * 10);

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

        if (Input.GetKeyDown(PrimaryJumpKey) || Input.GetKeyDown(SecondaryJumpKey))
        {
            Velocity = Vector3.Slerp(Vector3.ClampMagnitude(Velocity, JumpPowerAirState), (normal + Vector3.up) * JumpPowerAirState, 0.5f);

            JumpPowerAirState *= 0.5f;
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
        JumpPowerAirState = 15f;
        Owner.TransitionTo<PlayerWalkingState>();
    }
}
