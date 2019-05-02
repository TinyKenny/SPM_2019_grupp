﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfindning : MonoBehaviour
{
    //private BoxCompareNode start;
    //private BoxCompareNode end;
    private PriorityQueue pq;
    private Dictionary<NavBox, BoxCompareNode> list;

    //public List<GameObject> boxes = new List<GameObject>();
    //public NavBox startb, endb;
    public SortedList<float, Vector3> Paths = new SortedList<float, Vector3>();
    public GameObject NavmeshRender;

    private void Awake()
    {
        
        //BoxCompareNode e = new BoxCompareNode(endb, null, null);
        //BoxCompareNode s = new BoxCompareNode(startb, e, null);
        //FindPath(s, transform.position, e);

        //boxes.Add(endb.gameObject);
        //Debug.Log(list[endb].DistanceTraveled);
        //for (BoxCompareNode b = list[endb].Previous; !b.Equals(s); b = list[b.GetBox()].Previous)
        //{
        //    boxes.Add(b.GetBox().gameObject);
        //}

        //boxes.Add(startb.gameObject);
    }

    
    public void FindPath(BoxCompareNode start, Vector3 startPosition, BoxCompareNode end)
    {
        Paths.Clear();
        list = new Dictionary<NavBox, BoxCompareNode>();
        pq = new PriorityQueue();

        foreach (NavBox b in NavmeshRender.GetComponent<NavmeshRenderer>().boxes)
        {
            BoxCompareNode bcn = new BoxCompareNode(b, end);
            bcn.DistanceTraveled = Mathf.Infinity;
            bcn.Known = false;
            if (b != null)
                list.Add(b, bcn);
        }

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
            if (!box.Known)
            {
                box.Known = true;

                foreach (NavBox aBox in box.Neighbours)
                {
                    BoxCompareNode compBox = list[aBox];
                    if (!compBox.Known)
                    {
                        Vector3 nextPosition = compBox.GetBox().GetComponent<BoxCollider>().ClosestPointOnBounds(currentPosition);
                        float distance = box.DistanceTraveled + Vector3.Distance(currentPosition, nextPosition);
                        if (distance < compBox.DistanceTraveled)
                        {
                            compBox.DistanceTraveled = distance;
                            compBox.Previous = box;
                            compBox.position = nextPosition;
                            list[aBox] = compBox;
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
