using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask ignoreLayer;

    private float distanceTraveled = 0.0f;
    private float distanceToTravel = 300.0f;
    private float damage = 3.0f;
    private float speed = 60.0f;
    private SphereCollider thisCollider;
    private ProjectileInfo projectile = null;

    public void SetInitialValues(LayerMask layerToIgnore)
    {
        ignoreLayer |= layerToIgnore;
    }

    void Start()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<SaveEventInfo>(SaveProjectileInfo);
        thisCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        Vector3 movement = transform.forward * speed * Time.deltaTime;

        RaycastHit rayHit;

        bool hitSomething = Physics.SphereCast(transform.position, thisCollider.radius * transform.localScale.x, transform.forward, out rayHit, speed * Time.deltaTime, ~ignoreLayer, QueryTriggerInteraction.Ignore);

        if (hitSomething)
        {

            if (rayHit.transform.CompareTag("Player"))
            {
                Debug.Log("The player was hit!");
                PlayerDamageEventInfo pDEI = new PlayerDamageEventInfo(rayHit.transform.gameObject, damage);
                EventCoordinator.CurrentEventCoordinator.ActivateEvent(pDEI);

            }
            Destroy(gameObject);
        }
        else
        {
            transform.position += movement;
        }

        if (distanceTraveled >= distanceToTravel)
        {
            Destroy(gameObject);
        }
        else
        {
            distanceTraveled += movement.magnitude;
        }
    }

    private void SaveProjectileInfo(EventInfo eI)
    {
        if (projectile != null)
            GameController.GameControllerInstance.CurrentSave.Projectiles.Remove(projectile);
        projectile = new ProjectileInfo(transform.position, transform.rotation.eulerAngles);
        GameController.GameControllerInstance.CurrentSave.Projectiles.Add(projectile);
    }

    private void OnDestroy()
    {
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<SaveEventInfo>(SaveProjectileInfo);
        if (projectile != null)
            GameController.GameControllerInstance.CurrentSave.Projectiles.Remove(projectile);
    }
}
