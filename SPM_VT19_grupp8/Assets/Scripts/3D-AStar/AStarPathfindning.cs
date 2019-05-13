using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfindning : MonoBehaviour
{
    private PriorityQueue pq;
    private Dictionary<NavBox, BoxCompareNode> list;

    /// <summary>
    /// Calculates the shortest path from start to end on a pregenerated 3D navmesh. Saves all nodes that should be 
    /// traveresed to take the shortest path in a sorted list. The list is sorted on its keys which 
    /// is how long away from start they are with a value that is their position.
    /// </summary>
    /// <param name="start">The start position of the path.</param>
    /// <param name="end">The end position of the path.</param>
    /// <returns>Returns a sorted list of all paths that need to be traversed to reach destination. The key is the distance from start, the value is the position to go to. If any end of the path is outside the navmesh null is returned.</returns>
    public SortedList<float, Vector3> FindPath(Vector3 start, Vector3 end)
    {
        BoxCompareNode bcnEnd;
        BoxCompareNode bcnStart;

        try
        {
            Collider[] colls;
            colls = Physics.OverlapBox(end, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, 1 << 14); //end
            colls = Physics.OverlapBox(start, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, 1 << 14); //start

            NavBox endBox = colls[0].GetComponent<NavBox>();
            NavBox startBox = colls[0].GetComponent<NavBox>();

            bcnEnd = new BoxCompareNode(endBox, null);
            bcnStart = new BoxCompareNode(startBox, bcnEnd);
        }
        catch (System.IndexOutOfRangeException e)
        {
            return null;
        }
        
        list = new Dictionary<NavBox, BoxCompareNode>();
        pq = new PriorityQueue();

        bcnStart.Known = false;
        bcnStart.DistanceTraveled = 0;
        list[bcnStart.GetBox()] = bcnStart;
        bcnStart.position = start;

        pq.Insert(bcnStart);
        Vector3 currentPosition = start;

        while (!bcnEnd.Known && !(pq.Size() == 0))
        {
            BoxCompareNode box = pq.DeleteMin();
            currentPosition = box.position;

            if(box.GetBox() == bcnEnd.GetBox())
            {
                break;
            }

            if (!box.Known)
            {
                box.Known = true;

                foreach (NavBox aBox in box.Neighbours)
                {
                    BoxCompareNode compBox = new BoxCompareNode(aBox, bcnEnd);
                    if (list.ContainsKey(aBox))
                    {
                        compBox = list[aBox];
                    }
                    else
                    {
                        list[aBox] = compBox;
                    }

                    if (!compBox.Known)
                    {
                        //Vector3 nextPosition = compBox.GetBox().Coll.ClosestPointOnBounds(currentPosition);
                        Vector3 nextPosition = compBox.GetBox().Coll.center + compBox.GetBox().transform.position;
                        float distance = box.DistanceTraveled + Vector3.Distance(currentPosition, nextPosition);
                        if (distance < compBox.DistanceTraveled)
                        {
                            compBox.DistanceTraveled = distance;
                            compBox.Previous = box;
                            compBox.position = nextPosition;
                            pq.Insert(compBox);
                        }
                    }
                }
            }
        }

        SortedList<float, Vector3> paths = new SortedList<float, Vector3>();

        for (BoxCompareNode b = list[bcnEnd.GetBox()]; b != null; b = list[b.GetBox()].Previous)
        {
            paths.Add(b.DistanceTraveled, b.position);
        }

        return paths;
    }
}
