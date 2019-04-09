using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float speed = 15.0f;
    public LayerMask ignoreLayer;

    private SphereCollider thisCollider;

    public void SetInitialValues(LayerMask layerToIgnore)
    {
        ignoreLayer = layerToIgnore;
    }

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = transform.forward * speed * Time.deltaTime;

        RaycastHit rayHit;

        bool hitSomething = Physics.SphereCast(transform.position, thisCollider.radius, transform.forward, out rayHit, speed * Time.deltaTime, ~ignoreLayer, QueryTriggerInteraction.Ignore);

        if (hitSomething)
        {

            if (rayHit.transform.CompareTag("Player"))
            {
                Debug.Log("The player was hit!");
            } else if (rayHit.transform.CompareTag("Enemy"))
            {
                Debug.Log("An enemy was hit!");
            }

            Destroy(gameObject);
        } else
        {
            transform.position += movement;
        }
    }
}
