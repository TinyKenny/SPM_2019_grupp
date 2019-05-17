using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Vertical Wall Run State")]
public class PlayerVerticalWallRunState : PlayerAirState
{
    private float maxVerticalVelocity = 10f;
    private Vector3 wallNormal;

    public override void Enter()
    {
        RaycastHit wall = new RaycastHit();

        WallRun(out wall);

        wallNormal = wall.normal;

        Velocity = ProjectSpeedOnSurface();

        Transform.LookAt(Transform.position - wallNormal);
    }

    public override void HandleUpdate()
    {
        Velocity += Vector3.down * Gravity * PlayerDeltaTime;

        if (Velocity.magnitude > maxVerticalVelocity)
        {
            Velocity = Velocity.normalized * maxVerticalVelocity;
        }

        CheckCollision(Velocity * PlayerDeltaTime);

        bool grounded = GroundCheck();

        Velocity *= Mathf.Pow(AirResistanceCoefficient, PlayerDeltaTime);

        RaycastHit wall = new RaycastHit(); float soundDistance = (Velocity.magnitude / Owner.MaxSpeed) * MovementSoundRange;
        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new PlayerDiegeticSoundEventInfo(Transform.gameObject, soundDistance));

        if (grounded)
        {
            TransitionToWalkingState();
        }
        else if (WallRun(out wall) && Velocity.y > MinimumYVelocity && Input.GetButton("Wallrun"))
        {
            LedgeGrabCheck();



            Jump(wallNormal);
        }
        else
        {
            Owner.TransitionTo<PlayerAirState>();
        }
    }

    private Vector3 ProjectSpeedOnSurface()
    {
        Vector3 projection = Vector3.Dot(Velocity, wallNormal) * wallNormal;
        Vector3 tempVelocity = Velocity - projection;
        Vector3 magnitude = projection.magnitude * tempVelocity.normalized;
        magnitude = Vector3.ClampMagnitude(magnitude, Velocity.magnitude);

        return magnitude;
    }

    public override void Exit()
    {
        base.Exit();
        Animator.SetBool("WallRunning", false);
    }
}
