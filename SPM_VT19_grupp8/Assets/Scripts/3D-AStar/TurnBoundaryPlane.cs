using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TurnBoundaryPlane
{
    private Plane plane;
    private bool approachSide;

    public TurnBoundaryPlane(Vector3 pointOnPlane, Vector3 pointPerpendicularToPlane)
    {
        plane = new Plane((pointPerpendicularToPlane - pointOnPlane).normalized, pointOnPlane);

        approachSide = false;
        approachSide = plane.GetSide(pointPerpendicularToPlane);
    }

    public bool HasCrossedLine(Vector3 p)
    {
        return plane.GetSide(p) != approachSide;
    }

    public float DistanceFromPoint(Vector3 p)
    {
        return plane.GetDistanceToPoint(p);
    }
}
