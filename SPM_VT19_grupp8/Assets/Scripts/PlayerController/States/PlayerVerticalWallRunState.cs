using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Vertical Wall Run State")]
public class PlayerVerticalWallRunState : PlayerAirState
{
    private Vector3 wallNormal;

    public override void Enter()
    {
        RaycastHit wall = new RaycastHit();

        WallRun(out wall);

        wallNormal = wall.normal;

        ProjectSpeedOnSurface();

        Transform.LookAt(Transform.position - wallNormal);
    }

    public override void HandleUpdate()
    {
        Velocity += Vector3.down * Gravity * PlayerDeltaTime;

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

    private void ProjectSpeedOnSurface()
    {
        Vector3 projection = Vector3.Dot(Velocity, wallNormal) * wallNormal;
        Vector3 tempVelocity = Velocity - projection;
        Vector3 magnitude = (projection.magnitude + tempVelocity.magnitude) * tempVelocity.normalized;
        Velocity = Vector3.ClampMagnitude(magnitude, Velocity.magnitude);
    }

    public override void Exit()
    {
        base.Exit();
        Animator.SetBool("WallRunning", false);
    }
}
