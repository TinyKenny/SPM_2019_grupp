using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Nodes that <see cref="AStarPathfindning"/> creates when it is finding the shortest path to allow all 
/// different instances of <see cref="AStarPathfindning"/> to be able to keep different values on their 
/// traversed boxes/nodes.
/// </summary>
public class BoxCompareNode : IComparable<BoxCompareNode>
{
    private NavBox box;
    private BoxCompareNode end;

    public BoxCompareNode Previous { get; set; }
    public List<NavBox> Neighbours { get { return box.Neighbours; } }
    public float DistanceTraveled { get; set; } = Mathf.Infinity;
    public bool Known { get; set; } = false;
    public Vector3 Position { get; set; }

    public BoxCompareNode (NavBox b, BoxCompareNode e)
    {
        box = b;
        end = e;
        Previous = null;
    }

    public int CompareTo(BoxCompareNode box)
    {
        return GetValue().CompareTo(box.GetValue());
    }

    /// <summary>
    /// Calculates the heuristic path to the end plus how far it has travelled to get to this node.
    /// </summary>
    /// <returns>Heuristic path plus how far it has travelled to get to this node</returns>
    private float GetValue()
    {
        return (end.box.transform.position - box.transform.position).magnitude + PreviousDistance();
    }

    /// <summary>
    /// Checks if this node has a previous, if it does it will return how far away the previous node was and add the previous nodes previous distance if possible.
    /// </summary>
    /// <returns>The distance travelled to start of the path.</returns>
    private float PreviousDistance()
    {
        float previousDistance = 0;

        if (Previous != null)
        {
            previousDistance += (box.Coll.bounds.max - Previous.box.Coll.bounds.max).magnitude + Previous.PreviousDistance();
        }

        return previousDistance;
    }

    public NavBox GetBox()
    {
        return box;
    }
}
