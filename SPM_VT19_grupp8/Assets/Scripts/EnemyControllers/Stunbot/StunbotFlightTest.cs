using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunbotFlightTest : MonoBehaviour
{
    public Transform rotationPrediction;
    public Transform directionMarker;

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
    private SphereCollider myCollider;

    public float CurrentMaxSpeed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        CurrentMaxSpeed = CalculateMaxSpeed(transform.position, currentGoal, transform.rotation, 0.0f);
        myCollider = GetComponent<SphereCollider>();

        Debug.Log("Förutsäg rotationen den ska ha när den når sitt mål, så att den kan anpassa sin hastighet i förväg");

        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGoal < goals.Length)
        {
            Vector3 goalPosition = goals[currentGoal].position;

            Vector3 goalDirection = (goalPosition - transform.position).normalized;

            Quaternion desiredRotation = Quaternion.LookRotation(goalDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);


            CurrentMaxSpeed = CalculateMaxSpeed(transform.position, currentGoal, transform.rotation, 0.0f);


            if (velocity > CurrentMaxSpeed + 0.01f)
            {
                float ammountToDecel = Mathf.Min(velocity - CurrentMaxSpeed, deceleration * Time.deltaTime);
                velocity -= ammountToDecel;
            }
            else
            {
                velocity = Mathf.Clamp(velocity + acceleration * Time.deltaTime, 0.0f, CurrentMaxSpeed);
            }



            
        }

        velocity *= Mathf.Pow(airResistance, Time.deltaTime);

        RaycastHit rayHit;

        bool hasHitSomething = Physics.SphereCast(transform.position, myCollider.radius, transform.forward, out rayHit, velocity * Time.deltaTime);


        

        transform.position += transform.forward * velocity * Time.deltaTime;


        if (Vector3.Distance(transform.position, goals[currentGoal].position) < (myCollider.radius +  velocity * Time.deltaTime) /*hasHitSomething && rayHit.transform == goals[currentGoal]*/)
        {
            currentGoal = (currentGoal + 1) % goals.Length;











        }


        //if (Vector3.Distance(transform.position, goals[currentGoal].position) < 0.1f)
        //{
        //    currentGoal = (currentGoal + 1) % goals.Length;
        //    Quaternion predictedRotation = new Quaternion();
        //    predictedRotation.SetFromToRotation(transform.forward, (goals[currentGoal].position - transform.position).normalized);
        //    predictedRotation = predictedRotation * predictedRotation;
        //    rotationPrediction.position = goals[currentGoal].position + predictedRotation * transform.forward;
        //}

        

    }

    private float CalculateMaxSpeed(Vector3 startPosition, int goalIndex, Quaternion startRotation, float distanceSoFar, float currentVelocity = 0.0f)
    {
        Vector3 goalPosition = goals[goalIndex].position;

        Vector3 goalPositionDifference = goalPosition - startPosition;

        float goFast = goalPositionDifference.magnitude * turnSpeed * Mathf.Deg2Rad / (2 * Mathf.Sin(Vector3.Angle(startRotation * Vector3.forward, goalPositionDifference.normalized) * Mathf.Deg2Rad));

        float newMaxSpeed = Mathf.Min(goFast, maxSpeed);
        

        #region prediction

        Quaternion predictedRotation = new Quaternion();
        predictedRotation.SetFromToRotation(startRotation * Vector3.forward, goalPositionDifference.normalized);
        predictedRotation = predictedRotation * predictedRotation;
        Vector3 predictedRotationVector = predictedRotation * (startRotation * Vector3.forward);

        if (Mathf.Approximately(distanceSoFar, 0.0f))
        {
            rotationPrediction.position = goalPosition + predictedRotationVector;

            directionMarker.position = goalPosition + goalPositionDifference.normalized;
        }

        float archLength = goalPositionDifference.magnitude * Vector3.Angle(startRotation * Vector3.forward, goalPositionDifference.normalized) * Mathf.Deg2Rad / Mathf.Sin(Vector3.Angle(startRotation * Vector3.forward, goalPositionDifference.normalized) * Mathf.Deg2Rad);


        distanceSoFar += archLength;

        
        if (distanceSoFar < (velocity * velocity / deceleration - velocity * velocity / (2 * deceleration)))
        {
            Debug.Log("wha!");

            float otherMaxSpeed = CalculateMaxSpeed(goalPosition, (goalIndex + 1) % goals.Length, startRotation * predictedRotation, distanceSoFar);


            float brakeTime = Mathf.Clamp((velocity - otherMaxSpeed) / deceleration, 0.0f, Mathf.Infinity);


            float maxBreakTime = Mathf.Clamp((newMaxSpeed - otherMaxSpeed) / deceleration, 0.0f, Mathf.Infinity);



            //float archLength = goalPositionDifference.magnitude * Vector3.Angle(startRotation * Vector3.forward, goalPositionDifference.normalized) * Mathf.Deg2Rad / Mathf.Sin(Vector3.Angle(startRotation * Vector3.forward, goalPositionDifference.normalized) * Mathf.Deg2Rad);


            float distanceTraveledUnitllDesiredSpeed = velocity * brakeTime - deceleration * brakeTime * brakeTime / 2;





            if (distanceTraveledUnitllDesiredSpeed * 1.1f > archLength)
            {
                if(otherMaxSpeed < newMaxSpeed)
                {
                    Debug.LogWarning("Prediction!");
                }
                newMaxSpeed = Mathf.Min(newMaxSpeed, otherMaxSpeed);
                
            }

            // check if you need to start braking yet


            if (velocity > otherMaxSpeed)
            {
                


            }
        }





        #endregion


        Debug.Log(newMaxSpeed);
        return newMaxSpeed;
    }
}
