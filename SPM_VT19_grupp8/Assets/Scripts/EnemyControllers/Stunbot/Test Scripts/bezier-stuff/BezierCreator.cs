using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCreator : MonoBehaviour
{
    public Transform AnchorOne;
    public Transform ControlOne;
    public Transform ControlTwo;
    public Transform AnchorTwo;

    public int pointsAlongTheWayCount = 20;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawLine(AnchorOne.position, ControlOne.position);
        //Gizmos.DrawLine(AnchorTwo.position, ControlTwo.position);


        Gizmos.color = Color.red;
        Vector3 previousPoint = AnchorOne.position;
        for (int i = 1; i <= pointsAlongTheWayCount; i++)
        {
            float lerpValue = 1.0f * i / pointsAlongTheWayCount;

            Vector3 straightOne = Vector3.Lerp(AnchorOne.position, ControlOne.position, lerpValue);
            Vector3 straightTwo = Vector3.Lerp(ControlOne.position, ControlTwo.position, lerpValue);
            Vector3 straightThree = Vector3.Lerp(ControlTwo.position, AnchorTwo.position, lerpValue);

            Vector3 quadraticOne = Vector3.Lerp(straightOne, straightTwo, lerpValue);
            Vector3 quadraticTwo = Vector3.Lerp(straightTwo, straightThree, lerpValue);

            Vector3 cubicCurrent = Vector3.Lerp(quadraticOne, quadraticTwo, lerpValue);

            Gizmos.DrawLine(previousPoint, cubicCurrent);
            previousPoint = cubicCurrent;
        }
    }
}
