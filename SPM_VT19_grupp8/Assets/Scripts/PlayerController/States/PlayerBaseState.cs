using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBaseState : State
{
    #region owner properties
    protected Vector3 Velocity { get { return Owner.Velocity; } set { Owner.Velocity = value; } }
    protected Transform Transform { get { return Owner.transform; } }
    protected CapsuleCollider ThisCollider { get { return Owner.ThisCollider; } }
    protected LayerMask CollisionLayers { get { return Owner.CollisionLayers; } }
    protected Animator Animator { get { return Owner.Animator; } }
    protected CameraController MainCameraController { get { return Owner.MainCameraController; } }
    protected float Acceleration { get { return Owner.Acceleration; } }
    protected float Deceleration { get { return Owner.Deceleration; } }
    protected float MaxSpeed { get { return Owner.MaxSpeed * MaxSpeedMod; } }
    protected float AirResistanceCoefficient { get { return Owner.AirResistanceCoefficient; } }
    protected float Gravity { get { return Owner.Gravity; } }
    protected float SkinWidth { get { return Owner.skinWidth; } }
    protected float GroundCheckDistance { get { return Owner.groundCheckDistance; } }
    protected float TurnSpeedModifier { get { return Owner.TurnSpeedModifier; } }
    protected float StandardColliderHeight { get { return Owner.StandardColliderHeight; } }
    protected float PlayerDeltaTime { get { return Owner.PlayerDeltaTime; } }
    protected float MovementSoundRange { get { return Owner.MovementSoundRange; } }
    protected float ShootSoundRange { get { return Owner.ShootSoundRange; } }
    protected float JumpPower { get { return Owner.JumpPower; } }
    private float FireRate { get { return Owner.FireRate; } }
    private GameObject ProjectilePrefab { get { return Owner.ProjectilePrefab; } }
    private AudioClip GunShotSound { get { return Owner.GunShotSound; } }
    #endregion

    protected PlayerStateMachine Owner { get; private set; }

    [Header("Leave at 1 in WalkingState")]
    [SerializeField] protected float MaxSpeedMod = 1.0f;



    public override void Initialize(StateMachine owner)
    {
        this.Owner = (PlayerStateMachine)owner;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        UpdatePlayerRotation();
        Animator.SetFloat("Speed", new Vector3(Velocity.x, 0, Velocity.z).magnitude / Owner.MaxSpeed);
        Animator.SetFloat("Direction", Vector3.Dot(Transform.right, Velocity.normalized));
        Animator.SetFloat("HorizontalDirection", Velocity.y);
    }

    protected bool FindCollision(Vector3 direction, float maxDistance)
    {
        return FindCollision(direction, out RaycastHit raycastHit, maxDistance);
    }

    protected bool FindCollision(Vector3 direction, out RaycastHit raycastHit, float maxDistance)
    {
        Vector3 topPoint = Transform.position + ThisCollider.center + Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);
        Vector3 bottomPoint = Transform.position + ThisCollider.center - Transform.up * (ThisCollider.height / 2 - ThisCollider.radius);

        return Physics.CapsuleCast(topPoint, bottomPoint, ThisCollider.radius, direction.normalized, out raycastHit, maxDistance, CollisionLayers, QueryTriggerInteraction.Ignore);
    }

    protected bool GroundCheck()
    {
        return GroundCheck(out RaycastHit raycastHit);
    }

    protected bool GroundCheck(out RaycastHit raycastHit)
    {
        bool grounded = FindCollision(Vector3.down, out raycastHit, GroundCheckDistance + SkinWidth);
        Animator.SetBool("GroundCheck", grounded);
        return grounded;
    }

    protected void CheckCollision(Vector3 movement)
    {
        bool castHasHit = FindCollision(movement.normalized, out RaycastHit raycastHit, Mathf.Infinity);

        if (castHasHit)
        {
            if (raycastHit.normal.y < -0.9f && raycastHit.distance < SkinWidth * 2 && Velocity.y > 0)
            {
                Velocity = new Vector3(Velocity.x, 0, Velocity.z);
            }


            Debug.DrawLine(Transform.position, raycastHit.point, new Color(0.0f, 255.0f, 0.0f));
            Debug.DrawRay(Transform.position, Velocity, new Color(255.0f, 0.0f, 0.0f));

            Vector3 hitNormal = raycastHit.normal;

            float angle = (Vector3.Angle(hitNormal, movement.normalized) - 90) * Mathf.Deg2Rad;
            float snapDistanceFromHit = SkinWidth / Mathf.Sin(angle);

            Vector3 snapMovement = movement.normalized * (raycastHit.distance - snapDistanceFromHit);
            snapMovement = Vector3.ClampMagnitude(snapMovement, movement.magnitude);

            movement -= snapMovement;

            Vector3 hitNormalMovement = MathHelper.NormalForce(movement, hitNormal);
            movement += hitNormalMovement;

            Transform.position += snapMovement;

            if (angle * Mathf.Rad2Deg < 70)
            {
                if (hitNormalMovement.magnitude > MathHelper.floatEpsilon)
                {
                    HandleCollition(hitNormal, raycastHit);
                }

                if (movement.magnitude > MathHelper.floatEpsilon)
                {
                    CheckCollision(movement);
                }
            }
            else if(hitNormalMovement.magnitude > MathHelper.floatEpsilon)
            {
                Velocity = Vector3.zero;
            }
        }

        else if (movement.magnitude > MathHelper.floatEpsilon)
        {
            Transform.position += movement;
        }
    }
    

    protected virtual void HandleCollition(Vector3 hitNormal, RaycastHit raycastHit)
    {
        Vector3 hitNormalForce = MathHelper.NormalForce(Velocity, hitNormal);

        Velocity += hitNormalForce;
    }

    protected void Shoot()
    {
        Owner.Shoot();
    }

    protected virtual void UpdatePlayerRotation()
    {
        Transform.LookAt(Transform.position + new Vector3(Velocity.x, 0.0f, Velocity.z).normalized);
    }


}
