using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3D AStarPathfinding for flying enemies. This is the class used on every instance of a flying enemies to find 
/// paths between start and end. Requires a <see cref="NavmeshRenderer"/> that has generated a 3D NavMesh in 
/// order to function. Every flying enemy needs its own <see cref="AStarPathfindning"/> to be able to find paths.
/// </summary>
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
            LayerMask navLayer = LayerMask.GetMask(new string[] {"3DNavMesh"});
            Collider[] colls;

            colls = Physics.OverlapBox(end, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, navLayer); //end
            NavBox endBox = colls[0].GetComponent<NavBox>();

            colls = Physics.OverlapBox(start, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, navLayer); //start
            NavBox startBox = colls[0].GetComponent<NavBox>();

            bcnEnd = new BoxCompareNode(endBox, end);
            bcnStart = new BoxCompareNode(startBox, end);
        }
        catch (System.IndexOutOfRangeException)
        {
            return null;
        }

        list = new Dictionary<NavBox, BoxCompareNode>();
        pq = new PriorityQueue();

        bcnStart.Known = false;
        bcnStart.DistanceTraveled = 0;
        list[bcnStart.GetBox()] = bcnStart;
        bcnStart.Position = start;

        pq.Insert(bcnStart);
        Vector3 currentPosition = start;

        int laps = 0; // for testing purposes

        while (bcnEnd.Known == false && pq.Size() != 0)
        {
            laps++; // for testing purposes

            BoxCompareNode box = pq.DeleteMin();
            currentPosition = box.Position;

            if(box.GetBox() == bcnEnd.GetBox())
            {
                Debug.Log("End found");
                break;
            }

            if (!box.Known)
            {
                
                box.Known = true;

                foreach (NavBox aBox in box.Neighbours)
                {
                    BoxCompareNode compBox = new BoxCompareNode(aBox, end);
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
                        Vector3 nextPosition = compBox.GetBox().Coll.ClosestPointOnBounds(currentPosition);
                        float distance = box.DistanceTraveled + (currentPosition - nextPosition).sqrMagnitude;
                        if (distance < compBox.DistanceTraveled)
                        {
                            compBox.DistanceTraveled = distance;
                            compBox.Previous = box;
                            compBox.Position = nextPosition;
                            pq.Insert(compBox);
                        }
                    }
                }
            }
        }

        SortedList<float, Vector3> paths = new SortedList<float, Vector3>();

        Debug.Log("number of boxes checked: " + laps); // for testing purposes

        for (BoxCompareNode b = list[bcnEnd.GetBox()]; b != null; b = list[b.GetBox()].Previous)
        {
            paths.Add(b.DistanceTraveled, b.Position);
        }

        return paths;
    }
}
