using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfindning : MonoBehaviour
{
    private PriorityQueue pq;
    private Dictionary<NavBox, BoxCompareNode> list;

    public SortedList<float, Vector3> Paths = new SortedList<float, Vector3>();
    public GameObject NavmeshRender;
    
    public void FindPath(BoxCompareNode start, Vector3 startPosition, BoxCompareNode end)
    {
        Paths.Clear();
        list = new Dictionary<NavBox, BoxCompareNode>();
        pq = new PriorityQueue();

        start.Known = false;
        start.DistanceTraveled = 0;
        list[start.GetBox()] = start;
        start.position = startPosition;

        pq.Insert(start);
        Vector3 currentPosition = startPosition;

        while (!end.Known && !(pq.Size() == 0))
        {
            BoxCompareNode box = pq.DeleteMin();
            currentPosition = box.position;

            if(box.GetBox() == end.GetBox())
            {
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
                        //Vector3 nextPosition = compBox.GetBox().Coll.center + compBox.GetBox().transform.position;
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


        for (BoxCompareNode b = list[end.GetBox()]; b != null; b = list[b.GetBox()].Previous)
        {
            Paths.Add(b.DistanceTraveled, b.position);
        }
    }
}
