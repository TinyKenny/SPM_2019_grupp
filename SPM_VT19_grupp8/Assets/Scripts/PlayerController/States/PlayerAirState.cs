using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Air State")]
public class PlayerAirState : PlayerBaseState
{
    private Vector3 direction;
    protected float MinimumYVelocity = -10f;
    protected float maxYVelocity = 12.5f;
    protected static float jumpPower = 12.5f;

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        RaycastHit wallRunCheck = new RaycastHit();

        Shoot();

        MovementInput();

        Velocity += (Vector3.down * Gravity) * PlayerDeltaTime;

        //Kolla om framåt för spelaren och direction från input är typ samma i så fall lägg till mindre fart så man inte superaccelererar i luften.
        Velocity += (direction * Gravity) * PlayerDeltaTime;

        CheckCollision(Velocity * PlayerDeltaTime);

        bool grounded = FindCollision(Vector3.down, GroundCheckDistance + SkinWidth);

        Velocity *= Mathf.Pow(AirResistanceCoefficient, PlayerDeltaTime);

        if (Velocity.y > maxYVelocity)
        {
            Velocity = new Vector3(Velocity.x, maxYVelocity, Velocity.z);
        }

        //if (Velocity.magnitude > MaxSpeed * 2)
        //{
        //    Velocity = Velocity.normalized * MaxSpeed;
        //}

        if (grounded)
        {
            TransitionToWalkingState();
        }
        else if (WallRun(out wallRunCheck))
        {
            Jump(wallRunCheck.normal);
            LedgeGrabCheck();

            if (Input.GetButton("Wallrun") && Velocity.y > MinimumYVelocity && wallRunCheck.normal.y > -0.5f && owner.WallrunAllowed() && Mathf.Abs(Vector3.Dot(wallRunCheck.normal, Vector3.up)) < MathHelper.floatEpsilon)
            {
                if (Mathf.Abs(Vector3.Angle(Transform.forward, wallRunCheck.normal)) > 140)
                {
                    owner.ResetWallrunCooldown();
                    owner.TransitionTo<PlayerVerticalWallRunState>();
                }
                else
                {
                    owner.ResetWallrunCooldown();
                    owner.TransitionTo<PlayerWallRunState>();
                }
            }
        }
    }

    protected void LedgeGrabCheck()
    {
        RaycastHit wall;
        Vector3 topPoint = Transform.position + ThisCollider.center + Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
        bool b = Physics.BoxCast(topPoint, new Vector3(ThisCollider.radius / 2, ThisCollider.height / 4, ThisCollider.radius / 2), Transform.forward, out wall, Transform.rotation, ThisCollider.radius + SkinWidth * 10, CollisionLayers) && wall.transform.tag == "Grabable";
        if (b && wall.collider.bounds.max.y < ThisCollider.bounds.max.y)
            owner.TransitionTo<PlayerLedgeGrabState>();
    }

    protected bool WallRun()
    {
        return FindCollision(Transform.forward, SkinWidth * 2);
    }

    protected bool WallRun(out RaycastHit wall)
    {
        bool wallFound = FindCollision(Transform.forward, out wall, SkinWidth * 2) || FindCollision(Transform.right, out wall, SkinWidth * 2) || FindCollision(-Transform.right, out wall, SkinWidth * 2);
        return wallFound;
    }

    protected override void HandleCollition(Vector3 hitNormal, RaycastHit raycastHit)
    {

    }

    private void MovementInput()
    {
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));

        direction = Camera.main.transform.rotation * direction;

        direction = Vector3.ProjectOnPlane(direction, Vector3.up);
    }

    protected void Jump(Vector3 normal)
    {

        if (Input.GetButtonDown("Jump"))
        {
            Velocity += (normal + Vector3.up + direction).normalized * (jumpPower * owner.TimeSlowMultiplier);
            jumpPower *= 0.5f;
        }
    }

    protected override void UpdatePlayerRotation()
    {
        if (Time.timeScale > 0)
        {
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
            direction = Camera.main.transform.rotation * direction;

            // project on plane?

            Transform.LookAt(Transform.position + new Vector3(direction.x, 0.0f, direction.z).normalized);
        }
    }

    protected void TransitionToWalkingState()
    {
        jumpPower = 12.5f;
        owner.TransitionTo<PlayerWalkingState>();
    }
}
