using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public readonly Vector3[] lookPoints;
    public readonly TurnBoundaryPlane[] turnBoundaries;
    public readonly int finishLineIndex;
    public readonly int slowDownIndex;

    public Path(Vector3[] waypoints, Vector3 startPosition, float turnDst, float stoppingDst)
    {
        lookPoints = waypoints;
        turnBoundaries = new TurnBoundaryPlane[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector3 previousPoint = startPosition;

        for(int i = 0; i < lookPoints.Length; i++)
        {
            Vector3 currentPoint = lookPoints[i];
            Vector3 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector3 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;

            //this might be completely wrong...
            turnBoundaries[i] = new TurnBoundaryPlane(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);

            previousPoint = turnBoundaryPoint;
        }

        float dstFromEndpoint = 0;
        for(int i = lookPoints.Length - 1; i > 0; i--)
        {
            dstFromEndpoint += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
            if(dstFromEndpoint > stoppingDst)
            {
                slowDownIndex = i;
                break;
            }
        }
    }
}
