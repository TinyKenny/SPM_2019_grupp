using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoxCompareNode : IComparable<BoxCompareNode>
{
    private NavBox box;
    private BoxCompareNode end;
    public BoxCompareNode Previous;
    public List<NavBox> Neighbours { get { return box.Neighbours; } }
    public float DistanceTraveled = Mathf.Infinity;
    public bool Known = false;

    public BoxCompareNode (NavBox b, BoxCompareNode e, BoxCompareNode pre)
    {
        box = b;
        end = e;
        Previous = pre;
    }

    public int CompareTo(BoxCompareNode box)
    {
        return GetValue().CompareTo(box.GetValue());
    }

    private float GetValue()
    {
        return (end.box.transform.position - box.transform.position).magnitude + PreviousDistance();
    }

    private float PreviousDistance()
    {
        float previousDistance = 0;

        if (Previous != null)
        {
            previousDistance += (box.transform.position - Previous.box.transform.position).magnitude + Previous.PreviousDistance();
        }

        return previousDistance;
    }

    public NavBox GetBox()
    {
        return box;
    }
}
