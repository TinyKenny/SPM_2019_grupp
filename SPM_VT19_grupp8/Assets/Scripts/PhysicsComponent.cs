using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsComponent : MonoBehaviour
{
    public float acceleration;
    public float deceleration; // not used?
    public float maxSpeed;
    //[Range(0.0f, 1.0f)]
    public float frictionCoefficient = 0.4f;
    [Range(0.0f, 1.0f)]
    public float airResistanceCoefficient = 0.95f;
    public float gravity;
    public float skinWidth = 0.01f;

    public Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
