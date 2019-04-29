using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunbotFlightTest : MonoBehaviour
{

    public Transform[] goals;

    private int currentGoal = 0;
    private Vector3 startPosition;
    private float maxSpeed = 10.0f;
    public float velocity = 0.0f;
    private float acceleration = 2.0f;
    private float deceleration = 1.0f;
    private float turnSpeed = 60.0f;
    //private float turnSpeed = 0.5f;
    private float airResistance = 0.9f;


    public float CurrentMaxSpeed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        CurrentMaxSpeed = CalculateMaxSpeed(transform.position, goals[currentGoal].position);

        Debug.Log("Förutsäg rotationen den ska ha när den når sitt mål, så att den kan anpassa sin hastighet i förväg");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGoal < goals.Length)
        {
            Vector3 goalPosition = goals[currentGoal].position;

            Vector3 goalDirection = (goalPosition - transform.position).normalized;

            Quaternion desiredRotation = Quaternion.LookRotation(goalDirection);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);


            if(velocity > CurrentMaxSpeed + 0.01f)
            {
                float ammountToDecel = Mathf.Min(velocity - CurrentMaxSpeed, deceleration * Time.deltaTime);
                velocity -= ammountToDecel;
            }
            else
            {
                velocity = Mathf.Clamp(velocity + acceleration * Time.deltaTime, 0.0f, CurrentMaxSpeed);
            }



            //if (Mathf.Approximately(Vector3.Dot(transform.forward, goalDirection), 1.0f))
            //{
            //    velocity = Mathf.Clamp(velocity + acceleration * Time.deltaTime, 0.0f, CurrentMaxSpeed);
            //}
            //else
            //{
            //    velocity = Mathf.Clamp(velocity - deceleration * Time.deltaTime, 0.0f, CurrentMaxSpeed);
            //}

            //if ((Vector3.Distance(transform.position, goalPosition) / velocity) > (Vector3.Angle(goalDirection, transform.forward) / turnSpeed))
            //{
            //    velocity = Mathf.Clamp(velocity + acceleration * Time.deltaTime, 0.0f, maxSpeed);
            //}
            //else
            //{
            //    velocity = Mathf.Clamp(velocity - deceleration * Time.deltaTime, 0.0f, maxSpeed);
            //}

            velocity *= Mathf.Pow(airResistance, Time.deltaTime);

            transform.position += transform.forward * velocity * Time.deltaTime;
        }

        if(Vector3.Distance(transform.position, goals[currentGoal].position) < 0.1f)
        {
            currentGoal = (currentGoal + 1) % goals.Length;
            
        }
        CurrentMaxSpeed = CalculateMaxSpeed(transform.position, goals[currentGoal].position);
    }

    private float CalculateMaxSpeed(Vector3 startPosition, Vector3 goalPosition)
    {


        Vector3 goalPositionDifference = goalPosition - startPosition;

        float goFast = goalPositionDifference.magnitude * turnSpeed * Mathf.Deg2Rad / (2 * Mathf.Sin(Vector3.Angle(transform.forward, goalPositionDifference.normalized) * Mathf.Deg2Rad));




        float newMaxSpeed = Mathf.Min(goFast, maxSpeed);
        Debug.Log(newMaxSpeed);

        return newMaxSpeed;
    }
}
