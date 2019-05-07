using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is only for testing, so you do not have to review this unless you absolutely want to.
/// Warning: it may or may not contain a lot of shit-quality code.
/// </summary>
public class ArcCalcTest : MonoBehaviour
{
    public Transform start;
    public Transform goal;
    public Transform pivot; // for monitoring, replace with a Vector3
    public Transform afterRotateMarker; // for monitoring, replace with a Vector3

    [Range(0.0f, 2.0f)]
    public float turnSpeed = 0.5f;
    private float _turnSpeed { get { return turnSpeed * Mathf.PI; } }
    public float movementSpeed = 1.5f;
    public float maxSpeed = 3.0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float speedAtGoal = 0.0f; // for testing, repace with next goals predicted max speed
    

    [Header("Output")]
    public float TurningTime = 0.0f; // first "acutal output"
    public float StraightTime = 0.0f; // second "actual output"
    public float TotalTime = 0.0f; // third "actual output"
    public float rotationAngle = 0.0f; // for monitoring
    public Vector3 rotationAxis = Vector3.zero; // for monitoring
    public Vector3 rotat = Vector3.zero; // for monitoring
    public float distanceAfterTurn = 0.0f;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        Quaternion rotationToGoal = Quaternion.FromToRotation(transform.forward, (goal.position - start.position).normalized);

        getAngleAxis(rotationToGoal, out rotationAngle, out rotationAxis);

        Debug.DrawRay(start.position, rotationAxis * 2.0f);
        rotat = rotationToGoal.eulerAngles;
        



        Quaternion pivotRotation = Quaternion.AngleAxis(90.0f, rotationAxis);

        pivot.position = start.position + (pivotRotation * start.forward * movementSpeed / _turnSpeed);


        float sigDegAngle = Vector3.SignedAngle(pivot.position - start.position, pivot.position - goal.position, rotationAxis);
        sigDegAngle = Mathf.Max(sigDegAngle, (360.0f + sigDegAngle) % 360.0f);

        



        //TurningTime = (Vector3.Angle(pivot.position - start.position, pivot.position - goal.position) * Mathf.Deg2Rad - Mathf.Acos(movementSpeed / (_turnSpeed * Vector3.Distance(pivot.position, goal.position)))) / _turnSpeed;
        TurningTime = (sigDegAngle * Mathf.Deg2Rad - Mathf.Acos(movementSpeed / (_turnSpeed * Vector3.Distance(pivot.position, goal.position)))) / _turnSpeed;
        TurningTime = Mathf.Clamp(TurningTime, 0.0f, Mathf.Infinity);




        Vector3 positionAfterRotation = pivot.position + Quaternion.AngleAxis(TurningTime * _turnSpeed * Mathf.Rad2Deg, rotationAxis) * (start.position - pivot.position);
        afterRotateMarker.position = positionAfterRotation;





        distanceAfterTurn = Vector3.Distance(afterRotateMarker.position, goal.position);

        float speedAfterTurn = movementSpeed;


        //float maxAccelPortion = acceleration / (acceleration + deceleration);

        float topSpeed;

        float timeAccelerating;
        float timeAtMaxSpeed;
        float timeDecelerating;

        float distanceAccelerating;
        float distanceAtMaxSpeed;
        float distanceDecelerating;






        //topSpeed = speedAfterTurn + acceleration * timeAccelerating;
        //topSpeed = speedAtGoal + deceleration * timeDecelerating;


        //distanceAccelerating = speedAfterTurn * timeAccelerating + acceleration * timeAccelerating * timeAccelerating / 2;
        //distanceDecelerating = topSpeed * timeDecelerating - deceleration * timeDecelerating * timeDecelerating / 2;

        










        StraightTime = distanceAfterTurn / speedAfterTurn;

        TotalTime = TurningTime + StraightTime;










    }

    void getAngleAxis(Quaternion rotation, out float angleOutput, out Vector3 axisOutput)
    {
        rotation.ToAngleAxis(out angleOutput, out axisOutput);
        if(angleOutput > 180.0f)
        {
            angleOutput = 180.0f - angleOutput;
            axisOutput = -axisOutput;
        }
    }
}
