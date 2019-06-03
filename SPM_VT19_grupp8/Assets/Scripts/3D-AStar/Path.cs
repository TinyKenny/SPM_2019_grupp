using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public Vector3[] LookPoints { get; }
    public TurnBoundaryPlane[] TurnBoundaries { get; }
    public int FinishLineIndex { get; }
    public int SlowDownIndex { get; }

    public Path(Vector3[] waypoints, Vector3 startPosition, float turnDst, float stoppingDst)
    {
        LookPoints = waypoints;
        TurnBoundaries = new TurnBoundaryPlane[LookPoints.Length];
        FinishLineIndex = TurnBoundaries.Length - 1;

        Vector3 previousPoint = startPosition;

        for(int i = 0; i < LookPoints.Length; i++)
        {
            Vector3 currentPoint = LookPoints[i];
            Vector3 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector3 turnBoundaryPoint = (i == FinishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;

            TurnBoundaries[i] = new TurnBoundaryPlane(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);

            previousPoint = turnBoundaryPoint;
        }

        float dstFromEndpoint = 0;
        for(int i = LookPoints.Length - 1; i > 0; i--)
        {
            dstFromEndpoint += Vector3.Distance(LookPoints[i], LookPoints[i - 1]);
            if(dstFromEndpoint > stoppingDst)
            {
                SlowDownIndex = i;
                break;
            }
        }
    }
}
