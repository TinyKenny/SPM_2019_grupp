using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float damage = 3.0f;
    private float speed = 60.0f;
    public LayerMask ignoreLayer;
    public float distanceTraveled = 0.0f;
    public float distanceToTravel = 300.0f;

    private SphereCollider thisCollider;

    public void SetInitialValues(LayerMask layerToIgnore)
    {
        ignoreLayer |= layerToIgnore;
    }

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<SphereCollider>();
        Debug.Log(thisCollider.radius);
        Debug.Log(transform.localScale);
    }

    // Update is called once per frame
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
            //else if (rayHit.transform.CompareTag("Enemy Hitbox"))
            //{
            //    Debug.Log("An enemy was hit!");
            //    EnemyDamageEventInfo eDEI = new EnemyDamageEventInfo(rayHit.transform.gameObject);
            //    EventCoordinator.CurrentEventCoordinator.ActivateEvent(eDEI);
            //}

            //Debug.Log("projectile forward: " + transform.forward);
            //Debug.Log("projectile hit: " + rayHit.point);
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
}
