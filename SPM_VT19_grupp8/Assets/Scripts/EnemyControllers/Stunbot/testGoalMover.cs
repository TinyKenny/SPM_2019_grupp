using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is only for testing, so you do not have to review this unless you absolutely want to.
/// Warning: it may or may not contain a lot of shit-quality code.
/// </summary>
public class testGoalMover : MonoBehaviour
{
    private float movementSpeed = 1.0f;
    ///private float rotationSpeed = 45.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0.0f , -Input.GetAxisRaw("GP Look Y"), -Input.GetAxisRaw("GP Look X")) * movementSpeed * Time.deltaTime;


        //transform.Rotate(-new Vector3(1.0f, 0.0f, 0.0f) * Input.GetAxisRaw("Aim")  * rotationSpeed * Time.deltaTime);
        //transform.position += transform.forward * Input.GetAxisRaw("Shoot") * movementSpeed * Time.deltaTime;

    }
}
