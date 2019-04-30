using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBaseState : State
{
    protected PlayerStateMachine owner;

    [Header("Leave at 1 in WalkingState")] //för att skrämma iväg designers
    //[Range(0.0f, 1.0f)]
    public float MaxSpeedMod = 1.0f;


    public float Acceleration { get { return owner.Acceleration; } set { owner.Acceleration = value; } }
    public float Deceleration { get { return owner.Deceleration; } set { owner.Deceleration = value; } }
    public float MaxSpeed { get { return owner.MaxSpeed * MaxSpeedMod; } /*set { owner.MaxSpeed = value; } */}
    public float FrictionCoefficient { get { return owner.FrictionCoefficient; } set { owner.FrictionCoefficient = value; } }
    public float AirResistanceCoefficient { get { return owner.AirResistanceCoefficient; } set { owner.AirResistanceCoefficient = value; } }
    public float Gravity { get { return owner.Gravity; } set { owner.Gravity = value; } }
    private float FireRate { get { return owner.fireRate; } }
    
    public Vector3 Velocity { get { return owner.Velocity; } set { owner.Velocity = value; } }

    protected Transform Transform { get { return owner.transform; } }
    protected LayerMask CollisionLayers { get { return owner.collisionLayers; } }
    protected CapsuleCollider ThisCollider { get { return owner.thisCollider; } }
    protected float SkinWidth { get { return owner.skinWidth; } }
    protected float GroundCheckDistance { get { return owner.groundCheckDistance; } }
    protected float TurnSpeedModifier { get { return owner.turnSpeedModifier; } }
    protected float StandardColliderHeight { get { return owner.standardColliderHeight; } }
    protected float PlayerDeltaTime { get { return owner.getPlayerDeltaTime(); } }
    protected int Ammo { get { return owner.ammo; } set { owner.ammo = value; } }
    private float FireCoolDown { get { return owner.fireCoolDown; } set { owner.fireCoolDown = value; } }

    public override void Initialize(StateMachine owner)
    {
        this.owner = (PlayerStateMachine)owner;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        UpdatePlayerRotation();
    }

    protected bool FindCollision(Vector3 direction, float maxDistance)
    {
        RaycastHit raycastHit;
        return FindCollision(direction, out raycastHit, maxDistance);
    }

    protected bool FindCollision(Vector3 direction, out RaycastHit raycastHit, float maxDistance)
    {
        Vector3 colliderDirection = Vector3.zero;

        switch (ThisCollider.direction)
        {
            case (0): // X-axis
                colliderDirection = Transform.right;
                break;
            case (1): // Y-axis
                colliderDirection = Transform.up;
                break;
            case (2): // Z-axis
                colliderDirection = Transform.forward;
                break;
        }

        Vector3 topPoint = Transform.position + ThisCollider.center + colliderDirection * (ThisCollider.height / 2 - ThisCollider.radius);
        Vector3 bottomPoint = Transform.position + ThisCollider.center - colliderDirection * (ThisCollider.height / 2 - ThisCollider.radius);

        return Physics.CapsuleCast(topPoint, bottomPoint, ThisCollider.radius, direction.normalized, out raycastHit, maxDistance, CollisionLayers, QueryTriggerInteraction.Ignore);
    }

    protected bool GroundCheck()
    {
        RaycastHit raycastHit;
        return GroundCheck(out raycastHit);
    }

    protected bool GroundCheck(out RaycastHit raycastHit)
    {
        return FindCollision(Vector3.down, out raycastHit, GroundCheckDistance + SkinWidth);
    }

    protected void CheckCollision(Vector3 movement)
    {
        RaycastHit raycastHit;

        bool castHasHit = FindCollision(movement.normalized, out raycastHit, Mathf.Infinity);

        if (castHasHit)
        {
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
            castHasHit = FindCollision(Vector3.down, out raycastHit, SkinWidth + GroundCheckDistance);

            if (castHasHit)
            {
                PhysicsComponent otherPhysicsComponent = raycastHit.collider.GetComponent<PhysicsComponent>();
                //CalculateFriction(Gravity * PlayerDeltaTime * PlayerDeltaTime, otherPhysicsComponent); // remove this if we dont need friction
            }

            Transform.position += movement;
        }
    }

    /*
    protected void CalculateFriction(float normalForceMagnitude, PhysicsComponent otherPhysicsComponent)
    {
        float realFrictionCoefficient = FrictionCoefficient;
        Vector3 velocityDelta = Velocity;

        if (otherPhysicsComponent != null)
        {
            realFrictionCoefficient = (realFrictionCoefficient + otherPhysicsComponent.frictionCoefficient) / 2;
            velocityDelta -= otherPhysicsComponent.velocity;
        }

        float frictionForceMagnitude = realFrictionCoefficient * normalForceMagnitude;

        if (velocityDelta.magnitude < frictionForceMagnitude)
        {
            if (otherPhysicsComponent != null)
            {
                Velocity = otherPhysicsComponent.velocity;
            }
            else
            {
                Velocity = Vector3.zero;
            }
        }
        else
        {
            frictionForceMagnitude *= 0.7f;
            Vector3 frictionForce = velocityDelta.normalized * frictionForceMagnitude;
            Velocity -= frictionForce;
        }
    }
    */

    protected virtual void HandleCollition(Vector3 hitNormal, RaycastHit raycastHit)
    {
        Vector3 hitNormalForce = MathHelper.NormalForce(Velocity, hitNormal);
        PhysicsComponent otherPhysicsComponent = raycastHit.collider.GetComponent<PhysicsComponent>();

        Velocity += hitNormalForce;
        //CalculateFriction(hitNormalForce.magnitude, otherPhysicsComponent); // remove this if we dont need friction
    }

    protected void Shoot()
    {
        if (Input.GetAxisRaw("Aim") == 1f)
        {
            Camera.main.GetComponent<CameraController>().Aiming();

            if (Input.GetAxisRaw("Shoot") == 1f && FireCoolDown < 0 && Ammo > 0 && Time.timeScale > 0)
            {
                Ammo--;
               

                Vector3 reticleLocation = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0.0f);

                Ray aimRay = Camera.main.ScreenPointToRay(reticleLocation);


                string[] ignoreLayers = new string[] { "3DNavMesh" };
                RaycastHit rayHit;
                bool rayHasHit = Physics.Raycast(aimRay, out rayHit, owner.projectilePrefab.GetComponent<ProjectileBehaviour>().distanceToTravel, ~((1 << owner.gameObject.layer) | (LayerMask.GetMask(ignoreLayers))));

                Debug.Log(aimRay.origin);
                Debug.DrawRay(aimRay.origin, aimRay.direction * rayHit.distance, new Color(255.0f, 0.0f, 0.0f), 1.5f);

                Vector3 pointHit = rayHit.point;
                if (!rayHasHit)
                {
                    pointHit = aimRay.GetPoint(owner.projectilePrefab.GetComponent<ProjectileBehaviour>().distanceToTravel);
                }
                else
                {
                    Debug.Log("aim hit! " + rayHit.point);
                    Debug.Log(rayHit.collider.name);
                }
                GameObject projectile = Instantiate(owner.projectilePrefab, Transform.position + (Camera.main.transform.rotation * Vector3.forward), Camera.main.transform.rotation);

                projectile.transform.LookAt(pointHit); // test-y stuff
                projectile.GetComponent<ProjectileBehaviour>().SetInitialValues((1 << owner.gameObject.layer) | LayerMask.GetMask(ignoreLayers));
                FireCoolDown = FireRate;
                owner.ammoNumber.text = Ammo.ToString();
            }
        }
        else
        {
            Camera.main.GetComponent<CameraController>().StopAiming();
        }
        
        FireCoolDown -= PlayerDeltaTime;
    }

    protected virtual void UpdatePlayerRotation()
    {
        Transform.LookAt(Transform.position + new Vector3(Velocity.x, 0.0f, Velocity.z).normalized);
    }


}
