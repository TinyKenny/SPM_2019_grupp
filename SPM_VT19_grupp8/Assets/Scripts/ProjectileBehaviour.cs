using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float speed = 15.0f;
    //public LayerMask;

    private SphereCollider thisCollider;

    public void SetInitialValues()
    {

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

        //Physics.SphereCast()

        //bool hitSomething = Physics.SphereCast(transform.position, thisCollider.radius, transform.forward, speed * Time.deltaTime, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        transform.position += movement;
    }
}
