using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBaseState : State
{
    #region owner properties
    protected Vector3 Velocity { get { return owner.Velocity; } set { owner.Velocity = value; } }
    protected Transform Transform { get { return owner.transform; } }
    protected CapsuleCollider ThisCollider { get { return owner.ThisCollider; } }
    protected LayerMask CollisionLayers { get { return owner.CollisionLayers; } }
    protected Animator Animator { get { return owner.Animator; } }
    protected CameraController MainCameraController { get { return owner.MainCameraController; } } // make this private?
    protected float Acceleration { get { return owner.Acceleration; } }
    protected float Deceleration { get { return owner.Deceleration; } }
    protected float MaxSpeed { get { return owner.MaxSpeed * MaxSpeedMod; } }
    protected float AirResistanceCoefficient { get { return owner.AirResistanceCoefficient; } }
    protected float Gravity { get { return owner.Gravity; } }
    protected float SkinWidth { get { return owner.skinWidth; } }
    protected float GroundCheckDistance { get { return owner.groundCheckDistance; } }
    protected float TurnSpeedModifier { get { return owner.TurnSpeedModifier; } }
    protected float StandardColliderHeight { get { return owner.StandardColliderHeight; } }
    protected float PlayerDeltaTime { get { return owner.getPlayerDeltaTime(); } }
    protected float MovementSoundRange { get { return owner.MovementSoundRange; } }
    protected float ShootSoundRange { get { return owner.ShootSoundRange; } }
    protected float JumpPower { get { return owner.JumpPower; } }
    private float FireRate { get { return owner.FireRate; } }
    private GameObject ProjectilePrefab { get { return owner.ProjectilePrefab; } }
    private AudioClip GunShotSound { get { return owner.GunShotSound; } }
    #endregion

    protected PlayerStateMachine owner;
    [Header("Leave at 1 in WalkingState")]
    [SerializeField] protected float MaxSpeedMod = 1.0f;



    public override void Initialize(StateMachine owner)
    {
        this.owner = (PlayerStateMachine)owner;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        UpdatePlayerRotation();
        Animator.SetFloat("Speed", new Vector3(Velocity.x, 0, Velocity.z).magnitude / owner.MaxSpeed);
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
        owner.Shoot();
    }

    protected virtual void UpdatePlayerRotation()
    {
        Transform.LookAt(Transform.position + new Vector3(Velocity.x, 0.0f, Velocity.z).normalized);
    }


}
