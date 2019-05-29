using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Wall run State")]
public class PlayerWallRunState : PlayerAirState
{
    private Vector3 wallNormal;
    private float wallRunCooldown = 0.5f;
    private float currentCooldown;
    [Range(0f, 1f)]
    private float forwardArcAmount = 0.4f;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        MaxSpeedMod = 1f;
    }

    public override void Enter()
    {
        WallRun(out RaycastHit wall);

        wallNormal = wall.normal;

        Debug.Log(new Vector3(Velocity.x, 0.0f, Velocity.z).normalized);

        ProjectSpeedOnSurface();

        Transform.LookAt(Transform.position + new Vector3(Velocity.x, 0.0f, Velocity.z).normalized);

        Debug.Log(new Vector3(Velocity.x, 0.0f, Velocity.z).normalized);

        Velocity = Vector3.Slerp(Velocity, Transform.forward * Velocity.magnitude, forwardArcAmount);

        currentCooldown = wallRunCooldown;
    }

    public override void HandleUpdate()
    {
        Movement();

        CauseDiegeticSound();

        CheckStateChange();
    }

    private void CheckStateChange()
    {
        RaycastHit wall = new RaycastHit();

        if (GroundCheck())
        {
            TransitionToWalkingState();
        }
        else if (WallRun(out wall) && Velocity.y > MinimumYVelocity && (Input.GetKey(PrimaryWallrunKey) || Input.GetKey(SecondaryWallrunKey)))
        {
            currentCooldown = wallRunCooldown;
        }
        else
        {
            if (currentCooldown < 0)
            {
                Velocity = Vector3.Lerp(Vector3.ProjectOnPlane(Velocity, Transform.forward), Velocity, 0.3f);
                Owner.TransitionTo<PlayerAirState>();
            }
            currentCooldown -= PlayerDeltaTime;
        }
    }

    private void Movement()
    {
        Velocity += Vector3.down * (Gravity / 2) * PlayerDeltaTime;

        CheckCollision(Velocity * PlayerDeltaTime);

        Velocity *= Mathf.Pow(AirResistanceCoefficient, PlayerDeltaTime);

        Jump(wallNormal);
    }

    private void CauseDiegeticSound()
    {
        float soundDistance = (Velocity.magnitude / Owner.MaxSpeed) * MovementSoundRange;
        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new PlayerDiegeticSoundEventInfo(Owner.gameObject, soundDistance));
    }

    private void ProjectSpeedOnSurface()
    {
        //Vector3 projection = Vector3.Dot(Velocity, wallNormal) * wallNormal;
        //Vector3 tempVelocity = Velocity - projection;
        //Vector3 magnitude = (projection.magnitude + tempVelocity.magnitude) * tempVelocity.normalized;
        //Velocity = Vector3.ClampMagnitude(magnitude, Velocity.magnitude);
        Velocity = Velocity.magnitude * Vector3.ProjectOnPlane(Velocity, wallNormal).normalized;
    }

    public override void Exit()
    {
        base.Exit();
        Animator.SetBool("WallRunning", false);
    }
}
