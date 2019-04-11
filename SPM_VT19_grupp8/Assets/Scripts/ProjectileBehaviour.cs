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
        ignoreLayer = layerToIgnore;
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
                PlayerStateMachine player = rayHit.transform.GetComponent<PlayerStateMachine>();
                player.TakeDamage(damage);

            }
            else if (rayHit.transform.CompareTag("Enemy Hitbox"))
            {
                Debug.Log("An enemy was hit!");
                EnemyHealthPOC enemyHealth = rayHit.transform.GetComponent<EnemyHealthPOC>();
                enemyHealth.TakeDamage(damage);
            }

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
